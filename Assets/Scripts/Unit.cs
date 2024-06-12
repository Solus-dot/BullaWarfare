using UnityEngine;
using System.Collections;

[System.Serializable]
public class Move {
	public string moveName;      // Name of the move
	public string moveDesc;      // Description of the move in character selection
	public string moveMessage;   // Message to be printed on the dialogue when the move is used

	public bool isDamaging;      // Flag indicating if the move deals damage
	public int damage;           // Base damage dealt by the move
	public float cooldown;       // Time (turns) before reuse (optional)
	public int accuracy;         // Percentage chance of hitting (optional)

	public bool isHealingMove;   // Flag indicating if the move heals (optional)
	public int healAmount;       // Amount of health healed (if isHealingMove is true)

	public bool isStatChange;    // Flag indicating if the move changes stats
	public int attackChange;     // Amount by which the attack stat changes (positive for increase, negative for decrease)
	public int defenseChange;    // Amount by which the defense stat changes (positive for increase, negative for decrease)
}

public class Unit : MonoBehaviour {
    public string unitName;
    public int unitLevel;
	public int maxHP;
	public int attack;
    public int defense;
    public int currentHP;

    public int recvEffectiveDamage; // Effective damage received
    private Renderer unitRenderer;
    private Color originalColor;
    private float flashDuration = 0.2f; 
    private Sohom sohom; // Reference to the Sohom script
    private Ravi ravi; // Reference to the Ravi script

    void Start() {
        unitRenderer = GetComponent<SpriteRenderer>();
        if (unitRenderer != null) {
            originalColor = unitRenderer.material.color;
        }

        // Get the Sohom or Ravi script component from the same GameObject
        sohom = GetComponent<Sohom>();
        ravi = GetComponent<Ravi>();
        if (sohom == null && ravi == null) {
            Debug.LogError("Sohom or Ravi script not found on the unit prefab.");
            return;
        }
    }

    public bool TakeDamage(int dmg) {
        // Calculate effective damage after defense reduction
        recvEffectiveDamage = Mathf.Max(0, dmg - defense);
        currentHP -= recvEffectiveDamage;
        if (unitRenderer != null) {
            StartCoroutine(FlashRed());
        }
        if (currentHP <= 0) {
            currentHP = 0;
            return true;
        }
        return false;
    }

    public void Heal(int amount) {
        currentHP += amount;
        if (currentHP > maxHP) {
            currentHP = maxHP;
        }
    }

    public Move GetMove(int index) {
        if (sohom != null) {
            return sohom.GetMoveAt(index);
        } else if (ravi != null) {
            return ravi.GetMoveAt(index);
        } else {
            Debug.LogError("Moveset script is not assigned.");
            return null;
        }
    }

    private IEnumerator FlashRed() {
        unitRenderer.material.color = Color.red;
        yield return new WaitForSeconds(flashDuration); // Use customizable duration
        unitRenderer.material.color = originalColor;
        yield return new WaitForSeconds(flashDuration);
        unitRenderer.material.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        unitRenderer.material.color = originalColor;
    }
}
