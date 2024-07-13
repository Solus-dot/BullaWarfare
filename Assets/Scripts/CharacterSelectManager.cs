using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public enum CharacterSelectionState { Default, P1Selected, P2Selected, P1P2Selected }

public class CharacterSelectManager : NetworkBehaviour {
	public static CharacterSelectManager Instance;

	public TMP_Text titleText;
	public GameObject CharacterSelectParent;
	public GameObject BackgroundSelectParent;

	[Header("Character Select")]
	[Header("PlayerPanel")]
	public GameObject player1DetailsPanel;
	public GameObject player2DetailsPanel;

	// Names of the characters hovered on
	public TMP_Text P1Name;
	public TMP_Text P2Name;

	// Stats of the characters hovered on
	public TMP_Text P1Stats;
	public TMP_Text P2Stats;

	// Moves of the characters hovered on
	public TMP_Text P1MoveName;
	public TMP_Text P2MoveName;

	// Descs of the characters hovered on
	public TMP_Text P1MoveDesc;
	public TMP_Text P2MoveDesc;

	// Move switch Buttons
	public Button P1Left;
	public Button P1Right;
	public Button P2Left;
	public Button P2Right;

	// The game objects that show up when hovering/selecting
	private GameObject selectedCharacterPlayer1;
	private GameObject selectedCharacterPlayer2;

	// The game objects that show up when confirming/selected
	private GameObject confirmedCharacterPlayer1;
	private GameObject confirmedCharacterPlayer2;

	private Dictionary<int, GameObject> selectedCharacterPrefabs = new Dictionary<int, GameObject>();

	// Move indexes for cycling through moves
	private int p1MoveIndex = 0;
	private int p2MoveIndex = 0;

	// Last clicked characters
	private GameObject lastClickedCharacterPlayer1;
	private GameObject lastClickedCharacterPlayer2;

	// List of player selection buttons
	private List<PlayerSelectionButton> playerButtons = new List<PlayerSelectionButton>();

	// Network variables for character selection indices
	public NetworkVariable<int> confirmedCharacterIndexPlayer1 = new NetworkVariable<int>(-1);
	public NetworkVariable<int> confirmedCharacterIndexPlayer2 = new NetworkVariable<int>(-1);

