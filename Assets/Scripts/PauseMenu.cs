using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour {
	public GameObject tintPanel;
	public GameObject pauseMenuPanel;

	private bool isPaused;

	void Start() {
		isPaused = false;
		tintPanel.SetActive(false);
		pauseMenuPanel.SetActive(false);
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
		SceneManager.LoadScene(0);
	}
}
