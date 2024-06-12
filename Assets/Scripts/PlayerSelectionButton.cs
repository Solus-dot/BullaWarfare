using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerSelectionButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler {
    public string characterName;
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
        // Show details of the character prefab when hovered over
        // You can implement your logic here, such as displaying a tooltip or enlarging the character image
        if (characterSelectManager != null) {
            characterSelectManager.OnButtonHover(characterPrefab);
        }
        Debug.Log("Hovering over " + characterName);
    }

    // This method is called when the pointer exits the button
    public void OnPointerExit(PointerEventData eventData) {
        if (characterSelectManager != null) {
            characterSelectManager.OnButtonDehover(characterPrefab);
        }
        Debug.Log("Exiting hover for " + characterName);
    }

    // This method is called when the button is clicked
    public void OnPointerClick(PointerEventData eventData) {
        // Call the SelectCharacter method in the CharacterSelectManager when clicked
        if (characterSelectManager != null) {
            characterSelectManager.SelectCharacter(characterName, characterPrefab);
        }
    }
}
