using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Collections.Generic;

public class SyncSceneManager : NetworkBehaviour {
	[Header("Prefabs")]
	[SerializeField] private GameObject redPrefab;
	[SerializeField] private GameObject greenPrefab;
	[SerializeField] private GameObject bluePrefab;

	[Header("Spawnpoints")]
	[SerializeField] private Transform leftSpawnPoint;
	[SerializeField] private Transform rightSpawnPoint;

	[Header("Buttons")]
	[SerializeField] private Button redButton;
	[SerializeField] private Button greenButton;
	[SerializeField] private Button blueButton;

	private enum PrefabType {
		Blue,
		Red,
		Green
	}

	private void Start() {
		blueButton.onClick.AddListener(() => OnButtonClick(PrefabType.Blue));
		redButton.onClick.AddListener(() => OnButtonClick(PrefabType.Red));
		greenButton.onClick.AddListener(() => OnButtonClick(PrefabType.Green));
	}

	private void OnButtonClick(PrefabType prefabType) {
		if (IsHost) {
			SpawnSpriteServerRpc(prefabType, true);
		} else {
			SpawnSpriteServerRpc(prefabType, false);
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void SpawnSpriteServerRpc(PrefabType prefabType, bool isHost, ServerRpcParams rpcParams = default) {
		var spawnPoint = isHost ? leftSpawnPoint.position : rightSpawnPoint.position;
		DeleteExistingPrefab(spawnPoint);
		GameObject prefab = GetPrefabByType(prefabType);
		GameObject newSprite = Instantiate(prefab, spawnPoint, Quaternion.identity);
		newSprite.GetComponent<NetworkObject>().Spawn(true);

		// Notify clients about the new prefab
		NotifyClientsAboutNewPrefabClientRpc(newSprite.GetComponent<NetworkObject>().NetworkObjectId, prefabType, isHost);
	}

	private GameObject GetPrefabByType(PrefabType prefabType) {
		switch (prefabType) {
			case PrefabType.Blue:
				return bluePrefab;
			case PrefabType.Red:
				return redPrefab;
			case PrefabType.Green:
				return greenPrefab;
			default:
				return null;
		}
	}

	private void DeleteExistingPrefab(Vector3 spawnPosition) {
		List<NetworkObject> objectsToDelete = new List<NetworkObject>();

		foreach (var networkObject in NetworkManager.Singleton.SpawnManager.SpawnedObjectsList) {
			if (networkObject.transform.position == spawnPosition && networkObject.CompareTag("Character")) {
				objectsToDelete.Add(networkObject);
			}
		}

		foreach (var networkObject in objectsToDelete) {
			networkObject.Despawn(true);
		}
	}

	[ClientRpc]
	private void NotifyClientsAboutNewPrefabClientRpc(ulong networkObjectId, PrefabType prefabType, bool isHost) {
		if (IsHost) return;

		var spawnPoint = isHost ? leftSpawnPoint.position : rightSpawnPoint.position;
		DeleteExistingPrefab(spawnPoint);
		GameObject prefab = GetPrefabByType(prefabType);
		GameObject newSprite = Instantiate(prefab, spawnPoint, Quaternion.identity);
		newSprite.GetComponent<NetworkObject>().Spawn(true);
	}
}
