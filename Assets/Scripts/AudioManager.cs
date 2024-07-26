using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour {
	public static AudioManager Instance { get; private set; }

	private AudioSource audioSource;

	[SerializeField] private AudioClip mainMenuMusic;
	[SerializeField] private AudioClip characterSelectMusic;
	[SerializeField] private AudioClip battleMusic;

	void Awake() {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(gameObject);
			audioSource = GetComponent<AudioSource>();
		} else {
			Destroy(gameObject);
		}
	}

	void OnEnable() {
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnDisable() {
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
		switch (scene.name) {
			case "MainMenuScene":
				ChangeMusic(mainMenuMusic);
				break;
			case "LobbyScene":
				ChangeMusic(mainMenuMusic);
				break;
			case "CharacterSelect":
				ChangeMusic(characterSelectMusic);
				break;
			case "MPCharacterSelect":
				ChangeMusic(characterSelectMusic);
				break;
			case "BattleScene":
				ChangeMusic(battleMusic);
				break;
			case "MPBattleScene":
				ChangeMusic(battleMusic);
				break;
		}
	}

	public void SetVolume(float volume) {
		if (audioSource != null) {
			audioSource.volume = volume;
		}
	}

	public void ChangeMusic(AudioClip newClip) {
		if (audioSource != null && newClip != null && audioSource.clip != newClip) {
			audioSource.clip = newClip;
			audioSource.Play();
		}
	}
}
