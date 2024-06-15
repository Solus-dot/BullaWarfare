using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MovesetEnum { SOHOM, RAVI, MANAS, HARSH }

[System.Serializable]
public class Move {
	public string moveName;			// Name of the move
	public string moveDesc;			// Description of the move in character selection
	public string moveMessage;		// Message to be printed on the dialogue when the move is used
	public string missMessage;		// Message to be printed on the dialogue when the move missed

	public float cooldown;			// Time (turns) before reuse (optional)
	public int accuracy;			// Percentage chance of hitting (optional)

	public bool isDamaging;			// Flag indicating if the move deals damage
	public int damage;				// Base damage dealt by the move

	public bool isHealingMove;		// Flag indicating if the move heals (optional)
	public int healAmount;			// Amount of health healed (if isHealingMove is true)

	public bool isStatChange;		// Flag indicating if the move changes stats
	public int selfAttackChange;	// Amount by which the self attack stat changes (positive for increase, negative for decrease)
	public int selfDefenseChange;	// Amount by which the self defense stat changes (positive for increase, negative for decrease)
	public int oppAttackChange;		// Amount by which the opponent attack stat changes (positive for increase, negative for decrease)
	public int oppDefenseChange;	// Amount by which the opponent defense stat changes (positive for increase, negative for decrease)
}

public class Unit : MonoBehaviour {
	public string unitName;
	public int unitLevel;
	public int maxHP;
	public int attack;
	public int defense;
	public int baseAccuracy = 100;

	public int attackStage = 0;
	public int defenseStage = 0;
	public int currentHP;
	public MovesetEnum moveset;

	public int recvEffectiveDamage; // Effective damage received
	private Renderer unitRenderer;
	private Color originalColor;
	private float flashDuration = 0.2f; 

	void Start() {
		unitRenderer = GetComponent<SpriteRenderer>();
		if (unitRenderer != null) {
			originalColor = unitRenderer.material.color;
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

	public void Heal(int percent) {
		currentHP += percent*maxHP;
		if (currentHP > maxHP) {
			currentHP = maxHP;
		}
	}

	public void TakeBuff(int attackChange, int defenseChange) {
		attackStage += attackChange;
		defenseStage += defenseChange;

		attack += 10*attackChange;
		defense += 5*defenseChange;
	}

	public Move GetMove(int index) {
		if (moveset == MovesetEnum.SOHOM) {
			return Sohom.moves[index];
		} else if (moveset == MovesetEnum.RAVI) {
			return Ravi.moves[index];
		} else if (moveset == MovesetEnum.MANAS) {
			return Manas.moves[index];
		} else if (moveset == MovesetEnum.HARSH) {
			return Harsh.moves[index];
		} else {
			Debug.LogError("Moveset script is not assigned. This error from Unit.GetMove");
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
