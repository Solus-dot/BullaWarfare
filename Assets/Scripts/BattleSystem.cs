using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BattleState { START, P1_TURN, P2_TURN, P1_WIN, P2_WIN }


public class BattleSystem : MonoBehaviour {

	public Transform P1_BattleStation;
	public Transform P2_BattleStation;

	Unit P1_Unit;
	Unit P2_Unit;

	public TMP_Text dialogueText;

	public BattleHUD P1_HUD;
	public BattleHUD P2_HUD;

	public BattleState state;
	private bool actionInProgress = false;

	public Button move1Button;
	public Button move2Button;
	public Button move3Button;
	public Button move4Button;

	public TMP_Text move1Button_Text;
	public TMP_Text move2Button_Text;
	public TMP_Text move3Button_Text;
	public TMP_Text move4Button_Text;

	private float turnDelay = 1f; // Delay between turns

	void Start() {
		move1Button_Text.text = "";
		move2Button_Text.text = "";
		move3Button_Text.text = "";
		move4Button_Text.text = "";

		state = BattleState.START;
		StartCoroutine(SetupBattle());
	}

	IEnumerator SetupBattle() {
		GameObject P1_GameObject = InstantiatePrefab(CharacterSelectManager.Instance.GetSelectedCharacterPrefab(1), P1_BattleStation);
		P1_Unit = P1_GameObject.GetComponent<Unit>();

		GameObject P2_GameObject = InstantiatePrefab(CharacterSelectManager.Instance.GetSelectedCharacterPrefab(2), P2_BattleStation);
		P2_Unit = P2_GameObject.GetComponent<Unit>();

		dialogueText.text = "An intense battle between " + P1_Unit.unitName + " and " + P2_Unit.unitName + " commences...";

		P1_HUD.SetHUD(P1_Unit);
		P2_HUD.SetHUD(P2_Unit);

		yield return new WaitForSeconds(1f);

		state = BattleState.P1_TURN;
		StartCoroutine(PlayerTurn());
	}

	GameObject InstantiatePrefab(GameObject prefab, Transform parentTransform) {
		float Offset = 1f; // Y offset from battlestation
		return Instantiate(prefab, parentTransform.position + new Vector3(0, Offset, 0), parentTransform.rotation, parentTransform);
	}

	IEnumerator PlayerTurn() {
		DisableActionButtons();

		if (state == BattleState.P1_TURN) {
			dialogueText.text = "Player 1's Turn: Choose an action.";
			move1Button_Text.text = P1_Unit.GetMove(0).moveName;
			move2Button_Text.text = P1_Unit.GetMove(1).moveName;
			move3Button_Text.text = P1_Unit.GetMove(2).moveName;
			move4Button_Text.text = P1_Unit.GetMove(3).moveName;

		} else if (state == BattleState.P2_TURN) {
			dialogueText.text = "Player 2's Turn: Choose an action.";
			move1Button_Text.text = P2_Unit.GetMove(0).moveName;
			move2Button_Text.text = P2_Unit.GetMove(1).moveName;
			move3Button_Text.text = P2_Unit.GetMove(2).moveName;
			move4Button_Text.text = P2_Unit.GetMove(3).moveName;
		}

		yield return new WaitForSeconds(turnDelay);

		EnableActionButtons();
	}

	void EndBattle() {
		DisableActionButtons();

		if (state == BattleState.P1_WIN) {
			dialogueText.text = P1_Unit.unitName + " (Player 1) has Won!";
		} else if (state == BattleState.P2_WIN) {
			dialogueText.text = P2_Unit.unitName + " (Player 2) has won!";
		}
	}

	public void OnMoveButton(int moveIndex) {
		if (!actionInProgress) {
			if (state == BattleState.P1_TURN) {
				StartCoroutine(HandleMove(P1_Unit, P2_Unit, moveIndex));
			} else if (state == BattleState.P2_TURN) {
				StartCoroutine(HandleMove(P2_Unit, P1_Unit, moveIndex));
			}
		}
	}

	IEnumerator HandleMove(Unit attacker, Unit defender, int moveIndex) {
		actionInProgress = true;
		Move move = attacker.GetMove(moveIndex);
		bool isDead = false;

		// Calculate if the move hits or misses
    	int hitChance = attacker.baseAccuracy * move.accuracy / 100;
    	int randomValue = Random.Range(0, 100);
		
		if (randomValue < hitChance) {
			if (move.isDamaging) {
				int damage = attacker.attack * move.damage / 20; // Calculate effective damage
				isDead = defender.TakeDamage(damage);
				dialogueText.text = damage + "!!";

				if (attacker == P1_Unit) {
					P2_HUD.SetHP(defender.currentHP);
				} else {
					P1_HUD.SetHP(defender.currentHP);
				}		
			} 

			if (move.isHealingMove) {
				int healAmount = move.healAmount; // Implement proper heal logic
				attacker.Heal(healAmount);
				dialogueText.text = move.moveDesc;
			} 

			if (move.isStatChange) {
				if (move.selfAttackChange != 0) {attacker.TakeBuff(move.selfAttackChange, 0);}
				if (move.selfDefenseChange != 0) {attacker.TakeBuff(0, move.selfDefenseChange);}
				if (move.oppAttackChange != 0) {defender.TakeBuff(move.oppAttackChange, 0);}
				if (move.oppDefenseChange != 0) {defender.TakeBuff(0, move.oppDefenseChange);}

				P1_HUD.SetStatChange(P1_Unit);
				P2_HUD.SetStatChange(P2_Unit);
			}
		} else {
			dialogueText.text = "The move missed!";
		}

		yield return new WaitForSeconds(turnDelay);

		if (isDead) {
			state = (state == BattleState.P1_TURN) ? BattleState.P1_WIN : BattleState.P2_WIN;
			EndBattle();
		} else {
			SwitchTurn();
		}

		actionInProgress = false;
	}

	void SwitchTurn() {
		state = (state == BattleState.P1_TURN) ? BattleState.P2_TURN : BattleState.P1_TURN;
		StartCoroutine(PlayerTurn());
	}

	void DisableActionButtons() {
		move1Button.interactable = false;
		move2Button.interactable = false;
		move3Button.interactable = false;
		move4Button.interactable = false;
	}

	void EnableActionButtons() {
		move1Button.interactable = true;
		move2Button.interactable = true;
		move3Button.interactable = true;
		move4Button.interactable = true;
	}
}
