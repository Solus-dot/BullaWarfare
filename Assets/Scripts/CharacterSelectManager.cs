using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum CharacterSelectionState { Default, P1Selected, P2Selected, P1P2Selected }

public class CharacterSelectManager : MonoBehaviour {
	public static CharacterSelectManager Instance;

	public TMP_Text titleText;
	public GameObject CharacterSelectParent;
	public GameObject BackgroundSelectParent;

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

	// Background selection UI
	public SpriteRenderer BGSpriteRenderer;
	public TMP_Text BGName;
	public Button BGLeftButton;
	public Button BGRightButton;
	public Button ConfirmBGButton;

	public Sprite[] Backgrounds; // Array of background sprites
	public string[] BackgroundNames; // Array of background names

	// The game objects that show up when hovering/selecting
	private GameObject selectedCharacterPlayer1;
	private GameObject selectedCharacterPlayer2;

	// The game objects that show up when confirming/selected
	private GameObject confirmedCharacterPlayer1;
	private GameObject confirmedCharacterPlayer2;

	private bool p1Picked;
	private bool p2Picked;

	private Dictionary<int, GameObject> selectedCharacterPrefabs = new Dictionary<int, GameObject>();

	// Move indexes for cycling through moves
	private int p1MoveIndex = 0;
	private int p2MoveIndex = 0;

	// Last clicked characters
	private GameObject lastClickedCharacterPlayer1;
	private GameObject lastClickedCharacterPlayer2;

	// List of player selection buttons
	private List<PlayerSelectionButton> playerButtons = new List<PlayerSelectionButton>();

	// Background selection variables
	public int currentBackgroundIndex = 0;

