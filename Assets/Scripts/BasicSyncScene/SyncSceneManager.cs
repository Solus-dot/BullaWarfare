using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class Character {
	public string name;
	public GameObject prefab;
	public Button button;
}

public class SyncSceneManager : NetworkBehaviour {
	[Header("Characters")]
	[SerializeField] private List<Character> characters;

	[Header("Spawnpoints")]
	[SerializeField] private Transform leftSpawnPoint;
	[SerializeField] private Transform rightSpawnPoint;

	[Header("UI Elements")]
	[SerializeField] private Button readyButton;
	[SerializeField] private TMP_Text hostReadyText;
	[SerializeField] private TMP_Text clientReadyText;

	private bool isReady = false;
	private bool characterSelected = false;

	private void Start() {
		foreach (var character in characters) {
			var charCopy = character;
			charCopy.button.onClick.AddListener(() => OnButtonClick(charCopy));
		}

		readyButton.onClick.AddListener(OnReadyButtonClick);
		readyButton.interactable = false;  // Disable the ready button initially
		UpdateReadyText(false, IsHost);
	}

	private void OnButtonClick(Character character) {
		if (isReady) return;  // Prevent button click if already ready

		if (IsHost) {
			SpawnSpriteServerRpc(character.name, true);
		} else {
			SpawnSpriteServerRpc(character.name, false);
		}

		// Enable the ready button after a character has been selected
		if (!characterSelected) {
			characterSelected = true;
			readyButton.interactable = true;
		}
	}

	private void OnReadyButtonClick() {
		isReady = !isReady;
		UpdateReadyText(isReady, IsHost);
		ToggleCharacterButtons(!isReady);  // Disable buttons when ready

		if (IsHost) {
			UpdateReadyStateClientRpc(isReady, IsHost);
		} else {
			UpdateReadyStateServerRpc(isReady);
		}
	}

	private void ToggleCharacterButtons(bool enable) {
		foreach (var character in characters) {
			character.button.interactable = enable;
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void UpdateReadyStateServerRpc(bool readyState, ServerRpcParams rpcParams = default) {
		UpdateReadyText(readyState, false);
		UpdateReadyStateClientRpc(readyState, false);
	}

	[ClientRpc]
	private void UpdateReadyStateClientRpc(bool readyState, bool isHost) {
		UpdateReadyText(readyState, isHost);
	}

	private void UpdateReadyText(bool readyState, bool isHost) {
		if (isHost) {
			hostReadyText.text = readyState ? "<color=green>Ready</color>" : "<color=red>Not Ready</color>";
		} else {
			clientReadyText.text = readyState ? "<color=green>Ready</color>" : "<color=red>Not Ready</color>";
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void SpawnSpriteServerRpc(string characterName, bool isHost, ServerRpcParams rpcParams = default) {
		var spawnPoint = isHost ? leftSpawnPoint.position : rightSpawnPoint.position;
		DeleteExistingPrefab(spawnPoint);
		GameObject prefab = GetPrefabByName(characterName);
		GameObject newSprite = Instantiate(prefab, spawnPoint, Quaternion.identity);
		newSprite.GetComponent<NetworkObject>().Spawn(true);

		NotifyClientsAboutNewPrefabClientRpc(newSprite.GetComponent<NetworkObject>().NetworkObjectId, characterName, isHost);
	}

	private GameObject GetPrefabByName(string characterName) {
		foreach (var character in characters) {
			if (character.name == characterName) {
				return character.prefab;
			}
		}
		return null;
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
	private void NotifyClientsAboutNewPrefabClientRpc(ulong networkObjectId, string characterName, bool isHost) {
		if (IsHost) return;

		var spawnPoint = isHost ? leftSpawnPoint.position : rightSpawnPoint.position;
		DeleteExistingPrefab(spawnPoint);
		GameObject prefab = GetPrefabByName(characterName);
		GameObject newSprite = Instantiate(prefab, spawnPoint, Quaternion.identity);
		newSprite.GetComponent<NetworkObject>().Spawn(true);
	}
}
