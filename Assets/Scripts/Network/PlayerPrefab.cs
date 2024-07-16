using UnityEngine;

public class PlayerPrefab : MonoBehaviour {
    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }
}