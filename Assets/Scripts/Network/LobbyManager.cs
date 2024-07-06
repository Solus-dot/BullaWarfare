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
	[SerializeField] private Button createRoomButton;

	[Header("Room Panel")]
	[SerializeField] private GameObject roomPanel;
	[SerializeField] private TMP_Text roomName;
	[SerializeField] private TMP_Text roomCode;
	[SerializeField] private GameObject playerInfoContent;
	[SerializeField] private GameObject playerInfoPrefab;
	[SerializeField] private Button leaveRoomButton;

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
				Player = GetPlayer()
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
					currentLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
					VisualizeRoomDetails();
				} catch (LobbyServiceException e) {
					Debug.Log(e);
				}
			}
		}
	}

	private void VisualizeRoomDetails() {
		for (int i = 0; i < playerInfoContent.transform.childCount; i++) {
			Destroy(playerInfoContent.transform.GetChild(i).gameObject);
		}

		foreach (Unity.Services.Lobbies.Models.Player player in currentLobby.Players) {
			GameObject newPlayerInfo = Instantiate(playerInfoPrefab, playerInfoContent.transform);
			newPlayerInfo.GetComponentInChildren<TMP_Text>().text = player.Data["PlayerName"].Value;
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

	private void ExitRoom() {
		mainPanel.SetActive(true);
		createRoomPanel.SetActive(false);
		roomPanel.SetActive(false);
	}
}
