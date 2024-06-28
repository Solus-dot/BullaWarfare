	using System.Collections;
	using UnityEngine;
	using UnityEngine.UI;
	using TMPro;

	public class BattleHUD : MonoBehaviour
	{
	public TMP_Text nameText;

	public TMP_Text atkText;
	public TMP_Text defText;

	public SpriteRenderer AtkIcon;		// Reference to the icon used for attack stages
	public SpriteRenderer DefIcon;		// Reference to the icon used for defense stages
	public SpriteRenderer CDIcon;		// Reference to the icon used for turn cooldown

	public Sprite AttackUp;
	public Sprite AttackDown;
	public Sprite DefenseUp;
	public Sprite DefenseDown;

	public Slider hpSlider;				// Reference to the slider representing unit's HP
	public Gradient hpGradient;			// Gradient for HP bar color

	public void SetHUD(Unit unit)
	{
		nameText.text = unit.unitName;				// Update the name text with unit's name
		AtkIcon.gameObject.SetActive(false);
		DefIcon.gameObject.SetActive(false);
		CDIcon.gameObject.SetActive(false);

		atkText.text = ""; defText.text = "";						// Leave blank at the start

		hpSlider.maxValue = unit.maxHP;				// Set max value of HP slider to unit's max HP
		hpSlider.value = unit.currentHP;			// Set current value of HP slider to unit's current HP

		// Update HP bar color based on HP percentage
		UpdateHPColor();
	}

	public void SetStatChange(Unit unit) {
		atkText.text = ""; defText.text = "";
		AtkIcon.gameObject.SetActive(false);
		DefIcon.gameObject.SetActive(false);
		
		if(unit.attackStage != 0) {
			AtkIcon.gameObject.SetActive(true);
			if (unit.attackStage > 0) {
				AtkIcon.sprite = AttackUp;
				atkText.text += "+" + unit.attackStage;
			} else if (unit.attackStage < 0) {
				AtkIcon.sprite = AttackDown;
				atkText.text += unit.attackStage;
			}
		}

		if(unit.defenseStage != 0) {
			DefIcon.gameObject.SetActive(true);
			if (unit.defenseStage > 0) {
				DefIcon.sprite = DefenseUp;
				defText.text += "+" + unit.defenseStage;
			} else if (unit.defenseStage < 0) {
				DefIcon.sprite = DefenseDown;
				defText.text += unit.defenseStage;
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

	public void CooldownOn() {
		CDIcon.gameObject.SetActive(true);
	}

	public void CooldownOff() {
		CDIcon.gameObject.SetActive(false);
	}
}
