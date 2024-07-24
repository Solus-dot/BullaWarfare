using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class MPBattleSystem : NetworkBehaviour {
	[Header("Characters")]
	public List<Character> battleCharacters;
	private Unit hostUnit;
	private Unit clientUnit;

	[Header("Spawn Points")]
	[SerializeField] private Transform hostSpawnPoint;
	[SerializeField] private Transform clientSpawnPoint;

	[Header("HUD")]
	[SerializeField] private MPBattleHUD hostHUD;
	[SerializeField] private MPBattleHUD clientHUD;

	[Header("Move Buttons")]
	[SerializeField] private Button moveButton1;
	[SerializeField] private Button moveButton2;
	[SerializeField] private Button moveButton3;
	[SerializeField] private Button moveButton4;

	[Header("Dialogue")]
	[SerializeField] private TMP_Text dialogueText;
	[SerializeField] private TMP_Text moveText;

	private TMP_Text moveButton1Text;
	private TMP_Text moveButton2Text;
	private TMP_Text moveButton3Text;
	private TMP_Text moveButton4Text;

	private int? hostSelectedMove = null;
	private int? clientSelectedMove = null;

	private string playerName;

	private void Start() {
		playerName = PlayerPrefs.GetString("Name", "Player");

		moveButton1Text = moveButton1.GetComponentInChildren<TMP_Text>();
		moveButton2Text = moveButton2.GetComponentInChildren<TMP_Text>();
		moveButton3Text = moveButton3.GetComponentInChildren<TMP_Text>();
		moveButton4Text = moveButton4.GetComponentInChildren<TMP_Text>();

		moveButton1.onClick.AddListener(() => OnMoveButtonClicked(0));
		moveButton2.onClick.AddListener(() => OnMoveButtonClicked(1));
		moveButton3.onClick.AddListener(() => OnMoveButtonClicked(2));
		moveButton4.onClick.AddListener(() => OnMoveButtonClicked(3));

		if (IsHost) {
			Debug.Log("Host is spawning character: " + SelectedCharacterData.HostCharacterName);
			SpawnCharacterServerRpc(SelectedCharacterData.HostCharacterName, true);
		} else {
			Debug.Log("Client is spawning character: " + SelectedCharacterData.ClientCharacterName);
			SpawnCharacterServerRpc(SelectedCharacterData.ClientCharacterName, false);
		}

		SetInitialUIState();
	}

	private void SetInitialUIState() {
		dialogueText.text = $"{playerName}, pick your move.";
		moveText.gameObject.SetActive(false);
	}

	private void OnMoveButtonClicked(int moveIndex) {
		Debug.Log($"Move button {moveIndex + 1} clicked.");
		if (IsHost) {
			hostSelectedMove = moveIndex;
			Debug.Log($"Host selected move {moveIndex}");
			UpdateUIAfterMoveSelection(true);
		} else {
			clientSelectedMove = moveIndex;
			Debug.Log($"Client selected move {moveIndex}");
			SelectMoveServerRpc(moveIndex);  // Inform server about client move
			ShowWaitingForOpponentUI(); // Update client UI immediately
		}
		CheckMovesAndExecute(); // Check moves immediately after selection
	}

	private void UpdateUIAfterMoveSelection(bool isHost) {
		if (isHost) {
			if (!clientSelectedMove.HasValue) {
				ShowWaitingForOpponentUI();
			}
		} else {
			if (!hostSelectedMove.HasValue) {
				ShowWaitingForOpponentUI();
			}
		}
	}

	private void ShowWaitingForOpponentUI() {
		dialogueText.gameObject.SetActive(false);
		moveButton1.gameObject.SetActive(false);
		moveButton2.gameObject.SetActive(false);
		moveButton3.gameObject.SetActive(false);
		moveButton4.gameObject.SetActive(false);

		moveText.gameObject.SetActive(true);
		moveText.text = "Waiting for opponent...";
	}

	[ServerRpc(RequireOwnership = false)]
	private void SelectMoveServerRpc(int clientMove, ServerRpcParams rpcParams = default) {
		clientSelectedMove = clientMove;
		Debug.Log($"Server registered client move: {clientMove}");
		UpdateHostUIAfterClientMoveSelection(); // Ensure the host UI updates correctly
		CheckMovesAndExecute();
	}

	private void UpdateHostUIAfterClientMoveSelection() {
		if (hostSelectedMove.HasValue) {
			ShowWaitingForOpponentUI();
		}
	}

	private void CheckMovesAndExecute() {
		if (hostSelectedMove.HasValue && clientSelectedMove.HasValue) {
			Debug.Log("Both moves selected, executing moves.");
			ExecuteMovesServerRpc(hostSelectedMove.Value, clientSelectedMove.Value);
			hostSelectedMove = null;
			clientSelectedMove = null;
		}
	}

	private void ResetUIAfterMoveExecution() {
		moveText.gameObject.SetActive(false);
		dialogueText.gameObject.SetActive(true);
		moveButton1.gameObject.SetActive(true);
		moveButton2.gameObject.SetActive(true);
		moveButton3.gameObject.SetActive(true);
		moveButton4.gameObject.SetActive(true);

		dialogueText.text = $"{playerName}, pick your move.";
	}

	[ServerRpc(RequireOwnership = false)]
	private void ExecuteMovesServerRpc(int hostMove, int clientMove) {
		Debug.Log($"Executing moves: HostMove = {hostMove}, ClientMove = {clientMove}");
		List<System.Action> moveActions = new List<System.Action> {
			() => PerformMove(hostUnit, clientUnit, hostMove, true),
			() => PerformMove(clientUnit, hostUnit, clientMove, false)
		};

		moveActions.Shuffle(); // Randomize move execution order

		foreach (var moveAction in moveActions) {
			moveAction();
		}

		// Inform clients to reset their UI after moves execution
		ResetUIAfterMoveExecutionClientRpc();
	}

	[ClientRpc]
	private void ResetUIAfterMoveExecutionClientRpc() {
		ResetUIAfterMoveExecution();
	}

	private void PerformMove(Unit attacker, Unit defender, int moveIndex, bool isHost) {
		Debug.Log($"{(isHost ? "Host" : "Client")} performing move {moveIndex} on {(isHost ? "Client" : "Host")}.");
		// Get move and calculate damage
		Move move = attacker.GetMove(moveIndex);
		int damage = CalculateDamage(attacker, defender, move);

		// Apply damage
		defender.TakeDamage(damage);
		UpdateHealthClientRpc(defender.currentHP, defender.maxHP, !isHost);

		// Apply stat changes
		attacker.TakeBuff(move.selfAttackChange, move.selfDefenseChange);
		defender.TakeBuff(move.oppAttackChange, move.oppDefenseChange);

		// Update stat changes on both host and client
		UpdateStatChangesClientRpc(attacker.attackStage, attacker.defenseStage, isHost, attacker.name);
		UpdateStatChangesClientRpc(defender.attackStage, defender.defenseStage, !isHost, defender.name);
	}

	private int CalculateDamage(Unit attacker, Unit defender, Move move) {
		return (attacker.attack * move.damage) / defender.defense;
	}

	[ClientRpc]
	private void UpdateHealthClientRpc(int currentHP, int maxHP, bool isHost) {
		Debug.Log($"Updating health for {(isHost ? "Host" : "Client")} to {currentHP}/{maxHP}");
		if (isHost) {
			hostHUD.SetHP(currentHP);
		} else {
			clientHUD.SetHP(currentHP);
		}
	}

	[ClientRpc]
	private void UpdateStatChangesClientRpc(int attackStage, int defenseStage, bool isHost, string unitName) {
		Debug.Log($"Updating stat changes for {unitName} - {(isHost ? "Host" : "Client")}. Attack Stage: {attackStage}, Defense Stage: {defenseStage}");

		if (isHost) {
			hostUnit.attackStage = attackStage;
			hostUnit.defenseStage = defenseStage;
			hostHUD.SetStatChange(hostUnit);
		} else {
			clientUnit.attackStage = attackStage;
			clientUnit.defenseStage = defenseStage;
			clientHUD.SetStatChange(clientUnit);
		}
	}

	private void UpdateMoveButtons() {
		Unit localPlayerUnit = IsHost ? hostUnit : clientUnit;
		if (localPlayerUnit != null) {
			moveButton1Text.text = localPlayerUnit.GetMove(0).moveName;
			moveButton2Text.text = localPlayerUnit.GetMove(1).moveName;
			moveButton3Text.text = localPlayerUnit.GetMove(2).moveName;
			moveButton4Text.text = localPlayerUnit.GetMove(3).moveName;
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void SpawnCharacterServerRpc(string characterName, bool isHost, ServerRpcParams rpcParams = default) {
		var spawnPoint = (isHost ? hostSpawnPoint.position : clientSpawnPoint.position) + new Vector3(0, 1f, 0);
		GameObject prefab = GetPrefabByName(characterName);
		if (prefab != null) {
			Debug.Log("Spawning prefab: " + characterName + " at " + spawnPoint);
			GameObject newSprite = Instantiate(prefab, spawnPoint, Quaternion.identity);
			newSprite.GetComponent<NetworkObject>().Spawn(true);

			Unit unit = newSprite.GetComponent<Unit>();
			if (isHost) {
				hostUnit = unit;
				hostHUD.SetHUD(unit);
				UpdateMoveButtons(); // Update move buttons for the host
			} else {
				clientUnit = unit;
				clientHUD.SetHUD(unit);
			}

			// Pass the NetworkObjectId to the clients
			SpawnCharacterClientRpc(newSprite.GetComponent<NetworkObject>().NetworkObjectId, characterName, isHost);
		} else {
			Debug.LogError("Prefab not found for character: " + characterName);
		}
	}

	[ClientRpc]
	private void SpawnCharacterClientRpc(ulong networkObjectId, string characterName, bool isHost) {
		if (IsHost) return;
		StartCoroutine(WaitAndSetUpHUD(networkObjectId, isHost));
	}

	private IEnumerator WaitAndSetUpHUD(ulong networkObjectId, bool isHost) {
		NetworkObject networkObject = null;

		while (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out networkObject)) {
			yield return null;
		}

		Unit unit = networkObject.GetComponent<Unit>();

		if (isHost) {
			hostUnit = unit;
			Debug.Log("Client setting host HUD.");
			hostHUD.SetHUD(hostUnit);
		} else {
			clientUnit = unit;
			Debug.Log("Client setting client HUD.");
			clientHUD.SetHUD(clientUnit);
		}

		Debug.Log("Client: HUDs set up. HostUnit: " + (hostUnit != null) + ", ClientUnit: " + (clientUnit != null));
		UpdateMoveButtons();
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

public static class ListExtensions {
	private static System.Random rng = new System.Random();

	public static void Shuffle<T>(this IList<T> list) {
		int n = list.Count;
		while (n > 1) {
			n--;
			int k = rng.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}
}
