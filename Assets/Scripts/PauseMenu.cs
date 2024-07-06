using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {
	public GameObject tintPanel;
	public GameObject pauseMenuPanel;
	public Slider volumeSlider;

	private bool isPaused;

	void Start() {
		isPaused = false;
		tintPanel.SetActive(false);
		pauseMenuPanel.SetActive(false);

		if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(delegate { SnapSliderValue(); });
            volumeSlider.value = PlayerPrefs.GetFloat("Volume", 50f); // Initialize the value from PlayerPrefs
            SnapSliderValue(); // Initialize the value to the nearest multiple of 10
        }

	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (isPaused) {
				Resume();
			}
			else {
				Pause();
			}
		}
	}

	public void Resume() {
		tintPanel.SetActive(false);
		pauseMenuPanel.SetActive(false);
		Time.timeScale = 1f;
		isPaused = false;
	}

	void Pause() {
		tintPanel.SetActive(true);
		pauseMenuPanel.SetActive(true);
		Time.timeScale = 0f;
		isPaused = true;
	}

	public void LoadMainMenu() {
		Time.timeScale = 1f;
		SceneManager.LoadScene("MainMenuScene");
	}

	public void RestartBattle() {
		BattleSystem battleSystem = FindObjectOfType<BattleSystem>();
		if (battleSystem != null) {
			battleSystem.Start();
		} else {
			Debug.LogError("BattleSystem not assigned.");
		}

		Resume();
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
}
