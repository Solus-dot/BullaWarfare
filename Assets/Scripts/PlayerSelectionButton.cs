using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerSelectionButton : MonoBehaviour, IPointerClickHandler {
	public GameObject characterPrefab; // Assign the character prefab in the Inspector
	public CharacterSelectManager characterSelectManager; // Reference to your CharacterSelectManager

	// Frame images
	public Image frameImage;
	public Sprite defaultFrame;
	public Sprite P1Frame;
	public Sprite P2Frame;
	public Sprite P1P2Frame;

	private void Start() {
		// Ensure that the CharacterSelectManager reference is set
		if (characterSelectManager == null) {
			characterSelectManager = CharacterSelectManager.Instance;
			if (characterSelectManager == null) {
				Debug.LogError("CharacterSelectManager reference not found.");
			}
		}

		// Set default frame
		frameImage.sprite = defaultFrame;
	}

	public void OnPointerClick(PointerEventData eventData) {
		// Call the SelectCharacter method in the CharacterSelectManager when clicked
		if (characterSelectManager != null) {
			characterSelectManager.SelectCharacter(characterPrefab);
		}
	}

	public void SetFrameImage(CharacterSelectionState state) {
		switch (state) {
			case CharacterSelectionState.Default:
				frameImage.sprite = defaultFrame;
				break;
			case CharacterSelectionState.P1Selected:
				frameImage.sprite = P1Frame;
				break;
			case CharacterSelectionState.P2Selected:
				frameImage.sprite = P2Frame;
				break;
			case CharacterSelectionState.P1P2Selected:
				frameImage.sprite = P1P2Frame;
				break;
			default:
				break;
		}
	}
}
