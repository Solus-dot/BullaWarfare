using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class LobbyManager : NetworkBehaviour {
	private string playerId;

	[Header("Main Lobby Panel")]
	[SerializeField] private GameObject mainPanel;
	[SerializeField] private Button getLobbiesListButton;
	[SerializeField] private GameObject lobbyInfoPrefab;
	[SerializeField] private GameObject lobbiesInfoContent;
	[SerializeField] private TMP_InputField playerNameInputField;

	[Header("Create Room Panel")]
	[SerializeField] private GameObject createRoomPanel;
	[SerializeField] private TMP_InputField roomNameInputField;
	[SerializeField] private Toggle isPrivateToggle;
	[SerializeField] private Button createRoomButton;

	[Header("Room Panel")]
	[SerializeField] private GameObject roomPanel;
	[SerializeField] private TMP_Text roomName;
	[SerializeField] private TMP_Text roomCode;
	[SerializeField] private GameObject playerInfoContent;
	[SerializeField] private GameObject playerInfoPrefab;
	[SerializeField] private Button startGameButton;
	[SerializeField] private Button deleteLobbyButton;
	[SerializeField] private Button leaveRoomButton;
	[SerializeField] private GameObject loadingScreen;

	[Header("Join Room With Code")]
	[SerializeField] private GameObject joinRoomPanel;
	[SerializeField] private TMP_InputField roomCodeInputField;
	[SerializeField] private Button joinRoomButton;

	private Lobby currentLobby;
	private float lastRoomUpdateTime = 0f;
	private float roomUpdateInterval = 1.5f;

	private float lastHeartbeatTime = 0f;
	private float heartBeatTimer = 15f;

	private async void Start() {
		await UnityServices.InitializeAsync();
		AuthenticationService.Instance.SignedIn += () => {
			playerId = AuthenticationService.Instance.PlayerId;
			Debug.Log("Signed in " + playerId);
		};
		await AuthenticationService.Instance.SignInAnonymouslyAsync();

		playerNameInputField.onValueChanged.AddListener(delegate {
			PlayerPrefs.SetString("Name", playerNameInputField.text);
		});

		playerNameInputField.text = PlayerPrefs.GetString("Name");

		createRoomButton.onClick.AddListener(CreateLobby);
		joinRoomButton.onClick.AddListener(JoinLobbyWithCode);
		leaveRoomButton.onClick.AddListener(LeaveRoom);
	}

	private void Update() {
		HandleLobbiesListUpdate();
		HandleLobbyHeartbeat();
		HandleRoomUpdate();
	}

	private async void CreateLobby() {
		try {
			string lobbyName = roomNameInputField.text;
			string joinCode = await RelayManager.Instance.CreateRelay();

			CreateLobbyOptions options = new CreateLobbyOptions {
				IsPrivate = isPrivateToggle.isOn,
				Player = GetPlayer(),
				Data = new Dictionary<string, DataObject> {
					{"IsGameStarted", new DataObject(DataObject.VisibilityOptions.Member, "false")},
					{"JoinCode", new DataObject(DataObject.VisibilityOptions.Public, joinCode)}
				}
			};

			int maxPlayers = 2;
			currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
			Debug.Log("Room Created: " + currentLobby.Id);

			EnterRoom();
		} catch (LobbyServiceException e) {
			Debug.Log(e);
		}
	}


	private void EnterRoom() {
		if (mainPanel != null) mainPanel.SetActive(false);
		if (joinRoomPanel != null) joinRoomPanel.SetActive(false);
		if (createRoomPanel != null) createRoomPanel.SetActive(false);
		if (roomPanel != null) roomPanel.SetActive(true);

		if (roomName != null) roomName.text = currentLobby.Name;
		if (roomCode != null) roomCode.text = currentLobby.LobbyCode;

		VisualizeRoomDetails();
	}


	private async void HandleRoomUpdate() {
		if (currentLobby != null) {
			if (Time.time - lastRoomUpdateTime >= roomUpdateInterval) {
				lastRoomUpdateTime = Time.time;

				try {
					if (IsInLobby()) {
						currentLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
						VisualizeRoomDetails();

						if (IsLobbyDeleted()) {
							Debug.Log("Lobby has been deleted. Exiting room...");
							ExitRoom();
							currentLobby = null;
							return;
						}

						if (IsGameStarted()) {
							EnterGame();
						}
					}
				} catch (LobbyServiceException e) {
					Debug.Log(e);
					if (currentLobby.IsPrivate && (e.Reason == LobbyExceptionReason.LobbyNotFound || e.Reason == LobbyExceptionReason.Forbidden)) {
						currentLobby = null;
						ExitRoom();
					}
				}
			}
		}
	}

	private bool IsInLobby() {
		foreach (Unity.Services.Lobbies.Models.Player player in currentLobby.Players) {
			if (player.Id == playerId) {
				return true;
			}
		}
		currentLobby = null;
		return false;
	}

	private void VisualizeRoomDetails() {
		for (int i = 0; i < playerInfoContent.transform.childCount; i++) {
			Destroy(playerInfoContent.transform.GetChild(i).gameObject);
		}

		if (IsInLobby()) {
			foreach (Unity.Services.Lobbies.Models.Player player in currentLobby.Players) {
				GameObject newPlayerInfo = Instantiate(playerInfoPrefab, playerInfoContent.transform);
				newPlayerInfo.GetComponentInChildren<TMP_Text>().text = player.Data["PlayerName"].Value;
				if (IsHost() && player.Id != playerId) {
					Button KickButton = newPlayerInfo.GetComponentInChildren<Button>(true);
					KickButton.onClick.AddListener(() => KickPlayer(player.Id));
					KickButton.gameObject.SetActive(true);
				}
			}

			if (IsHost()) {
				if(currentLobby.AvailableSlots == 0) {
					startGameButton.onClick.AddListener(StartGame);
					startGameButton.gameObject.SetActive(true);
				}

				deleteLobbyButton.onClick.AddListener(DeleteLobby);
				deleteLobbyButton.gameObject.SetActive(true);
			} else {
				startGameButton.onClick.RemoveAllListeners();
				startGameButton.gameObject.SetActive(false);

				deleteLobbyButton.onClick.RemoveAllListeners(); // Remove listeners
				deleteLobbyButton.gameObject.SetActive(false); // Hide the delete button for non-hosts
			}
		} else {
			ExitRoom();
		}
	}

	private async void ListPublicLobbies() {
		try {
			QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync();
			VisualizeLobbyList(response.Results);
			Debug.Log("Available Public Lobbies: " + response.Results.Count);
			foreach (Lobby lobby in response.Results) {
				Debug.Log("Lobby Name: " + lobby.Name + " Lobby ID: " + lobby.Id);
			}
		} catch (LobbyServiceException e) {
			Debug.Log(e);
		}
	}

	private float updateLobbiesListTimer = 2f;
	private float lastLobbyUpdateTime = 0f;
	private void HandleLobbiesListUpdate() {
		if (Time.time - lastLobbyUpdateTime >= updateLobbiesListTimer) {
			lastLobbyUpdateTime = Time.time;
			ListPublicLobbies();
		}
	}

	private void VisualizeLobbyList(List<Lobby> publicLobbies) {
		for (int i = 0; i < lobbiesInfoContent.transform.childCount; i++) {
			Destroy(lobbiesInfoContent.transform.GetChild(i).gameObject);
		}

		foreach (Lobby lobby in publicLobbies) {
			if (lobby.AvailableSlots == 1 || lobby.AvailableSlots == 2) {
				GameObject newLobbyInfo = Instantiate(lobbyInfoPrefab, lobbiesInfoContent.transform);
				var lobbyDetailsTexts = newLobbyInfo.GetComponentsInChildren<TMP_Text>();
				lobbyDetailsTexts[0].text = lobby.Name;
				lobbyDetailsTexts[1].text = ((lobby.MaxPlayers - lobby.AvailableSlots).ToString() + "/" + lobby.MaxPlayers.ToString());
				newLobbyInfo.GetComponentInChildren<Button>().onClick.AddListener(() => JoinLobby(lobby.Id)); //We will call join lobby;
			}
		}
	}

	private async void JoinLobby(string lobbyId) {
		try {
			JoinLobbyByIdOptions options = new JoinLobbyByIdOptions {
				Player = GetPlayer()
			};

			currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, options);

			if (RelayManager.Instance != null) {
				string joinCode = currentLobby.Data["JoinCode"].Value;
				await RelayManager.Instance.JoinRelay(joinCode);
			} else {
				Debug.LogError("RelayManager instance is null.");
			}

			EnterRoom();
			Debug.Log("Players in room: " + currentLobby.Players.Count);
		} catch (LobbyServiceException e) {
			Debug.Log(e);
		}
	}

	private async void JoinLobbyWithCode() {
		string lobbyCode = roomCodeInputField.text;
		try {
			JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions {
				Player = GetPlayer()
			};

			currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, options);
			Debug.Log("Successfully joined the lobby with code: " + lobbyCode);
			EnterRoom();
			Debug.Log("Players in room: " + currentLobby.Players.Count);
		} catch (LobbyServiceException e) {
			Debug.Log(e);
		}
	}

	private async void HandleLobbyHeartbeat() {
		if (currentLobby != null && currentLobby.HostId == playerId) {
			if (Time.time - lastHeartbeatTime >= heartBeatTimer) {
				lastHeartbeatTime = Time.time;
				await LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
			}
		}
	}

	private bool IsHost() {
		if (currentLobby != null && currentLobby.HostId == playerId) {
			return true;
		}

		return false;
	}

	private Unity.Services.Lobbies.Models.Player GetPlayer() {
		string playerName = PlayerPrefs.GetString("Name");
		if (playerName == null || playerName == "") playerName = playerId;

		Unity.Services.Lobbies.Models.Player player = new Unity.Services.Lobbies.Models.Player {
			Data = new Dictionary<string, PlayerDataObject> {
				{"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName)}
			}
		};

		return player;
	}

	private async void LeaveRoom() {
		if (currentLobby != null) {
			try {
				await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, playerId);
				Debug.Log("Player left the room.");
				RelayManager.Instance.LeaveRelay();
				currentLobby = null;
				ExitRoom();
			} catch (LobbyServiceException e) {
				Debug.Log(e);
			}
		}
	}

	private async void DeleteLobby() {
		if (currentLobby != null && IsHost()) {
			try {
				UpdateLobbyOptions updateOptions = new UpdateLobbyOptions {
					Data = new Dictionary<string, DataObject> {
						{"IsLobbyDeleted", new DataObject(DataObject.VisibilityOptions.Public, "true")}
					}
				};

				await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, updateOptions);
				await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);
				currentLobby = null;
				Debug.Log("Lobby deleted successfully.");
				ExitRoom();
			} catch (LobbyServiceException e) {
				Debug.LogError($"Error deleting lobby: {e}");
			}
		}
	}



	private async void KickPlayer(string _playerId) {
		try {
			await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, _playerId);
		} catch (LobbyServiceException e) {
			Debug.Log(e);
		}
	}

	private void ExitRoom() {
		mainPanel.SetActive(true);
		joinRoomPanel.SetActive(false);
		createRoomPanel.SetActive(false);
		joinRoomPanel.SetActive(false);
		roomPanel.SetActive(false);
	}

	private async void StartGame() {
		if (currentLobby != null && IsHost()) {
			try {
				UpdateLobbyOptions updateOptions = new UpdateLobbyOptions {
					Data = new Dictionary<string, DataObject> {
						{"IsGameStarted", new DataObject(DataObject.VisibilityOptions.Member, "true")}
					}
				};

				currentLobby = await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, updateOptions);
			} catch (LobbyServiceException e) {
				Debug.Log(e);
			}
		}
	}

	private bool IsGameStarted() {
		if (currentLobby != null) {
			if (currentLobby.Data.ContainsKey("IsGameStarted") && currentLobby.Data["IsGameStarted"].Value == "true") {
				return true;
			}
		}
		return false;
	}

	private bool IsLobbyDeleted() {
		if (currentLobby != null) {
			if (currentLobby.Data.ContainsKey("IsLobbyDeleted") && currentLobby.Data["IsLobbyDeleted"].Value == "true") {
				return true;
			}
		}
		return false;
	}

	private async void EnterGame() {
		if (IsHost()) {
			string sceneName = "MPCharacterSelect";
			loadingScreen.SetActive(true);
			Debug.Log("Loading scene: " + sceneName);
			NetworkManager.Singleton.SceneManager.LoadScene("MPCharacterSelect", UnityEngine.SceneManagement.LoadSceneMode.Single);
		} else {
			loadingScreen.SetActive(true);
			string joinCode = currentLobby.Data["JoinCode"].Value;
			await RelayManager.Instance.JoinRelay(joinCode);
		}
	}
}