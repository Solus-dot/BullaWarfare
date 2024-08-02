using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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

	[Header("Disconnect UI")]
	[SerializeField] private GameObject leavePanel;
	[SerializeField] private TMP_Text leaveMessage;

	private TMP_Text moveButton1Text;
	private TMP_Text moveButton2Text;
	private TMP_Text moveButton3Text;
	private TMP_Text moveButton4Text;

	private int? hostSelectedMove = null;
	private int? clientSelectedMove = null;

	private string playerName;
	private string hostName;
	private string clientName;
	private string syncedClientName;

	private bool isDisconnectingPlayer = false;

	private void OnEnable() {
		NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
	}

	private void OnDisable() {
		NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
	}

	private void Start() {
		moveButton1Text = moveButton1.GetComponentInChildren<TMP_Text>();
		moveButton2Text = moveButton2.GetComponentInChildren<TMP_Text>();
		moveButton3Text = moveButton3.GetComponentInChildren<TMP_Text>();
		moveButton4Text = moveButton4.GetComponentInChildren<TMP_Text>();

		moveButton1.onClick.AddListener(() => OnMoveButtonClicked(0));
		moveButton2.onClick.AddListener(() => OnMoveButtonClicked(1));
		moveButton3.onClick.AddListener(() => OnMoveButtonClicked(2));
		moveButton4.onClick.AddListener(() => OnMoveButtonClicked(3));

		if (IsHost) {
			hostName = PlayerPrefs.GetString("Name");
			Debug.Log("Host is spawning character: " + SelectedCharacterData.HostCharacterName);
			SpawnCharacterServerRpc(SelectedCharacterData.HostCharacterName, true);
		} else {
			clientName = PlayerPrefs.GetString("Name");
			Debug.Log("Client is spawning character: " + SelectedCharacterData.ClientCharacterName);
			SpawnCharacterServerRpc(SelectedCharacterData.ClientCharacterName, false);
			SyncClientNameServerRpc(clientName);
		}

		leavePanel.SetActive(false);
		SetInitialUIState();
	}

	private void SetInitialUIState() {
		playerName = IsHost ? hostName : clientName;
		dialogueText.text = $"{(playerName)}, pick your move.";
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
			SelectMoveServerRpc(moveIndex);
			ShowWaitingForOpponentUI();
		}
		Debug.Log($"Before CheckMovesAndExecute: HostSelectedMove = {hostSelectedMove}, ClientSelectedMove = {clientSelectedMove}");
		CheckMovesAndExecute();
		Debug.Log($"After CheckMovesAndExecute: HostSelectedMove = {hostSelectedMove}, ClientSelectedMove = {clientSelectedMove}");
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
		UpdateHostUIAfterClientMoveSelection();
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

	[ClientRpc]
	private void ResetUIAfterMoveExecutionClientRpc() {
		ResetUIAfterMoveExecution();
	}

	private void ResetUIAfterMoveExecution() {
		moveText.gameObject.SetActive(false);
		dialogueText.gameObject.SetActive(true);
		moveButton1.gameObject.SetActive(true);
		moveButton2.gameObject.SetActive(true);
		moveButton3.gameObject.SetActive(true);
		moveButton4.gameObject.SetActive(true);

		playerName = IsHost ? hostName : clientName;
		dialogueText.text = $"{playerName}, pick your move.";
	}

	[ServerRpc(RequireOwnership = false)]
	private void ExecuteMovesServerRpc(int hostMove, int clientMove) {
		StartCoroutine(ExecuteMovesAndResetUI(hostMove, clientMove));
	}

	private IEnumerator ExecuteMovesAndResetUI(int hostMove, int clientMove) {
		Debug.Log($"Executing moves: HostMove = {hostMove}, ClientMove = {clientMove}");

		List<System.Action> moveActions = new List<System.Action> {
			() => {
				PerformMove(hostUnit, clientUnit, hostMove, true);
				DisplayMoveTextClientRpc(hostUnit.GetMove(hostMove).moveMessage.Replace("(opp_name)", clientUnit.unitName), 1.0f);
			},
			() => {
				PerformMove(clientUnit, hostUnit, clientMove, false);
				DisplayMoveTextClientRpc(clientUnit.GetMove(clientMove).moveMessage.Replace("(opp_name)", hostUnit.unitName), 1.0f);
			}
		};

		foreach (var moveAction in moveActions) {
			moveAction();
			yield return new WaitForSeconds(1.0f); // Ensure the display text coroutine completes before proceeding
		}

		if (!CheckForGameOver()) {
			yield return new WaitForSeconds(1.0f); // Additional delay before resetting UI
			ResetUIAfterMoveExecutionClientRpc();
		}
	}

	private bool CheckForGameOver() {
		Debug.Log($"Checking for game over: Host HP = {hostUnit.currentHP}, Client HP = {clientUnit.currentHP}");
		if (hostUnit.currentHP <= 0 || clientUnit.currentHP <= 0) {
			string winnerName = hostUnit.currentHP > 0 ? SelectedCharacterData.HostCharacterName : SelectedCharacterData.ClientCharacterName;
			string winnerPlayerName = hostUnit.currentHP > 0 ? hostName : syncedClientName; // Use the synced client name
			Debug.Log($"Game over detected. Winner: {winnerName}");
			EndGameClientRpc($"{winnerName} ({winnerPlayerName}) has Won!");
			return true;
		}
		return false;
	}

	[ClientRpc]
	private void EndGameClientRpc(string message) {
		Debug.Log("Ending game with message: " + message);
		dialogueText.gameObject.SetActive(false);
		moveButton1.gameObject.SetActive(false);
		moveButton2.gameObject.SetActive(false);
		moveButton3.gameObject.SetActive(false);
		moveButton4.gameObject.SetActive(false);

		moveText.gameObject.SetActive(true);
		moveText.text = message + "\nPress Spacebar to return to lobby.";

		StartCoroutine(WaitForExitInput());
	}

	private IEnumerator WaitForExitInput() {
		while (!Input.GetKeyDown(KeyCode.Space)) {
			yield return null;
		}

		isDisconnectingPlayer = true;
		NotifyOpponentOfDisconnectServerRpc("Opponent Left\nReturning to Lobby...");
		NetworkManager.Singleton.Shutdown();
		ShowDisconnectPanel("Returning to Lobby...");
		SceneManager.LoadScene("LobbyScene");
	}

	[ServerRpc(RequireOwnership = false)]
	private void NotifyOpponentOfDisconnectServerRpc(string message) {
		ShowOpponentLeftPanelClientRpc(message);
	}

	[ClientRpc]
	private void NotifyHostDisconnectedClientRpc() {
		if (!isDisconnectingPlayer) {
			ShowDisconnectPanel("Host Connection Lost\nReturning to Lobby...");
		}
	}

	[ClientRpc]
	private void ShowOpponentLeftPanelClientRpc(string message) {
		if (!isDisconnectingPlayer) {
			ShowDisconnectPanel(message);
		}
	}

	private void ShowDisconnectPanel(string message) {
		leaveMessage.text = message;
		leavePanel.SetActive(true);
		StartCoroutine(WaitAndReturnToLobby());
	}

	private IEnumerator WaitAndReturnToLobby() {
		yield return new WaitForSeconds(2);
		SceneManager.LoadScene("LobbyScene");
	}

	private void PerformMove(Unit attacker, Unit defender, int moveIndex, bool isHost) {
		Debug.Log($"{(isHost ? "Host" : "Client")} performing move {moveIndex} on {(isHost ? "Client" : "Host")}.");
		InitializeMoveset();

		bool isDefeated = false;
		Move move = attacker.GetMove(moveIndex);
		string message = move.moveMessage.Replace("(opp_name)", defender.unitName);

		if (move.isDamaging) {
			int damage = CalculateDamage(attacker, defender, move);
			isDefeated = defender.TakeDamage(damage);
			message = message.Replace("(value)", damage.ToString());
		}

		if (move.isStatChange) {
			attacker.TakeBuff(move.selfAttackChange, move.selfDefenseChange, move.selfSpeedChange);
			defender.TakeBuff(move.oppAttackChange, move.oppDefenseChange, move.oppSpeedChange);
		}

		if (move.isHealingMove) {
			int atkHealAmount = (move.selfHealAmount * attacker.maxHP / 100);
			int defHealAmount = (move.oppHealAmount * defender.maxHP / 100);
			if (atkHealAmount != 0) attacker.Heal(atkHealAmount);
			if (defHealAmount != 0) defender.Heal(defHealAmount);
		}

		UpdateHealthClientRpc(defender.currentHP, !isHost);
		UpdateHealthClientRpc(attacker.currentHP, isHost);

		UpdateStatChangesClientRpc(attacker.attackStage, attacker.defenseStage, attacker.speedStage, isHost, attacker.name);
		UpdateStatChangesClientRpc(defender.attackStage, defender.defenseStage, defender.speedStage, !isHost, defender.name);

		if (isDefeated) {
			Debug.Log($"{(isHost ? "Client" : "Host")} is defeated. Checking for game over.");
			CheckForGameOver();
		}

	}


	private int CalculateDamage(Unit attacker, Unit defender, Move move) {
		return ((attacker.attack * move.damage) / defender.defense) + move.trueDamage;
	}

	[ClientRpc]
	private void UpdateHealthClientRpc(int currentHP, bool isHost) {
		if (isHost) {
			hostHUD.SetHP(currentHP);
		} else {
			clientHUD.SetHP(currentHP);
		}
	}

	[ClientRpc]
	private void UpdateStatChangesClientRpc(int attackStage, int defenseStage, int speedStage, bool isHost, string unitName) {
		Debug.Log($"Updating stat changes for {unitName} - {(isHost ? "Host" : "Client")}. Attack Stage: {attackStage}, Defense Stage: {defenseStage}, Speed Stage: {speedStage}");

		if (isHost) {
			hostUnit.attackStage = attackStage;
			hostUnit.defenseStage = defenseStage;
			hostUnit.speedStage = speedStage;
			hostHUD.SetStatChange(hostUnit);
		} else {
			clientUnit.attackStage = attackStage;
			clientUnit.defenseStage = defenseStage;
			clientUnit.speedStage = speedStage;
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

	[ClientRpc]
	private void DisplayMoveTextClientRpc(string text, float delay) {
		StartCoroutine(DisplayMoveText(text, delay));
	}

	private IEnumerator DisplayMoveText(string text, float delay) {
		moveText.text = text;
		moveText.gameObject.SetActive(true);
		yield return new WaitForSeconds(delay);
		moveText.gameObject.SetActive(false);
	}

	[ServerRpc(RequireOwnership = false)]
	private void SpawnCharacterServerRpc(string characterName, bool isHost, ServerRpcParams rpcParams = default) {
		Character character = battleCharacters.Find(c => c.name == characterName);
		if (character == null) {
			Debug.LogError($"Character {characterName} not found in the battleCharacters list.");
			return;
		}

		GameObject characterGO = Instantiate(character.prefab, (isHost ? hostSpawnPoint.position : clientSpawnPoint.position) + new Vector3(0, 1f, 0), Quaternion.identity);
		NetworkObject networkObject = characterGO.GetComponent<NetworkObject>();
		networkObject.SpawnWithOwnership(rpcParams.Receive.SenderClientId);

		if (isHost) {
			hostUnit = characterGO.GetComponent<Unit>();
			hostHUD.SetHUD(hostUnit);
			UpdateMoveButtons();
		} else {
			clientUnit = characterGO.GetComponent<Unit>();
			clientHUD.SetHUD(clientUnit);
		}
		
		SpawnCharacterClientRpc(characterGO.GetComponent<NetworkObject>().NetworkObjectId, characterName, isHost);
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

	private void OnClientDisconnect(ulong clientId) {
		if (!isDisconnectingPlayer) {
			if (NetworkManager.Singleton.IsServer) {
				if (clientId == NetworkManager.ServerClientId) {
					NotifyHostDisconnectedClientRpc();
				} else {
					NotifyOpponentOfDisconnectServerRpc("Opponent has disconnected\nReturning to Lobby...");
				}
			} else {
				NotifyOpponentOfDisconnectServerRpc("Opponent has disconnected\nReturning to Lobby...");
			}
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void SyncClientNameServerRpc(string name, ServerRpcParams rpcParams = default) {
		syncedClientName = name;
		Debug.Log($"Client name synced to host: {name}");
		SyncClientNameClientRpc(name); // Inform other clients (like the host) of this name
	}

	[ClientRpc]
	private void SyncClientNameClientRpc(string name) {
		if (!IsHost) {
			clientName = name;
			Debug.Log($"Client name synced: {name}");
		}
	}

	void InitializeMoveset() {
		Sohom.Initialize();
		Ravi.Initialize();
		Manas.Initialize();
		Harsh.Initialize();
		Arya.Initialize();
		Khush.Initialize();
		Aditi.Initialize();
		Sarv.Initialize();
		Daksh.Initialize();
		Aarav.Initialize();
		Hima.Initialize();
		Vrush.Initialize();
		Mrman.Initialize();
	}
}
