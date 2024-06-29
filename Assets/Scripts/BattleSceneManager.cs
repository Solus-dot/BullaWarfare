using UnityEngine;

public class BattleSceneManager : MonoBehaviour {
	public SpriteRenderer backgroundRenderer;
	public Sprite[] availableBackgrounds;

	void Start() {
		// Retrieve the selected background index
		int selectedBackgroundIndex = PlayerPrefs.GetInt("SelectedBackgroundIndex", 0);

		// Ensure the index is within bounds
		if (selectedBackgroundIndex >= 0 && selectedBackgroundIndex < availableBackgrounds.Length) {
			// Set the background sprite
			backgroundRenderer.sprite = availableBackgrounds[selectedBackgroundIndex];
		} else {
			Debug.LogWarning("Selected background index is out of bounds.");
		}
	}
}