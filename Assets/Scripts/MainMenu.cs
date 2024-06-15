using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    public Slider volumeSlider;     // Reference to the UI slider
    public AudioSource audioSource; // Reference to the audio source

    void Start() {
        if (volumeSlider != null && audioSource != null) {
            // Set the slider's max and min values if needed
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            // Set the slider's value to the current volume of the audio source
            volumeSlider.value = audioSource.volume;

            // Add listener for when the slider value changes
            volumeSlider.onValueChanged.AddListener(OnVolumeSliderValueChanged);
        }
    }

    public void OnPlayButtonClick() {
        Debug.Log("Start Game button clicked");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OnExitButtonClick() {
        Debug.Log("Exit button clicked");
        Application.Quit();
    }

    void OnVolumeSliderValueChanged(float value) {
        if (audioSource != null) {
            audioSource.volume = value;
        }
    }
}
