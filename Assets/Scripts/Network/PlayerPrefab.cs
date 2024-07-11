using UnityEngine;
using TMPro;

public class PlayerPrefab : MonoBehaviour {
    private void Awake() {
        DontDestroyOnLoad(gameObject);
    }
}