	private void Awake() {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}
	}

	void Start() {
		titleText.text = "Player 1: Pick your Character!";
		player1DetailsPanel.SetActive(false);
		player2DetailsPanel.SetActive(false);

		P1Left.onClick.AddListener(() => CycleMove(-1, 1));
		P1Right.onClick.AddListener(() => CycleMove(1, 1));
		P2Left.onClick.AddListener(() => CycleMove(-1, 2));
		P2Right.onClick.AddListener(() => CycleMove(1, 2));

		// Find all player selection buttons
		playerButtons.AddRange(FindObjectsOfType<PlayerSelectionButton>());

		confirmedCharacterIndexPlayer1.OnValueChanged += OnPlayer1CharacterChanged;
		confirmedCharacterIndexPlayer2.OnValueChanged += OnPlayer2CharacterChanged;
	}

	void Update() {
		if (!IsClient) return;

		if (Input.GetKeyDown(KeyCode.Space)) {
			if (confirmedCharacterIndexPlayer1.Value != -1 && confirmedCharacterIndexPlayer2.Value != -1) {
				SceneManager.LoadScene("BattleScene");
			}
		}

		if (Input.GetKeyDown(KeyCode.F)) {
			ResetCharacterSelection();
		}
	}

	public void SelectCharacter(GameObject characterPrefab, int characterIndex) {
		if (!IsClient) return;

		Debug.Log($"SelectCharacter called with prefab: {characterPrefab.name}, index: {characterIndex}");

		if (IsOwner && confirmedCharacterIndexPlayer1.Value == -1) {
			HandlePlayer1Selection(characterPrefab, characterIndex);
		} else if (IsOwner && confirmedCharacterIndexPlayer2.Value == -1) {
			HandlePlayer2Selection(characterPrefab, characterIndex);
		}
	}

	private void HandlePlayer1Selection(GameObject characterPrefab, int characterIndex) {
		if (selectedCharacterPlayer1 == null || lastClickedCharacterPlayer1 != characterPrefab) {
			// First click or different character
			if (selectedCharacterPlayer1 != null) {
				Destroy(selectedCharacterPlayer1);
			}

			selectedCharacterPlayer1 = InstantiateCharacter(characterPrefab, characterIndex);
			player1DetailsPanel.SetActive(true);
			ShowStats(selectedCharacterPlayer1, P1Name, P1Stats);
			ShowMoveInfo(selectedCharacterPlayer1, P1MoveName, P1MoveDesc, p1MoveIndex);

			lastClickedCharacterPlayer1 = characterPrefab;
		} else {
			// Second click on the same character
			confirmedCharacterIndexPlayer1.Value = characterIndex;
			SetButtonFrame(characterPrefab, CharacterSelectionState.P1Selected);
			titleText.text = "Player 2: Pick your Character!";
		}
	}

	private void HandlePlayer2Selection(GameObject characterPrefab, int characterIndex) {
		if (selectedCharacterPlayer2 == null || lastClickedCharacterPlayer2 != characterPrefab) {
			// First click or different character
			if (selectedCharacterPlayer2 != null) {
				Destroy(selectedCharacterPlayer2);
			}

			selectedCharacterPlayer2 = InstantiateCharacter(characterPrefab, characterIndex);
			player2DetailsPanel.SetActive(true);
			ShowStats(selectedCharacterPlayer2, P2Name, P2Stats);
			ShowMoveInfo(selectedCharacterPlayer2, P2MoveName, P2MoveDesc, p2MoveIndex);

			lastClickedCharacterPlayer2 = characterPrefab;
		} else {
			// Second click on the same character
			confirmedCharacterIndexPlayer2.Value = characterIndex;
			if (confirmedCharacterIndexPlayer1.Value == confirmedCharacterIndexPlayer2.Value) {
				SetButtonFrame(characterPrefab, CharacterSelectionState.P1P2Selected);
			} else {
				SetButtonFrame(characterPrefab, CharacterSelectionState.P2Selected);
			}
			titleText.text = "Press Space to confirm your selections!";
		}
	}

	void SetButtonFrame(GameObject characterPrefab, CharacterSelectionState state) {
		foreach (var button in playerButtons) {
			if (button.characterPrefab == characterPrefab) {
				button.SetFrameImage(state);
				break;
			}
		}
	}

	void ShowStats(GameObject prefab, TMP_Text NameText, TMP_Text StatsText) {
		Unit unit = prefab.GetComponent<Unit>();
		NameText.text = unit.unitName;
		StatsText.text = $"HP: {unit.currentHP}\nAttack: {unit.attack}\nDefense: {unit.defense}";
	}

	void ShowMoveInfo(GameObject prefab, TMP_Text MoveNameText, TMP_Text MoveDescText, int MoveIndex) {
		Unit unit = prefab.GetComponent<Unit>();
		Move move = unit.GetMove(MoveIndex);
		MoveNameText.text = move.moveName;
		MoveDescText.text = move.moveDesc;
	}

	void ResetCharacterSelection() {
		if (selectedCharacterPlayer1 != null) {
			Destroy(selectedCharacterPlayer1);
			selectedCharacterPlayer1 = null;
		}

		if (selectedCharacterPlayer2 != null) {
			Destroy(selectedCharacterPlayer2);
			selectedCharacterPlayer2 = null;
		}

		if (confirmedCharacterIndexPlayer1.Value != -1) {
			confirmedCharacterIndexPlayer1.Value = -1;
		}
		if (confirmedCharacterIndexPlayer2.Value != -1) {
			confirmedCharacterIndexPlayer2.Value = -1;
		}

		p1MoveIndex = 0;
		p2MoveIndex = 0;

		lastClickedCharacterPlayer1 = null;
		lastClickedCharacterPlayer2 = null;

		titleText.text = "Player 1: Pick your Character!";
		player1DetailsPanel.SetActive(false);
		player2DetailsPanel.SetActive(false);

		foreach (var button in playerButtons) {
			button.SetFrameImage(CharacterSelectionState.Default);
		}
	}

	void CycleMove(int direction, int playerIndex) {
		if (playerIndex == 1 && selectedCharacterPlayer1 != null) {
			p1MoveIndex = (p1MoveIndex + direction + 4) % 4;
			ShowMoveInfo(selectedCharacterPlayer1, P1MoveName, P1MoveDesc, p1MoveIndex);
		} else if (playerIndex == 2 && selectedCharacterPlayer2 != null) {
			p2MoveIndex = (p2MoveIndex + direction + 4) % 4;
			ShowMoveInfo(selectedCharacterPlayer2, P2MoveName, P2MoveDesc, p2MoveIndex);
		}
	}

	private void OnPlayer1CharacterChanged(int oldIndex, int newIndex) {
		if (newIndex != -1) {
			foreach (var button in playerButtons) {
				if (button.characterIndex == newIndex) {
					if (confirmedCharacterIndexPlayer2.Value == newIndex) {
						button.SetFrameImage(CharacterSelectionState.P1P2Selected);
					} else {
						button.SetFrameImage(CharacterSelectionState.P1Selected);
					}
				} else if (confirmedCharacterIndexPlayer2.Value == button.characterIndex) {
					button.SetFrameImage(CharacterSelectionState.P2Selected);
				} else {
					button.SetFrameImage(CharacterSelectionState.Default);
				}
			}
		}
	}

	private void OnPlayer2CharacterChanged(int oldIndex, int newIndex) {
		if (newIndex != -1) {
			foreach (var button in playerButtons) {
				if (button.characterIndex == newIndex) {
					if (confirmedCharacterIndexPlayer1.Value == newIndex) {
						button.SetFrameImage(CharacterSelectionState.P1P2Selected);
					} else {
						button.SetFrameImage(CharacterSelectionState.P2Selected);
					}
				} else if (confirmedCharacterIndexPlayer1.Value == button.characterIndex) {
					button.SetFrameImage(CharacterSelectionState.P1Selected);
				} else {
					button.SetFrameImage(CharacterSelectionState.Default);
				}
			}
		}
	}

	public override void OnNetworkSpawn() {
		if (IsClient && !IsOwner) {
			enabled = false;
		}
	}

	private GameObject InstantiateCharacter(GameObject characterPrefab, int characterIndex) {
		var character = Instantiate(characterPrefab);
		character.GetComponent<NetworkObject>().Spawn();
		selectedCharacterPrefabs[characterIndex] = character;
		return character;
	}

	public GameObject GetSelectedCharacterPrefab(int playerIndex) {
		if (playerIndex == 1) {
			return confirmedCharacterPlayer1;
		} else if (playerIndex == 2) {
			return confirmedCharacterPlayer2;
		}
		return null;
	}
}
