using UnityEngine;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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

	private MPCharacterSelectManager mpCharacterSelectManager;
	private Vector3 Offset;

	private void Start() {
		Offset = new Vector3(0, 1f, 0);

		// Set up move buttons
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
	}

	[ServerRpc(RequireOwnership = false)]
	private void SpawnCharacterServerRpc(string characterName, bool isHost, ServerRpcParams rpcParams = default) {
		var spawnPoint = (isHost ? hostSpawnPoint.position : clientSpawnPoint.position) + Offset;
		GameObject prefab = GetPrefabByName(characterName);
		if (prefab != null) {
			Debug.Log("Spawning prefab: " + characterName + " at " + spawnPoint);
			GameObject newSprite = Instantiate(prefab, spawnPoint, Quaternion.identity);
			newSprite.GetComponent<NetworkObject>().Spawn(true);

			Unit unit = newSprite.GetComponent<Unit>();
			if (isHost) {
				hostUnit = unit;
				hostHUD.SetHUD(unit);
			} else {
				clientUnit = unit;
				clientHUD.SetHUD(unit);
			}

			SpawnCharacterClientRpc(newSprite.GetComponent<NetworkObject>().NetworkObjectId, characterName, isHost);
		} else {
			Debug.LogError("Prefab not found for character: " + characterName);
		}
	}

	[ClientRpc]
	private void SpawnCharacterClientRpc(ulong networkObjectId, string characterName, bool isHost) {
		// Avoid host execution
		if (IsHost) return;

		NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
		if (networkObject == null) {
			Debug.LogError("Client: NetworkObject not found for ID: " + networkObjectId);
			return;
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

	private void Update() {
		if (IsHost) {
			if (hostUnit != null && clientUnit != null) {
				if (hostUnit.currentHP <= 0 || clientUnit.currentHP <= 0) {
					EndBattle();
				}
			}
		}
	}

	private void EndBattle() {
		if (hostUnit.currentHP <= 0) {
			Debug.Log("Client wins!");
		} else if (clientUnit.currentHP <= 0) {
			Debug.Log("Host wins!");
		}
	}

	[ServerRpc(RequireOwnership = false)]
	public void ApplyDamageServerRpc(bool isHost, int damage) {
		if (isHost) {
			hostUnit.TakeDamage(damage);
			UpdateHealthClientRpc(hostUnit.currentHP, hostUnit.maxHP, isHost);
		} else {
			clientUnit.TakeDamage(damage);
			UpdateHealthClientRpc(clientUnit.currentHP, clientUnit.maxHP, isHost);
		}
	}

	[ClientRpc]
	private void UpdateHealthClientRpc(int currentHP, int maxHP, bool isHost) {
		if (isHost) {
			hostHUD.SetHP(currentHP);
		} else {
			clientHUD.SetHP(currentHP);
		}
	}

	[ServerRpc(RequireOwnership = false)]
	public void UpdateStatChangesServerRpc(bool isHost, int attackStage, int defenseStage) {
		if (isHost) {
			hostUnit.attackStage = attackStage;
			hostUnit.defenseStage = defenseStage;
			UpdateStatChangesClientRpc(attackStage, defenseStage, isHost);
		} else {
			clientUnit.attackStage = attackStage;
			clientUnit.defenseStage = defenseStage;
			UpdateStatChangesClientRpc(attackStage, defenseStage, isHost);
		}
	}

	[ClientRpc]
	private void UpdateStatChangesClientRpc(int attackStage, int defenseStage, bool isHost) {
		if (isHost) {
			hostHUD.SetStatChange(hostUnit);
		} else {
			clientHUD.SetStatChange(clientUnit);
		}
	}

	[ServerRpc(RequireOwnership = false)]
	public void ToggleCooldownServerRpc(bool isHost, bool on) {
		if (on) {
			CooldownOnClientRpc(isHost);
		} else {
			CooldownOffClientRpc(isHost);
		}
	}

	[ClientRpc]
	private void CooldownOnClientRpc(bool isHost) {
		if (isHost) {
			hostHUD.CooldownOn();
		} else {
			clientHUD.CooldownOn();
		}
	}

	[ClientRpc]
	private void CooldownOffClientRpc(bool isHost) {
		if (isHost) {
			hostHUD.CooldownOff();
		} else {
			clientHUD.CooldownOff();
		}
	}

	private void OnMoveButtonClicked(int moveIndex) {
		if (IsHost) {
			PerformMoveServerRpc(moveIndex, true);
		} else {
			PerformMoveServerRpc(moveIndex, false);
		}
	}

	// Move handling implementation
	[ServerRpc(RequireOwnership = false)]
	public void PerformMoveServerRpc(int moveIndex, bool isHost, ServerRpcParams rpcParams = default) {
		Unit attacker = isHost ? hostUnit : clientUnit;
		Unit defender = isHost ? clientUnit : hostUnit;

		// Apply move logic here
		Move move = attacker.GetMove(moveIndex);
		if (move.isCooldown) return; // If the move is on cooldown, do nothing

		int damage = CalculateDamage(attacker, defender, move);

		ApplyDamageServerRpc(!isHost, damage);
		ToggleCooldownServerRpc(isHost, true);

		// Update HUD
		UpdateHealthClientRpc(defender.currentHP, defender.maxHP, !isHost);
		UpdateStatChangesServerRpc(isHost, attacker.attackStage, attacker.defenseStage);
		UpdateStatChangesServerRpc(!isHost, defender.attackStage, defender.defenseStage);

		StartCoroutine(CooldownCoroutine(isHost, move));
	}

	private int CalculateDamage(Unit attacker, Unit defender, Move move) {
		// Simplified damage calculation logic
		return (attacker.attack * move.damage) / defender.defense;
	}

	private IEnumerator CooldownCoroutine(bool isHost, Move move) {
		move.isCooldown = true;
		yield return new WaitForSeconds(1f);
		move.isCooldown = false;
		ToggleCooldownServerRpc(isHost, false);
	}
}
