using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class Character {
	public string name;
	public GameObject prefab;
	public Button button;
}

public static class SelectedCharacterData {
	public static string HostCharacterName { get; set; }
	public static string ClientCharacterName { get; set; }
}

public class MPCharacterSelectManager : NetworkBehaviour {
	[Header("Characters")]
	public List<Character> characters;

	[Header("Selection Frames")]
	[SerializeField] private Sprite defaultFrame;
	[SerializeField] private Sprite p1SelectFrame;
	[SerializeField] private Sprite p2SelectFrame;
	[SerializeField] private Sprite p1P2SelectFrame;

	private Button hostSelectedButton;
	private Button clientSelectedButton;
	private string hostPreviousCharacter = null;
	private string clientPreviousCharacter = null;

	[Header("Spawnpoints")]
	[SerializeField] private Transform leftSpawnPoint;
	[SerializeField] private Transform rightSpawnPoint;

	[Header("UI Elements")]
	[SerializeField] private Button readyButton;
	[SerializeField] private TMP_Text hostReadyText;
	[SerializeField] private TMP_Text clientReadyText;
	[SerializeField] private GameObject hostStatsPanel;
	[SerializeField] private GameObject clientStatsPanel;
	[SerializeField] private TMP_Text countdownText;

	private TMP_Text hostNameText;
	private TMP_Text hostStatsText;
	private TMP_Text clientNameText;
	private TMP_Text clientStatsText;

	private TMP_Text hostMoveNameText;
	private TMP_Text hostMoveDescText;
	private TMP_Text clientMoveNameText;
	private TMP_Text clientMoveDescText;

	private Button hostLeftButton;
	private Button hostRightButton;
	private Button clientLeftButton;
	private Button clientRightButton;

	private int hostCurrentMoveIndex = 0;
	private int clientCurrentMoveIndex = 0;
	private List<Move> hostMoves;
	private List<Move> clientMoves;

	private bool isReady = false;
	private bool characterSelected = false;

	private bool hostReady = false;
	private bool clientReady = false;

	private void Start() {
		foreach (var character in characters) {
			var charCopy = character;
			charCopy.button.onClick.AddListener(() => OnButtonClick(charCopy));
		}

		readyButton.onClick.AddListener(OnReadyButtonClick);
		readyButton.interactable = false;  // Disable the ready button initially

		// Get TMP_Text components from host and client stats panels
		hostNameText = hostStatsPanel.GetComponentsInChildren<TMP_Text>()[0];
		hostStatsText = hostStatsPanel.GetComponentsInChildren<TMP_Text>()[1];
		hostMoveNameText = hostStatsPanel.GetComponentsInChildren<TMP_Text>()[2];
		hostMoveDescText = hostStatsPanel.GetComponentsInChildren<TMP_Text>()[3];

		clientNameText = clientStatsPanel.GetComponentsInChildren<TMP_Text>()[0];
		clientStatsText = clientStatsPanel.GetComponentsInChildren<TMP_Text>()[1];
		clientMoveNameText = clientStatsPanel.GetComponentsInChildren<TMP_Text>()[2];
		clientMoveDescText = clientStatsPanel.GetComponentsInChildren<TMP_Text>()[3];

		// Get Button components for cycling through moves
		hostLeftButton = hostStatsPanel.GetComponentsInChildren<Button>()[0];
		hostRightButton = hostStatsPanel.GetComponentsInChildren<Button>()[1];
		clientLeftButton = clientStatsPanel.GetComponentsInChildren<Button>()[0];
		clientRightButton = clientStatsPanel.GetComponentsInChildren<Button>()[1];

		hostLeftButton.onClick.AddListener(() => OnMoveButtonClick(false, true));
		hostRightButton.onClick.AddListener(() => OnMoveButtonClick(true, true));
		clientLeftButton.onClick.AddListener(() => OnMoveButtonClick(false, false));
		clientRightButton.onClick.AddListener(() => OnMoveButtonClick(true, false));

		hostStatsPanel.SetActive(false);
		clientStatsPanel.SetActive(false);
		countdownText.gameObject.SetActive(false);

		UpdateReadyText(false, IsHost);
	}

	private void OnButtonClick(Character character) {
		if (isReady) return;  // Prevent button click if already ready

		if (IsHost) {
			UpdateButtonImage(hostPreviousCharacter, character.name, true);
			SelectedCharacterData.HostCharacterName = character.name;  // Store host character
			hostPreviousCharacter = character.name;  // Update previous character
			SpawnSpriteServerRpc(character.name, true);
		} else {
			UpdateButtonImage(clientPreviousCharacter, character.name, false);
			SelectedCharacterData.ClientCharacterName = character.name;  // Store client character
			clientPreviousCharacter = character.name;  // Update previous character
			SpawnSpriteServerRpc(character.name, false);
		}

		// Enable the ready button after a character has been selected
		if (!characterSelected) {
			characterSelected = true;
			readyButton.interactable = true;
		}

		UpdateCharacterStats(character);
	}

