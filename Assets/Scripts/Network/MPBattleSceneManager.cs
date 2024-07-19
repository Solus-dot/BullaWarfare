using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class MPBattleSceneManager : NetworkBehaviour {
    [Header("Characters")]
    public List<Character> battleCharacters;

    [Header("Spawn Points")]
    [SerializeField] private Transform hostSpawnPoint;
    [SerializeField] private Transform clientSpawnPoint;

    private MPCharacterSelectManager mpCharacterSelectManager;

    private void Start() {
            if (IsHost) {
                Debug.Log("Host is spawning character: " + SelectedCharacterData.HostCharacterName);
                SpawnCharacterServerRpc(SelectedCharacterData.HostCharacterName, true);
            } else {
                Debug.Log("Client is spawning character: " + SelectedCharacterData.ClientCharacterName);
                SpawnCharacterServerRpc(SelectedCharacterData.ClientCharacterName, false);
            }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnCharacterServerRpc(string characterName, bool isHost, ServerRpcParams rpcParams = default) {
        var spawnPoint = isHost ? hostSpawnPoint.position : clientSpawnPoint.position;
        GameObject prefab = GetPrefabByName(characterName);
        if (prefab != null) {
            Debug.Log("Spawning prefab: " + characterName + " at " + spawnPoint);
            GameObject newSprite = Instantiate(prefab, spawnPoint, Quaternion.identity);
            newSprite.GetComponent<NetworkObject>().Spawn(true);

            SpawnCharacterClientRpc(newSprite.GetComponent<NetworkObject>().NetworkObjectId, characterName, isHost);
        } else {
            Debug.LogError("Prefab not found for character: " + characterName);
        }
    }

    [ClientRpc]
    private void SpawnCharacterClientRpc(ulong networkObjectId, string characterName, bool isHost) {
        if (IsHost) return;

        var spawnPoint = isHost ? hostSpawnPoint.position : clientSpawnPoint.position;
        GameObject prefab = mpCharacterSelectManager.GetPrefabByName(characterName);
        if (prefab != null) {
            Debug.Log("Client spawning prefab: " + characterName + " at " + spawnPoint);
            GameObject newSprite = Instantiate(prefab, spawnPoint, Quaternion.identity);
            newSprite.GetComponent<NetworkObject>().Spawn(true);
        } else {
            Debug.LogError("Client: Prefab not found for character: " + characterName);
        }
    }

    private bool IsHost {
        get { return NetworkManager.Singleton.IsHost; }
    }

    public GameObject GetPrefabByName(string characterName) {
		foreach (var character in battleCharacters) {
			if (character.name == characterName) {
				return character.prefab;
			}
		}
		return null;
	}
}
