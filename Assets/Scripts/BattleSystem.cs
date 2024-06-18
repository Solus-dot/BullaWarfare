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
	public TMP_Text moveText;

	public BattleHUD P1_HUD;
	public BattleHUD P2_HUD;

	public BattleState state;
	private bool actionInProgress = false;

	public Button move1Button;
	public Button move2Button;
	public Button move3Button;
	public Button move4Button;

	TMP_Text move1Button_Text;
	TMP_Text move2Button_Text;
	TMP_Text move3Button_Text;
	TMP_Text move4Button_Text;

	private float turnDelay = 1f; // Delay between turns

	void Start() {
		move1Button_Text = move1Button.GetComponentInChildren<TMP_Text>();
		move2Button_Text = move2Button.GetComponentInChildren<TMP_Text>();
		move3Button_Text = move3Button.GetComponentInChildren<TMP_Text>();
		move4Button_Text = move4Button.GetComponentInChildren<TMP_Text>();

		dialogueText.gameObject.SetActive(false);
		moveText.gameObject.SetActive(false);
		DisableActionButtons();

		state = BattleState.START;
		StartCoroutine(SetupBattle());
	}

	IEnumerator SetupBattle() {
		GameObject P1_GameObject = InstantiatePrefab(CharacterSelectManager.Instance.GetSelectedCharacterPrefab(1), P1_BattleStation);
		P1_Unit = P1_GameObject.GetComponent<Unit>();
		GameObject P2_GameObject = InstantiatePrefab(CharacterSelectManager.Instance.GetSelectedCharacterPrefab(2), P2_BattleStation);
		P2_Unit = P2_GameObject.GetComponent<Unit>();

		P1_HUD.SetHUD(P1_Unit);
		P2_HUD.SetHUD(P2_Unit);

		yield return StartCoroutine(DisplayMoveText("An intense battle between " + P1_Unit.unitName + " and " + P2_Unit.unitName + " commences...", 0.07f));

		yield return new WaitForSeconds(turnDelay);
		state = BattleState.P1_TURN;
		StartCoroutine(PlayerTurn());
	}

	GameObject InstantiatePrefab(GameObject prefab, Transform parentTransform) {
		float Offset = 1f; // Y offset from battlestation
		return Instantiate(prefab, parentTransform.position + new Vector3(0, Offset, 0), parentTransform.rotation, parentTransform);
	}

	IEnumerator PlayerTurn() {
		if (state == BattleState.P1_TURN) {
			if (P1_Unit.isFlinching) {
				yield return StartCoroutine(DisplayMoveText(P1_Unit.unitName + " flinched and couldn't move!", 0.05f));
				P1_Unit.EndTurn();
				yield return new WaitForSeconds(turnDelay);
				state = BattleState.P2_TURN;
				StartCoroutine(PlayerTurn());
				yield break;
			}
			dialogueText.gameObject.SetActive(true);
			moveText.gameObject.SetActive(false);

			dialogueText.text = "Player 1's Turn: Choose an action.";
			move1Button_Text.text = P1_Unit.GetMove(0).moveName;
			move2Button_Text.text = P1_Unit.GetMove(1).moveName;
			move3Button_Text.text = P1_Unit.GetMove(2).moveName;
			move4Button_Text.text = P1_Unit.GetMove(3).moveName;

		} else if (state == BattleState.P2_TURN) {
			if (P2_Unit.isFlinching) {
				yield return StartCoroutine(DisplayMoveText(P2_Unit.unitName + " flinched and couldn't move!", 0.05f));
				P2_Unit.EndTurn();
				yield return new WaitForSeconds(turnDelay);
				state = BattleState.P1_TURN;
				StartCoroutine(PlayerTurn());
				yield break;
			}
			dialogueText.gameObject.SetActive(true);
			moveText.gameObject.SetActive(false);

			dialogueText.text = "Player 2's Turn: Choose an action.";
			move1Button_Text.text = P2_Unit.GetMove(0).moveName;
			move2Button_Text.text = P2_Unit.GetMove(1).moveName;
			move3Button_Text.text = P2_Unit.GetMove(2).moveName;
			move4Button_Text.text = P2_Unit.GetMove(3).moveName;
		}

		yield return new WaitForSeconds(0f);
		EnableActionButtons();
	}

	void EndBattle() {
		DisableActionButtons();
		dialogueText.gameObject.SetActive(true);
		moveText.gameObject.SetActive(false);

		if (state == BattleState.P1_WIN) {
			dialogueText.text = P1_Unit.unitName + " (Player 1) has Won!";
		} else if (state == BattleState.P2_WIN) {
			dialogueText.text = P2_Unit.unitName + " (Player 2) has won!";
		}
	}

	public void OnMoveButton(int moveIndex) {
		if (!actionInProgress) {
			dialogueText.gameObject.SetActive(false);
			moveText.gameObject.SetActive(true);

			if (state == BattleState.P1_TURN) {
				StartCoroutine(HandleMove(P1_Unit, P2_Unit, moveIndex));
			} else if (state == BattleState.P2_TURN) {
				StartCoroutine(HandleMove(P2_Unit, P1_Unit, moveIndex));
			}
		}
	}

	IEnumerator HandleMove(Unit attacker, Unit defender, int moveIndex) {
		DisableActionButtons();
		InitializeMoveset();
		
		actionInProgress = true;
		Move move = attacker.GetMove(moveIndex);
		string hitMessage = move.moveMessage.Replace("(opp_name)", defender.unitName).Replace("(value)", defender.recvEffectiveDamage.ToString());
		bool isDead = false;

		// Calculate if the move hits or misses
		int hitChance = attacker.baseAccuracy * move.accuracy / 100;
		int randomValue = Random.Range(0, 100);

		if (randomValue < hitChance) {
			if (move.recoil > 0 && move.recoil * attacker.maxHP >= attacker.currentHP) {
				yield return StartCoroutine(DisplayMoveText(attacker.unitName + " is not healthy enough to use this move!", 0.05f));
			} else {
				if (move.isDamaging) {
					int damage = attacker.attack * move.damage / 20; // Calculate effective damage
					isDead = defender.TakeDamage(damage);

					if (attacker == P1_Unit) {
						P2_HUD.SetHP(defender.currentHP);
					} else {
						P1_HUD.SetHP(defender.currentHP);
					}

					hitMessage = move.moveMessage.Replace("(opp_name)", defender.unitName).Replace("(value)", defender.recvEffectiveDamage.ToString());
				}

				if (move.isHealingMove) {
					int healAmount = move.healAmount; // Implement proper heal logic
					attacker.Heal(healAmount);

					if (attacker == P1_Unit) {
						P1_HUD.SetHP(attacker.currentHP);
					} else {
						P2_HUD.SetHP(attacker.currentHP);
					}
				}

				if (move.isStatChange) {
					if (move.selfAttackChange != 0) { attacker.TakeBuff(move.selfAttackChange, 0); }
					if (move.selfDefenseChange != 0) { attacker.TakeBuff(0, move.selfDefenseChange); }
					if (move.oppAttackChange != 0) { defender.TakeBuff(move.oppAttackChange, 0); }
					if (move.oppDefenseChange != 0) { defender.TakeBuff(0, move.oppDefenseChange); }

					P1_HUD.SetStatChange(P1_Unit);
					P2_HUD.SetStatChange(P2_Unit);
				}

				if (move.flinch > 0) {
					defender.AttemptFlinch(move);
				}


				if (move.recoil > 0) {
					int recoilDamage = Mathf.CeilToInt(attacker.maxHP * move.recoil);
					attacker.TakeDamage(recoilDamage);

					if (attacker == P1_Unit) {
						P1_HUD.SetHP(attacker.currentHP);
					} else {
						P2_HUD.SetHP(attacker.currentHP);
					}
				}

				yield return StartCoroutine(DisplayMoveText(hitMessage, 0.05f));
			}
		} else {
			if (move.missMessage != null) {
				yield return StartCoroutine(DisplayMoveText(move.missMessage.Replace("(opp_name)", defender.unitName), 0.05f));
			} else {
				yield return StartCoroutine(DisplayMoveText("The move missed!", 0.05f));
			}
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

	IEnumerator DisplayMoveText(string text, float speed) {
		moveText.gameObject.SetActive(true);
		moveText.text = "";
		foreach (char letter in text.ToCharArray()) {
			moveText.text += letter;
			yield return new WaitForSeconds(speed);
		}
		yield return new WaitForSeconds(turnDelay);
	}

	void SwitchTurn() {
		state = (state == BattleState.P1_TURN) ? BattleState.P2_TURN : BattleState.P1_TURN;
		StartCoroutine(PlayerTurn());
	}

	void DisableActionButtons() {
		move1Button.gameObject.SetActive(false);
		move2Button.gameObject.SetActive(false);
		move3Button.gameObject.SetActive(false);
		move4Button.gameObject.SetActive(false);
	}

	void EnableActionButtons() {
		move1Button.gameObject.SetActive(true);
		move2Button.gameObject.SetActive(true);
		move3Button.gameObject.SetActive(true);
		move4Button.gameObject.SetActive(true);
	}

	void InitializeMoveset() {
		Sohom.Initialize();
		Ravi.Initialize();
		Manas.Initialize();
		Harsh.Initialize();
		Arya.Initialize();
		Khush.Initialize();
		Aditi.Initialize();
		Sarv.Initialize();
	}
}
