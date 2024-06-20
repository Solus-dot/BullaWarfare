using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerSelectionButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler {
	public GameObject characterPrefab; // Assign the character prefab in the Inspector
	public CharacterSelectManager characterSelectManager; // Reference to your CharacterSelectManager

	private void Start() {
		// Ensure that the CharacterSelectManager reference is set
		if (characterSelectManager == null) {
			characterSelectManager = CharacterSelectManager.Instance;
			if (characterSelectManager == null) {
				Debug.LogError("CharacterSelectManager reference not found.");
			}
		}
	}

	// This method is called when the pointer enters the button
	public void OnPointerEnter(PointerEventData eventData) {
		Debug.Log("Hovering over " + characterPrefab.name);
	}

	// This method is called when the pointer exits the button
	public void OnPointerExit(PointerEventData eventData) {
		Debug.Log("Exiting hover for " + characterPrefab.name);
	}

	// This method is called when the button is clicked
	public void OnPointerClick(PointerEventData eventData) {
		// Call the SelectCharacter method in the CharacterSelectManager when clicked
		if (characterSelectManager != null) {
			characterSelectManager.SelectCharacter(characterPrefab);
		}
	}
}
