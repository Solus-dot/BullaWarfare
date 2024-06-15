using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	public Slider volumeSlider;
	public AudioSource audioSource;

	void Start()
	{
		if (volumeSlider != null)
		{
			volumeSlider.onValueChanged.AddListener(delegate { SnapSliderValue(); });
			SnapSliderValue(); // Initialize the value to the nearest multiple of 10
		}
	}

	void SnapSliderValue()
	{
		// Snap the slider value to the nearest multiple of 10
		float snappedValue = Mathf.Round(volumeSlider.value / 10) * 10;
		volumeSlider.value = snappedValue;

		// Update the audio source volume
		if (audioSource != null)
		{
			audioSource.volume = snappedValue / 100f; // Assuming the slider range is from 0 to 100
		}
	}

	public void OnPlayButtonClick()
	{
		Debug.Log("Start Game button clicked");
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void OnExitButtonClick()
	{
		Debug.Log("Exit button clicked");
		Application.Quit();
	}
}
