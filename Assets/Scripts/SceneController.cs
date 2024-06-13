using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
	void Start()
	{
		// Get the name of the currently active scene
		string currentSceneName = SceneManager.GetActiveScene().name;

		// Print the scene name to the console
		Debug.Log("Currently active scene: " + currentSceneName);

		// You can also perform actions based on the scene name
		if (currentSceneName == "MainMenuScene")
		{
			// Do something specific for the main menu scene
			Debug.Log("This is the Main Menu scene.");
		}
		else if (currentSceneName == "BattleScene")
		{
			// Do something specific for the battle scene
			Debug.Log("This is the Battle scene.");
		}
		// Add more conditions as needed for other scenes
	}
}