	private void Awake() {
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy(gameObject);
		}
	}

	void Start() {
		CharacterSelectParent.SetActive(true);
		BackgroundSelectParent.SetActive(false);

		p1Picked = false;
		p2Picked = false;

		titleText.text = "Player 1: Pick your Character!";
		player1DetailsPanel.SetActive(false);
		player2DetailsPanel.SetActive(false);

		P1Left.onClick.AddListener(() => CycleMove(-1, 1));
		P1Right.onClick.AddListener(() => CycleMove(1, 1));
		P2Left.onClick.AddListener(() => CycleMove(-1, 2));
		P2Right.onClick.AddListener(() => CycleMove(1, 2));

		// Find all player selection buttons
		playerButtons.AddRange(FindObjectsOfType<PlayerSelectionButton>());

		// Background selection listeners
		BGLeftButton.onClick.AddListener(OnLeftButtonClick);
		BGRightButton.onClick.AddListener(OnRightButtonClick);
		ConfirmBGButton.onClick.AddListener(OnConfirmBGButtonClick);

		// Initialize the background selection UI
		UpdateBackgroundUI();
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			if (confirmedCharacterPlayer1 != null && confirmedCharacterPlayer2 != null) {
				CharacterSelectParent.SetActive(false);
				BackgroundSelectParent.SetActive(true);
				titleText.text = "Select a Background";
			}
		}

		if (Input.GetKeyDown(KeyCode.F)) {
			ResetCharacterSelection();
		}
	}

	public void SelectCharacter(GameObject characterPrefab) {
		if (!p1Picked && confirmedCharacterPlayer1 == null) {
			HandlePlayer1Selection(characterPrefab);
		} else if (!p2Picked && confirmedCharacterPlayer2 == null) {
			HandlePlayer2Selection(characterPrefab);
		}
	}

	private void HandlePlayer1Selection(GameObject characterPrefab) {
		if (selectedCharacterPlayer1 == null || lastClickedCharacterPlayer1 != characterPrefab) {
			// First click or different character
			if (selectedCharacterPlayer1 != null) {
				Destroy(selectedCharacterPlayer1);
			}
			selectedCharacterPlayer1 = Instantiate(characterPrefab, player1DetailsPanel.transform);
			player1DetailsPanel.SetActive(true);
			ShowStats(selectedCharacterPlayer1, P1Name, P1Stats);
			ShowMoveInfo(selectedCharacterPlayer1, P1MoveName, P1MoveDesc, p1MoveIndex);

			lastClickedCharacterPlayer1 = characterPrefab;
		} else {
			// Second click on the same character
			confirmedCharacterPlayer1 = Instantiate(characterPrefab, player1DetailsPanel.transform);
			selectedCharacterPrefabs[1] = characterPrefab;
			p1Picked = true;
			P1Name.color = Color.cyan;

			Destroy(selectedCharacterPlayer1);
			selectedCharacterPlayer1 = null;
			lastClickedCharacterPlayer1 = null;

			SetButtonFrame(characterPrefab, CharacterSelectionState.P1Selected);
			titleText.text = "Player 2: Pick your Character!";
		}
	}

	private void HandlePlayer2Selection(GameObject characterPrefab) {
		if (selectedCharacterPlayer2 == null || lastClickedCharacterPlayer2 != characterPrefab) {
			// First click or different character
			if (selectedCharacterPlayer2 != null) {
				Destroy(selectedCharacterPlayer2);
			}
			selectedCharacterPlayer2 = Instantiate(characterPrefab, player2DetailsPanel.transform);
			player2DetailsPanel.SetActive(true);
			ShowStats(selectedCharacterPlayer2, P2Name, P2Stats);
			ShowMoveInfo(selectedCharacterPlayer2, P2MoveName, P2MoveDesc, p2MoveIndex);

			lastClickedCharacterPlayer2 = characterPrefab;
		} else {
			// Second click on the same character
			confirmedCharacterPlayer2 = Instantiate(characterPrefab, player2DetailsPanel.transform);
			selectedCharacterPrefabs[2] = characterPrefab;
			p2Picked = true;
			P2Name.color = Color.red;

			Destroy(selectedCharacterPlayer2);
			selectedCharacterPlayer2 = null;
			lastClickedCharacterPlayer2 = null;
			if (selectedCharacterPrefabs[1] == selectedCharacterPrefabs[2]) {
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
		StatsText.text = "Title" + "\nHP:\t" + unit.currentHP + "\nAttack:\t" + unit.attack + "\nDefense:\t" + unit.defense;
	}

	void ShowMoveInfo(GameObject prefab, TMP_Text MoveNameText, TMP_Text MoveDescText, int MoveIndex) {
		Unit unit = prefab.GetComponent<Unit>();
		Move move = unit.GetMove(MoveIndex);
		MoveNameText.text = move.moveName;
		MoveDescText.text = move.moveDesc;
	}

	void ResetCharacterSelection() {
		CharacterSelectParent.SetActive(true);
		BackgroundSelectParent.SetActive(false);
		
		// Destroy the selected character game objects
		if (selectedCharacterPlayer1 != null) {
			Destroy(selectedCharacterPlayer1);
			selectedCharacterPlayer1 = null;
		}

		if (selectedCharacterPlayer2 != null) {
			Destroy(selectedCharacterPlayer2);
			selectedCharacterPlayer2 = null;
		}

		// Destroy the confirmed character game objects
		if (confirmedCharacterPlayer1 != null) {
			Destroy(confirmedCharacterPlayer1);
			confirmedCharacterPlayer1 = null;
		}
		if (confirmedCharacterPlayer2 != null) {
			Destroy(confirmedCharacterPlayer2);
			confirmedCharacterPlayer2 = null;
		}

		// Reset the selected character prefabs dictionary
		selectedCharacterPrefabs.Clear();

		// Reset the picked flags
		p1Picked = false;
		p2Picked = false;

		// Reset the move indexes
		p1MoveIndex = 0;
		p2MoveIndex = 0;

		// Reset the last clicked characters
		lastClickedCharacterPlayer1 = null;
		lastClickedCharacterPlayer2 = null;

		// Reset the UI elements
		titleText.text = "Player 1: Pick your Character!";
		player1DetailsPanel.SetActive(false);
		player2DetailsPanel.SetActive(false);
		P1Name.color = Color.black;
		P2Name.color = Color.black;

		// Reset button frames
		foreach (var button in playerButtons) {
			button.SetFrameImage(CharacterSelectionState.Default);
		}
	}

	void CycleMove(int direction, int playerIndex) {
		if (playerIndex == 1 && (selectedCharacterPlayer1 != null || confirmedCharacterPlayer1 != null)) {
			Unit unit = (selectedCharacterPlayer1 != null ? selectedCharacterPlayer1 : confirmedCharacterPlayer1).GetComponent<Unit>();
			p1MoveIndex = (p1MoveIndex + direction + 4) % 4;
			ShowMoveInfo((selectedCharacterPlayer1 != null ? selectedCharacterPlayer1 : confirmedCharacterPlayer1), P1MoveName, P1MoveDesc, p1MoveIndex);
		} else if (playerIndex == 2 && (selectedCharacterPlayer2 != null || confirmedCharacterPlayer2 != null)) {
			Unit unit = (selectedCharacterPlayer2 != null ? selectedCharacterPlayer2 : confirmedCharacterPlayer2).GetComponent<Unit>();
			p2MoveIndex = (p2MoveIndex + direction + 4) % 4;
			ShowMoveInfo((selectedCharacterPlayer2 != null ? selectedCharacterPlayer2 : confirmedCharacterPlayer2), P2MoveName, P2MoveDesc, p2MoveIndex);
		}
	}

	void OnLeftButtonClick() {
		currentBackgroundIndex = (currentBackgroundIndex - 1 + Backgrounds.Length) % Backgrounds.Length;
		UpdateBackgroundUI();
	}

	void OnRightButtonClick() {
		currentBackgroundIndex = (currentBackgroundIndex + 1) % Backgrounds.Length;
		UpdateBackgroundUI();
	}

	void OnConfirmBGButtonClick() {
		// Save the selected background index to use in the next scene
		PlayerPrefs.SetInt("SelectedBackgroundIndex", currentBackgroundIndex);
		// Load the next scene
		SceneManager.LoadScene(3);
	}

	void UpdateBackgroundUI() {
		BGSpriteRenderer.sprite = Backgrounds[currentBackgroundIndex];
		BGName.text = BackgroundNames[currentBackgroundIndex];
	}

	public GameObject GetSelectedCharacterPrefab(int playerIndex) {
		if (selectedCharacterPrefabs.ContainsKey(playerIndex)) {
			return selectedCharacterPrefabs[playerIndex];
		}
		return null;
	}
}
