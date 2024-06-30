using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
	public Slider volumeSlider;

	void Start() {
		if (volumeSlider != null) {
			volumeSlider.onValueChanged.AddListener(delegate { SnapSliderValue(); });
			volumeSlider.value = PlayerPrefs.GetFloat("Volume", 100f); // Initialize the value from PlayerPrefs
			SnapSliderValue(); // Initialize the value to the nearest multiple of 10
		}
	}

	void SnapSliderValue() {
		// Snap the slider value to the nearest multiple of 10
		float snappedValue = Mathf.Round(volumeSlider.value / 10) * 10;
		volumeSlider.value = snappedValue;

		// Update the audio source volume
		if (AudioManager.Instance != null) {
			AudioManager.Instance.SetVolume(snappedValue / 100f); // Assuming the slider range is from 0 to 100
		}

		// Save the volume setting
        PlayerPrefs.SetFloat("Volume", volumeSlider.value);
	}

	public void OnPlayButtonClick() {
		Debug.Log("Start Game button clicked");
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void OnExitButtonClick() {
		Debug.Log("Exit button clicked");
		Application.Quit();
	}
}
