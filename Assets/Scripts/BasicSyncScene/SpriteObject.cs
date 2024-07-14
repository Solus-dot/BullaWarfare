using UnityEngine;
using Unity.Netcode;

public class SpriteObject : NetworkBehaviour {
	private SpriteRenderer spriteRenderer;

	private void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void Initialize(Color color) {
		SetColor(color);
	}

	public void SetColor(Color color) {
		spriteRenderer.color = color;
	}
}
