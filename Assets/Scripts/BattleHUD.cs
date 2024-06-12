	using System.Collections;
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;

	public class BattleHUD : MonoBehaviour
	{
	public TMP_Text nameText;			// Reference to the text displaying unit's name
	public TMP_Text levelText;			// Reference to the text displaying unit's level
	public TMP_Text buffText;			// Reference to the text displaying unit's current buff/debuff status
	public Slider hpSlider;				// Reference to the slider representing unit's HP
	public Gradient hpGradient;			// Gradient for HP bar color

	public void SetHUD(Unit unit)
	{
		nameText.text = unit.unitName;				// Update the name text with unit's name
		levelText.text = "Lvl " + unit.unitLevel;	// Update the level text with unit's level
		buffText.text = "Atk+:" + unit.attackStage + "\nDef+:" + unit.defenseStage;			// Update the buff text with unit's buffs

		hpSlider.maxValue = unit.maxHP;		// Set max value of HP slider to unit's max HP
		hpSlider.value = unit.currentHP;	// Set current value of HP slider to unit's current HP

		// Update HP bar color based on HP percentage
		UpdateHPColor();
	}

	public void SetStatChange(Unit unit) {
		buffText.text = "Atk+:" + unit.attackStage + "\nDef+:" + unit.defenseStage;
	}

	public void SetHP(int hp)
	{
		StartCoroutine(SlideHP(hp));	// Start coroutine to animate HP slider
	}

	IEnumerator SlideHP(int newHP)
	{
		float startHP = hpSlider.value;
		float endHP = newHP;
		float duration = 0.7f;			// Duration of the sliding effect
		int steps = 4;					// Number of steps for the sliding effect

		float stepAmount = (endHP - startHP) / steps;

		// Animate HP slider over multiple steps
		for (int i = 0; i < steps; i++)
		{
			hpSlider.value += stepAmount;  // Increment HP slider value
			UpdateHPColor();               // Update HP bar color
			yield return new WaitForSeconds(duration / steps);  // Wait for the next step
		}

		// Ensure the slider value reaches the exact target value
		hpSlider.value = endHP;
		UpdateHPColor();
	}

	void UpdateHPColor()
	{
		float fillAmount = hpSlider.value / hpSlider.maxValue;  // Calculate HP percentage
		hpSlider.fillRect.GetComponent<Image>().color = hpGradient.Evaluate(fillAmount);  // Apply gradient color based on HP percentage
	}
	}
