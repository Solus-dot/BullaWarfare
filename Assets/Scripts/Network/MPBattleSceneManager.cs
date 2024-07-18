// using UnityEngine;
// using Unity.Netcode;
// using System.Collections.Generic;

// public class MPBattleSceneManager : NetworkBehaviour {
//     [Header("Spawn Points")]
//     [SerializeField] private Transform hostSpawnPoint;
//     [SerializeField] private Transform clientSpawnPoint;

//     private void Start() {
//         SyncSceneManager syncSceneManager = FindObjectOfType<SyncSceneManager>();
//         if (syncSceneManager != null) {
//             List<Character> characters = syncSceneManager.characters;

//             if (IsHost) {
//                 SpawnCharacter(syncSceneManager.selectedCharacterName, hostSpawnPoint);
//             } else {
//                 SpawnCharacter(syncSceneManager.selectedCharacterName, clientSpawnPoint);
//             }
//         }
//     }

//     private void SpawnCharacter(string characterName, Transform spawnPoint) {
//         SyncSceneManager syncSceneManager = FindObjectOfType<SyncSceneManager>();
//         var prefab = syncSceneManager.GetPrefabByName(characterName);
//         if (prefab != null) {
//             Instantiate(prefab, spawnPoint.position, Quaternion.identity);
//         }
//     }

//     private bool IsHost {
//         get { return NetworkManager.Singleton.IsHost; }
//     }
// }
