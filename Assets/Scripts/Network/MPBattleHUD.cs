using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class MPBattleHUD : NetworkBehaviour {

	[Header("HUD Text")]
	[SerializeField] private TMP_Text nameText;
	[SerializeField] private TMP_Text atkText;
	[SerializeField] private TMP_Text defText;
	[SerializeField] private TMP_Text speText;

	[Header("HUD Icons")]
	[SerializeField] private SpriteRenderer AtkIcon;        // Reference to the icon used for attack stages
	[SerializeField] private SpriteRenderer DefIcon;        // Reference to the icon used for defense stages
	[SerializeField] private SpriteRenderer SpeIcon;		// Reference to the icon used for speed stages
	[SerializeField] private SpriteRenderer CDIcon;         // Reference to the icon used for turn cooldown

	[SerializeField] private Sprite AttackUp;
	[SerializeField] private Sprite AttackDown;
	[SerializeField] private Sprite DefenseUp;
	[SerializeField] private Sprite DefenseDown;
	[SerializeField] private Sprite SpeedUp;
	[SerializeField] private Sprite SpeedDown;

	[Header("HP Bar")]
	[SerializeField] private Slider hpSlider;
	[SerializeField] private Gradient hpGradient;

	public void SetHUD(Unit unit) {
		nameText.text = unit.unitName;
		AtkIcon.gameObject.SetActive(false);
		DefIcon.gameObject.SetActive(false);
		SpeIcon.gameObject.SetActive(false);
		CDIcon.gameObject.SetActive(false);

		atkText.text = ""; defText.text = ""; speText.text = "";

		hpSlider.maxValue = unit.maxHP;
		hpSlider.value = unit.currentHP;
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

	public void SetHP(int hp) {
		StartCoroutine(SlideHP(hp));
	}

	IEnumerator SlideHP(int newHP) {
		float startHP = hpSlider.value;
		float endHP = newHP;
		float duration = 0.7f;
		int steps = 5;
		float stepAmount = (endHP - startHP) / steps;

		// Animate HP slider over multiple steps
		for (int i = 0; i < steps; i++) {
			hpSlider.value += stepAmount;
			UpdateHPColor();
			yield return new WaitForSeconds(duration / steps);
		}

		hpSlider.value = endHP;
		UpdateHPColor();
	}

	void UpdateHPColor() {
		float fillAmount = hpSlider.value / hpSlider.maxValue;
		hpSlider.fillRect.GetComponent<Image>().color = hpGradient.Evaluate(fillAmount);
	}

	[ClientRpc]
	public void UpdateHUDClientRpc(int currentHP, int maxHP, int attackStage, int defenseStage, string unitName) {
		hpSlider.maxValue = maxHP;
		hpSlider.value = currentHP;
		nameText.text = unitName;

		// Update attack and defense stages
		atkText.text = "";
		defText.text = "";
		AtkIcon.gameObject.SetActive(false);
		DefIcon.gameObject.SetActive(false);

		if (attackStage != 0) {
			AtkIcon.gameObject.SetActive(true);
			AtkIcon.sprite = attackStage > 0 ? AttackUp : AttackDown;
			atkText.text = (attackStage > 0 ? "+" : "") + attackStage.ToString();
		}

		if (defenseStage != 0) {
			DefIcon.gameObject.SetActive(true);
			DefIcon.sprite = defenseStage > 0 ? DefenseUp : DefenseDown;
			defText.text = (defenseStage > 0 ? "+" : "") + defenseStage.ToString();
		}

		// Update HP color
		UpdateHPColor();
	}

	[ClientRpc]
	public void CooldownOnClientRpc() {
		CDIcon.gameObject.SetActive(true);
	}

	[ClientRpc]
	public void CooldownOffClientRpc() {
		CDIcon.gameObject.SetActive(false);
	}

	public void CooldownOn() {
		CDIcon.gameObject.SetActive(true);
	}

	public void CooldownOff() {
		CDIcon.gameObject.SetActive(false);
	}
}