	private void UpdateButtonImage(string previousCharacterName, string newCharacterName, bool isHost) {
		Debug.Log($"Updating button image. Previous: {previousCharacterName}, New: {newCharacterName}, IsHost: {isHost}");
		foreach (var character in characters) {
			if (character.name == previousCharacterName) {
				Debug.Log($"Resetting previous character {previousCharacterName} to default or appropriate frame.");
				if (isHost) {
					if (SelectedCharacterData.ClientCharacterName == previousCharacterName) {
						character.button.image.sprite = p2SelectFrame;
					} else {
						character.button.image.sprite = defaultFrame;
					}
				} else {
					if (SelectedCharacterData.HostCharacterName == previousCharacterName) {
						character.button.image.sprite = p1SelectFrame;
					} else {
						character.button.image.sprite = defaultFrame;
					}
				}
			}

			if (character.name == newCharacterName) {
				Debug.Log($"Updating new character {newCharacterName} to appropriate frame.");
				if (isHost) {
					if (SelectedCharacterData.ClientCharacterName == newCharacterName) {
						character.button.image.sprite = p1P2SelectFrame;
					} else {
						character.button.image.sprite = p1SelectFrame;
					}
				} else {
					if (SelectedCharacterData.HostCharacterName == newCharacterName) {
						character.button.image.sprite = p1P2SelectFrame;
					} else {
						character.button.image.sprite = p2SelectFrame;
					}
				}
			}
		}

		UpdateButtonImageClientRpc(previousCharacterName, newCharacterName, isHost);
	}

	[ClientRpc]
	private void UpdateButtonImageClientRpc(string previousCharacterName, string newCharacterName, bool isHost) {
		Debug.Log($"ClientRpc: Updating button image. Previous: {previousCharacterName}, New: {newCharacterName}, IsHost: {isHost}");
		foreach (var character in characters) {
			if (character.name == previousCharacterName) {
				if (isHost) {
					if (SelectedCharacterData.ClientCharacterName == previousCharacterName) {
						character.button.image.sprite = p2SelectFrame;
					} else {
						character.button.image.sprite = defaultFrame;
					}
				} else {
					if (SelectedCharacterData.HostCharacterName == previousCharacterName) {
						character.button.image.sprite = p1SelectFrame;
					} else {
						character.button.image.sprite = defaultFrame;
					}
				}
			}

			if (character.name == newCharacterName) {
				if (isHost) {
					if (SelectedCharacterData.ClientCharacterName == newCharacterName) {
						character.button.image.sprite = p1P2SelectFrame;
					} else {
						character.button.image.sprite = p1SelectFrame;
					}
				} else {
					if (SelectedCharacterData.HostCharacterName == newCharacterName) {
						character.button.image.sprite = p1P2SelectFrame;
					} else {
						character.button.image.sprite = p2SelectFrame;
					}
				}
			}
		}
	}

	private void UpdateCharacterStats(Character character) {
		var unit = character.prefab.GetComponent<Unit>();
		string stats = $"HP: {unit.maxHP}\nAtk: {unit.attack}\nDef: {unit.defense}";

		if (IsHost) {
			hostNameText.text = unit.unitName;
			hostStatsText.text = stats;
			hostStatsPanel.SetActive(true);
			hostMoves = GetMovesForUnit(unit);
			hostCurrentMoveIndex = 0;
			UpdateMoveText(true);
			UpdateCharacterStatsClientRpc(unit.unitName, stats, true);
			for (int i = 0; i < hostMoves.Count; i++) {
				UpdateCharacterMoveClientRpc(i, hostMoves[i].moveName, hostMoves[i].moveDesc, true);
			}
		} else {
			clientNameText.text = unit.unitName;
			clientStatsText.text = stats;
			clientStatsPanel.SetActive(true);
			clientMoves = GetMovesForUnit(unit);
			clientCurrentMoveIndex = 0;
			UpdateMoveText(false);
			UpdateCharacterStatsServerRpc(unit.unitName, stats);
			for (int i = 0; i < clientMoves.Count; i++) {
				UpdateCharacterMoveServerRpc(i, clientMoves[i].moveName, clientMoves[i].moveDesc);
			}
		}
	}

	private List<Move> GetMovesForUnit(Unit unit) {
		List<Move> moves = new List<Move>();
		for (int i = 0; i < 4; i++) {
			moves.Add(unit.GetMove(i));
		}
		return moves;
	}

	private void UpdateMoveText(bool isHost) {
		if (isHost) {
			if (hostMoves != null && hostMoves.Count > 0) {
				var move = hostMoves[hostCurrentMoveIndex];
				hostMoveNameText.text = move.moveName;
				hostMoveDescText.text = move.moveDesc;
			}
		} else {
			if (clientMoves != null && clientMoves.Count > 0) {
				var move = clientMoves[clientCurrentMoveIndex];
				clientMoveNameText.text = move.moveName;
				clientMoveDescText.text = move.moveDesc;
			}
		}
	}

