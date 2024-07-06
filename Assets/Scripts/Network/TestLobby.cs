using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class TestLobby : MonoBehaviour {
	private Lobby hostLobby;
	private float heartbeatTimer;

	private async void Start() {
		await UnityServices.InitializeAsync();

		AuthenticationService.Instance.SignedIn += () => {
			Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
		};

		await AuthenticationService.Instance.SignInAnonymouslyAsync();

		// ListLobbies();
		// CreateLobby();
		// ListLobbies();
	}

	private void Update() {
		HandleLobbyHeartbeat();
	}

	private async void HandleLobbyHeartbeat() {
		if (hostLobby != null) {
			heartbeatTimer -= Time.deltaTime;
			if (heartbeatTimer < 0f) {
				float heartbeatTimerMax = 15f;
				heartbeatTimer = heartbeatTimerMax;

				await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
			}
		}
	}

	private async void CreateLobby() {
		try {
			string lobbyName = "MyLobby";
			int maxPlayers = 2;
			Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);

			hostLobby = lobby;
			Debug.Log("Created Lobby! Name: " + lobby.Name + " , Max players: " + lobby.MaxPlayers);
		} catch (LobbyServiceException e) {
			Debug.Log(e);
		}
	}

	private async void ListLobbies() {
		try {
			QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions {
				Count = 25,
				Filters = new List<QueryFilter> {
					new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
				},
				Order = new List<QueryOrder> {
					new QueryOrder(false, QueryOrder.FieldOptions.Created)
				}
			};

			QueryResponse queryresponse = await Lobbies.Instance.QueryLobbiesAsync();
			Debug.Log("Lobbies found: " + queryresponse.Results.Count);
			foreach (Lobby lobby in queryresponse.Results) {
				Debug.Log(lobby.Name + " " + lobby.MaxPlayers);
			}
		} catch (LobbyServiceException e) {
			Debug.Log(e);
		}
	}

	private async void JoinLobby() {
		try {
			QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();
			await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);
		} catch (LobbyServiceException e) {
			Debug.Log(e);
		}
	}
}