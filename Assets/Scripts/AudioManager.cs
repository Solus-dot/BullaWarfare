using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource audioSource;

    public AudioClip mainMenuMusic;
    public AudioClip characterSelectMusic;
    // Add more AudioClip references as needed for other scenes

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Prevent this object from being destroyed when loading a new scene
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject); // Ensures only one instance of the AudioManager exists
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.buildIndex)
        {
            case 0: // Main Menu Scene
                ChangeMusic(mainMenuMusic);
                break;
            case 2: // Character Select Scene
                ChangeMusic(characterSelectMusic);
                break;
            // Add more cases as needed for other scenes with corresponding build indices
        }
    }

    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }

    public void ChangeMusic(AudioClip newClip)
    {
        if (audioSource != null && newClip != null && audioSource.clip != newClip)
        {
            audioSource.clip = newClip;
            audioSource.Play();
        }
    }
}
