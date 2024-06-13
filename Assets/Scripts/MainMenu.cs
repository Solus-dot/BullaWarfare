using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
	public void OnPlayButtonClick() {
		Debug.Log("Start Game button clicked");
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void OnExitButtonClick() {
		Debug.Log("Exit button clicked");
		Application.Quit();
	}
}
