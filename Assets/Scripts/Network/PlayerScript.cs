using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour
{
	public NetworkVariable<string> playerName = new NetworkVariable<string>();
	public NetworkVariable<bool> isReady = new NetworkVariable<bool>();

	public override void OnNetworkSpawn()
	{
		if (IsOwner)
		{
			playerName.Value = PlayerPrefs.GetString("PlayerName");
		}
	}

	[ServerRpc]
	public void SetReadyStateServerRpc(bool ready)
	{
		isReady.Value = ready;
	}
}
