	using System.Collections;
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;

	public class BattleHUD : MonoBehaviour
	{
	public TMP_Text nameText;			// Reference to the text displaying unit's name
	public TMP_Text levelText;			// Reference to the text displaying unit's level
	public TMP_Text buffText;			// Reference to the text displaying unit's current buff/debuff status

	public SpriteRenderer AtkIcon;		// Reference to the icon used for attack stages
	public SpriteRenderer DefIcon;		// Reference to the icon used for defense stages

	public Sprite AttackUp;
	public Sprite AttackDown;
	public Sprite DefenseUp;
	public Sprite DefenseDown;

	public Slider hpSlider;				// Reference to the slider representing unit's HP
	public Gradient hpGradient;			// Gradient for HP bar color

	public void SetHUD(Unit unit)
	{
		nameText.text = unit.unitName;				// Update the name text with unit's name
		levelText.text = "Lvl " + unit.unitLevel;	// Update the level text with unit's level

		AtkIcon.gameObject.SetActive(false);
		DefIcon.gameObject.SetActive(false);
		buffText.text = "";							// Leave blank at the start

		hpSlider.maxValue = unit.maxHP;				// Set max value of HP slider to unit's max HP
		hpSlider.value = unit.currentHP;			// Set current value of HP slider to unit's current HP

		// Update HP bar color based on HP percentage
		UpdateHPColor();
	}

	public void SetStatChange(Unit unit) {
		buffText.text = "";
		AtkIcon.gameObject.SetActive(false);
		DefIcon.gameObject.SetActive(false);
		
		if(unit.attackStage != 0) {
			AtkIcon.gameObject.SetActive(true);
			if (unit.attackStage > 0) {
				AtkIcon.sprite = AttackUp;
				buffText.text += "+" + unit.attackStage;
			} else if (unit.attackStage < 0) {
				AtkIcon.sprite = AttackDown;
				buffText.text += unit.attackStage;
			}
		}

		if(unit.defenseStage != 0) {
			DefIcon.gameObject.SetActive(true);
			if (unit.defenseStage > 0) {
				DefIcon.sprite = DefenseUp;
				buffText.text += "\n\n+" + unit.defenseStage;
			} else if (unit.defenseStage < 0) {
				DefIcon.sprite = DefenseDown;
				buffText.text += "\n\n" + unit.defenseStage;
			}
		}
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
		int steps = 5;					// Number of steps for the sliding effect

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