	private void OnMoveButtonClick(bool next, bool isHost) {
		if (isHost) {
			if (next) {
				hostCurrentMoveIndex = (hostCurrentMoveIndex + 1) % hostMoves.Count;
			} else {
				hostCurrentMoveIndex = (hostCurrentMoveIndex - 1 + hostMoves.Count) % hostMoves.Count;
			}
			UpdateMoveText(true);
		} else {
			if (next) {
				clientCurrentMoveIndex = (clientCurrentMoveIndex + 1) % clientMoves.Count;
			} else {
				clientCurrentMoveIndex = (clientCurrentMoveIndex - 1 + clientMoves.Count) % clientMoves.Count;
			}
			UpdateMoveText(false);
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void UpdateCharacterStatsServerRpc(string name, string stats, ServerRpcParams rpcParams = default) {
		UpdateCharacterStatsClientRpc(name, stats, false);
	}

	[ClientRpc]
	private void UpdateCharacterStatsClientRpc(string name, string stats, bool isHost) {
		if (isHost) {
			hostNameText.text = name;
			hostStatsText.text = stats;
			hostStatsPanel.SetActive(true);
			UpdateMoveText(true);
		} else {
			clientNameText.text = name;
			clientStatsText.text = stats;
			clientStatsPanel.SetActive(true);
			UpdateMoveText(false);
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void UpdateCharacterMoveServerRpc(int index, string moveName, string moveDesc, ServerRpcParams rpcParams = default) {
		UpdateCharacterMoveClientRpc(index, moveName, moveDesc, false);
	}

	[ClientRpc]
	private void UpdateCharacterMoveClientRpc(int index, string moveName, string moveDesc, bool isHost) {
		if (isHost) {
			if (hostMoves == null) hostMoves = new List<Move>();
			if (index >= hostMoves.Count) hostMoves.Add(new Move { moveName = moveName, moveDesc = moveDesc });
			else hostMoves[index] = new Move { moveName = moveName, moveDesc = moveDesc };
			UpdateMoveText(true);
		} else {
			if (clientMoves == null) clientMoves = new List<Move>();
			if (index >= clientMoves.Count) clientMoves.Add(new Move { moveName = moveName, moveDesc = moveDesc });
			else clientMoves[index] = new Move { moveName = moveName, moveDesc = moveDesc };
			UpdateMoveText(false);
		}
	}

	private void OnReadyButtonClick() {
		isReady = !isReady;
		UpdateReadyText(isReady, IsHost);
		ToggleCharacterButtons(!isReady);

		if (IsHost) {
			hostReady = isReady;
			UpdateReadyStateClientRpc(isReady, IsHost);
		} else {
			clientReady = isReady;
			UpdateReadyStateServerRpc(isReady);
		}

		if (hostReady && clientReady) {
			StartCountdown();
		}
	}

	private void ToggleCharacterButtons(bool enable) {
		foreach (var character in characters) {
			character.button.interactable = enable;
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void UpdateReadyStateServerRpc(bool readyState, ServerRpcParams rpcParams = default) {
		clientReady = readyState;
		UpdateReadyText(readyState, false);

		if (hostReady && clientReady) {
			StartCountdown();
		}
	}

	[ClientRpc]
	private void UpdateReadyStateClientRpc(bool readyState, bool isHost) {
		if (isHost) {
			hostReady = readyState;
		} else {
			clientReady = readyState;
		}

		UpdateReadyText(readyState, isHost);
	}

	private void UpdateReadyText(bool readyState, bool isHost) {
		if (isHost) {
			hostReadyText.text = readyState ? "<color=green>Ready</color>" : "<color=red>Not Ready</color>";
		} else {
			clientReadyText.text = readyState ? "<color=green>Ready</color>" : "<color=red>Not Ready</color>";
		}
	}

	private void StartCountdown() {
		if (IsHost) {
			StartCoroutine(CountdownCoroutine());
		}
		UpdateCountdownClientRpc("3");
	}

	private IEnumerator CountdownCoroutine() {
		for (int i = 3; i >= 0; i--) {
			UpdateCountdownClientRpc(i.ToString());
			yield return new WaitForSeconds(1f);
		}

		NetworkManager.SceneManager.LoadScene("MPBattleScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
	}

	[ClientRpc]
	private void UpdateCountdownClientRpc(string text) {
		foreach (var character in characters) {
			character.button.gameObject.SetActive(false);
		}
		readyButton.gameObject.SetActive(false);
		countdownText.gameObject.SetActive(true);
		countdownText.text = text;
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

	public GameObject GetPrefabByName(string characterName) {
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
