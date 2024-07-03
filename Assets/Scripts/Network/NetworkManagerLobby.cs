using System;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class NetworkManagerLobby : NetworkManager {
	[SerializeField] private int minPlayers = 2;
	[SerializeField] string menuScene = string.Empty;

	[Header("Room")]
	[SerializeField] private NetworkRoomPlayerLobby roomPlayerPrefab = null;

	public event Action OnClientConnected;
	public event Action OnClientDisconnected;

	public List<NetworkRoomPlayerLobby> RoomPlayers { get; } =  new List<NetworkRoomPlayerLobby>();

	public override void OnStartServer() {
		spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();
	}

	public override void OnStartClient() {
		var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

		foreach (var prefab in spawnablePrefabs) {
			NetworkClient.RegisterPrefab(prefab);
		}
	}

	public override void OnClientConnect() {
		base.OnClientConnect();
		OnClientConnected?.Invoke();
	}

	public override void OnClientDisconnect() {
		base.OnClientDisconnect();
		OnClientDisconnected?.Invoke();
	}

	public override void OnServerConnect(NetworkConnectionToClient conn) {
		if (numPlayers >= maxConnections) {
			conn.Disconnect();
			return;
		}

		if (SceneManager.GetActiveScene().name != menuScene) {
			conn.Disconnect();
			return;
		}
	}

	public override void OnServerAddPlayer(NetworkConnectionToClient conn) {
		if (SceneManager.GetActiveScene().name == menuScene) {
			bool isLeader = RoomPlayers.Count == 0;
			NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerPrefab);
			roomPlayerInstance.IsLeader = isLeader;

			NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
		}
	}

	public override void OnServerDisconnect(NetworkConnectionToClient conn) {
		if (conn.identity != null) {
			var player = conn.identity.GetComponent<NetworkRoomPlayerLobby>();
			RoomPlayers.Remove(player);
			NotifyPlayersOfReadyState();
		}

		base.OnServerDisconnect(conn);
	}

	public override void OnStopServer() {
		RoomPlayers.Clear();
	}

	public void NotifyPlayersOfReadyState() {
		foreach (var player in RoomPlayers) {
			player.HandleReadyToStart(IsReadyToStart());
		}
	}

	private bool IsReadyToStart() {
		if (numPlayers < minPlayers) { return false; }
		foreach (var player in RoomPlayers) {
			if (!player.IsReady) { return false; }
		}

		return true;
	}
}