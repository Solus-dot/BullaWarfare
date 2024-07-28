using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;

public class RelayManager : MonoBehaviour {
	public static RelayManager Instance { get; private set; }
	private const int MaxConnections = 2;

	private void Awake() {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}
	}

	public async Task<string> CreateRelay() {
		Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MaxConnections);
		string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

		UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
		transport.SetHostRelayData(
			allocation.RelayServer.IpV4,
			(ushort)allocation.RelayServer.Port,
			allocation.AllocationIdBytes,
			allocation.Key,
			allocation.ConnectionData
		);

		NetworkManager.Singleton.StartHost();
		return joinCode;
	}

	public async Task JoinRelay(string joinCode) {
		JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
		RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

		UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
		transport.SetClientRelayData(
			joinAllocation.RelayServer.IpV4,
			(ushort)joinAllocation.RelayServer.Port,
			joinAllocation.AllocationIdBytes,
			joinAllocation.Key,
			joinAllocation.ConnectionData,
			joinAllocation.HostConnectionData
		);

		NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
		NetworkManager.Singleton.StartClient();
	}

	public void LeaveRelay() {
		if (NetworkManager.Singleton.IsHost) {
			NetworkManager.Singleton.Shutdown();
		} else if (NetworkManager.Singleton.IsClient) {
			NetworkManager.Singleton.Shutdown();
		}
	}
}
