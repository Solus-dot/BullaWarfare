using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectionButton : MonoBehaviour {
	public GameObject characterPrefab;
	public int characterIndex;
	public CharacterSelectManager characterSelectManager;

	private void Start() {
		Button button = GetComponent<Button>();
		button.onClick.AddListener(OnButtonClick);

		if (characterSelectManager == null) {
			characterSelectManager = CharacterSelectManager.Instance;
		}
	}

	private void OnButtonClick() {
		Debug.Log("Click Log #1");
		if (characterSelectManager != null) {
			characterSelectManager.SelectCharacter(characterPrefab, characterIndex);
			Debug.Log("Click Log #2");
		}
	}
}
