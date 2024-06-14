using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour {
	public static CharacterSelectManager Instance;

	public TMP_Text titleText;
	public GameObject player1DetailsPanel;
	public GameObject player2DetailsPanel;

	//Names of the characters hovered on
	public TMP_Text Player1Name;
	public TMP_Text Player2Name;

	//Stats of the characters hovered on
	public TMP_Text Player1Stats;
	public TMP_Text Player2Stats;

	//Moves of the characters hovered on
	public TMP_Text Player1Moves;
	public TMP_Text Player2Moves;

	//the game objects that show up when hovering/selecting
	private GameObject selectedCharacterPlayer1;
	private GameObject selectedCharacterPlayer2;

	//the game objects that show up when confirming/selected
	private GameObject confirmedCharacterPlayer1;
	private GameObject confirmedCharacterPlayer2;

	private bool p1Picked; private bool p2Picked;

	private Dictionary<int, GameObject> selectedCharacterPrefabs = new Dictionary<int, GameObject>();

	private void Awake() {
		if (Instance == null) {
			Instance = this;
		}
		else {
			Destroy(gameObject);
		}
	}

	void Start() {
		p1Picked = false; p2Picked = false;

		titleText.text = "Player 1: Pick your Character!";
		player1DetailsPanel.SetActive(false);
		player2DetailsPanel.SetActive(false);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space)) {
			if (confirmedCharacterPlayer1 != null && confirmedCharacterPlayer2 != null)
			{
				LoadNextScene();
			}
			
		}

		if (Input.GetKeyDown(KeyCode.F)) {
			ResetCharacterSelection();
		}
	}

	public void OnButtonHover(GameObject characterPrefab) {
		GameObject prefab = LoadCharacterPrefab(characterPrefab);
		
		if (selectedCharacterPlayer1 == null && confirmedCharacterPlayer1 == null) {
			selectedCharacterPlayer1 = Instantiate(prefab, player1DetailsPanel.transform);
			player1DetailsPanel.SetActive(true);
			ShowStats(prefab, Player1Name, Player1Stats, Player1Moves);
		}
		else if (selectedCharacterPlayer2 == null && confirmedCharacterPlayer2 == null) {
			selectedCharacterPlayer2 = Instantiate(prefab, player2DetailsPanel.transform);
			player2DetailsPanel.SetActive(true);
			ShowStats(prefab, Player2Name, Player2Stats, Player2Moves);
		}
	}

	public void OnButtonDehover(GameObject characterPrefab) {
	Unit unit = characterPrefab.GetComponent<Unit>();
	if (selectedCharacterPlayer1 != null && !p1Picked && selectedCharacterPlayer1.GetComponent<Unit>().unitName == unit.unitName) {
		player1DetailsPanel.SetActive(false);
		Destroy(selectedCharacterPlayer1);
		selectedCharacterPlayer1 = null;
	} else if (selectedCharacterPlayer2 != null && !p2Picked && selectedCharacterPlayer2.GetComponent<Unit>().unitName == unit.unitName) {
		player2DetailsPanel.SetActive(false);
		Destroy(selectedCharacterPlayer2);
		selectedCharacterPlayer2 = null;
	}
	}


	public void SelectCharacter(string characterName, GameObject characterPrefab) {
		GameObject prefab = LoadCharacterPrefab(characterPrefab);

		if (confirmedCharacterPlayer1 == null) {
			confirmedCharacterPlayer1 = Instantiate(prefab, player1DetailsPanel.transform);

			player1DetailsPanel.SetActive(true);
			selectedCharacterPrefabs[1] = prefab;
			p1Picked = true;

			titleText.text = "Player 2: Pick your Character!";
			Debug.Log("Player 1 clicked on " + characterName);
		}
		else if (confirmedCharacterPlayer2 == null) {
			confirmedCharacterPlayer2 = Instantiate(prefab, player2DetailsPanel.transform);

			player2DetailsPanel.SetActive(true);
			selectedCharacterPrefabs[2] = prefab;
			p2Picked = true;

			titleText.text = "Press Space to confirm your selections!";
			Debug.Log("Player 2 clicked on " + characterName);
		}
	}

	private GameObject LoadCharacterPrefab(GameObject characterPrefab) {
		return characterPrefab;
	}

	void LoadNextScene() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	void ShowStats(GameObject prefab, TMP_Text NameText, TMP_Text StatsText, TMP_Text MovesText) {
		Unit unit = prefab.GetComponent<Unit>();
		NameText.text = unit.unitName;
		StatsText.text = "Level:\t" + unit.unitLevel + "\nHP:\t" + unit.currentHP + "\nAttack:\t" + unit.attack + "\nDefense:\t" + unit.defense;
		MovesText.text = unit.GetMove(0).moveName + "\n" + unit.GetMove(1).moveName + "\n" +  unit.GetMove(2).moveName + "\n" + unit.GetMove(3).moveName;
	}

	void HideStats(TMP_Text StatsText) {
		StatsText.text = "";
	}

	void ResetCharacterSelection() {
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

		// Reset the UI elements
		titleText.text = "Player 1: Pick your Character!";
		player1DetailsPanel.SetActive(false);
		player2DetailsPanel.SetActive(false);
	}

	public GameObject GetSelectedCharacterPrefab(int playerIndex) {
		if (selectedCharacterPrefabs.ContainsKey(playerIndex)) {
			return selectedCharacterPrefabs[playerIndex];
		}
		return null;
	}
}