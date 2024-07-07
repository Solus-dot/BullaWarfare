using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviour {
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
	[SerializeField] private Button leaveRoomButton;

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
		getLobbiesListButton.onClick.AddListener(ListPublicLobbies);
		joinRoomButton.onClick.AddListener(JoinLobbyWithCode);
		leaveRoomButton.onClick.AddListener(LeaveRoom);
	}

	private void Update() {
		HandleLobbyHeartbeat();
		HandleRoomUpdate();
	}

	private async void CreateLobby() {
		try {
			string lobbyName = roomNameInputField.text;

			CreateLobbyOptions options = new CreateLobbyOptions {
				IsPrivate = isPrivateToggle.isOn,
				Player = GetPlayer(),
				Data = new Dictionary<string, DataObject> {
					{"IsGameStarted", new DataObject(DataObject.VisibilityOptions.Member, "false")}
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
		mainPanel.SetActive(false);
		joinRoomPanel.SetActive(false);
		createRoomPanel.SetActive(false);
		roomPanel.SetActive(true);

		roomName.text = currentLobby.Name;
		roomCode.text = currentLobby.LobbyCode;

		VisualizeRoomDetails();
	}

	private async void HandleRoomUpdate() {
		if (currentLobby != null) {
			// Check if the required time has passed since the last update
			if (Time.time - lastRoomUpdateTime >= roomUpdateInterval) {
				lastRoomUpdateTime = Time.time; // Update the last update time

				try {
					if (IsInLobby()) {
						currentLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
						VisualizeRoomDetails();

						// Check if the game has started
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
				startGameButton.onClick.AddListener(StartGame);
				startGameButton.gameObject.SetActive(true);
			} else {
				if (IsGameStarted()) {
					startGameButton.onClick.AddListener(EnterGame);
					startGameButton.gameObject.SetActive(true);
					startGameButton.GetComponentInChildren<TMP_Text>().text = "Enter Game";
				} else {
					startGameButton.onClick.RemoveAllListeners();
					startGameButton.gameObject.SetActive(false);
				}
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
			foreach(Lobby lobby in response.Results) {
				Debug.Log("Lobby Name: " + lobby.Name + " Lobby ID: " + lobby.Id);
			}
		} catch (LobbyServiceException e) {
			Debug.Log(e);
		}
	}

	private void VisualizeLobbyList(List<Lobby> publicLobbies) {
		for (int i = 0; i < lobbiesInfoContent.transform.childCount; i++) {
			Destroy(lobbiesInfoContent.transform.GetChild(i).gameObject);
		}

		foreach(Lobby lobby in publicLobbies) {
			GameObject newLobbyInfo = Instantiate(lobbyInfoPrefab, lobbiesInfoContent.transform);
			var lobbyDetailsTexts = newLobbyInfo.GetComponentsInChildren<TMP_Text>();
			lobbyDetailsTexts[0].text = lobby.Name;
			lobbyDetailsTexts[1].text = ((lobby.MaxPlayers - lobby.AvailableSlots).ToString() + "/" + lobby.MaxPlayers.ToString()); 
			newLobbyInfo.GetComponentInChildren<Button>().onClick.AddListener(() => JoinLobby(lobby.Id)); //We will call join lobby;
		}
	}

	private async void JoinLobby(string lobbyId) {
		try {
			JoinLobbyByIdOptions options = new JoinLobbyByIdOptions {
				Player = GetPlayer()
			};

			currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, options);
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
		try {
			await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, playerId);
			ExitRoom();
		} catch (LobbyServiceException e) {
			Debug.Log(e);
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
				// Let other players know to transition to the game
				EnterGame();

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

	private void EnterGame() {
		// Transition all players to the game scene
		Debug.Log("Entering game...");
		// Load your game scene here
		UnityEngine.SceneManagement.SceneManager.LoadScene("CharacterSelect");
	}
}
