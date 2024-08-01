using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public enum BattleState { START, P1_TURN, P2_TURN, P1_WIN, P2_WIN }

public class BattleSystem : MonoBehaviour {
	[Header("Transforms")]
	public Transform P1BattleStation;
	public Transform P2BattleStation;

	Unit P1_Unit;
	Unit P2_Unit;

	[Header("UI")]
	public TMP_Text dialogueText;
	public TMP_Text moveText;

	public BattleHUD P1_HUD;
	public BattleHUD P2_HUD;

	[Header("MoveButton")]
	public Button move1Button;
	public Button move2Button;
	public Button move3Button;
	public Button move4Button;

	TMP_Text move1Button_Text;
	TMP_Text move2Button_Text;
	TMP_Text move3Button_Text;
	TMP_Text move4Button_Text;

	GameObject P1_GameObject;
	GameObject P2_GameObject;

	SpriteRenderer P1_SpriteRenderer;
	SpriteRenderer P2_SpriteRenderer;

	private BattleState state;
	private bool actionInProgress = false;

	private Color originalColor;			// Battle Station Original Color
	private float turnDelay = 1f;			// Delay between turns

	Vector3 Offset;			// Position of Character with respect to Station
	float bobbingInterval = 0.8f;	// Interval for the bobbing effect
	float bobbingDistance = 0.1f;	// Max distance for the bobbing effect

	public void Start() {
		StopAllCoroutines();
		Destroy(P1_GameObject);
		Destroy(P2_GameObject);

		move1Button_Text = move1Button.GetComponentInChildren<TMP_Text>();
		move2Button_Text = move2Button.GetComponentInChildren<TMP_Text>();
		move3Button_Text = move3Button.GetComponentInChildren<TMP_Text>();
		move4Button_Text = move4Button.GetComponentInChildren<TMP_Text>();

		P1_SpriteRenderer = P1BattleStation.GetComponent<SpriteRenderer>();
		P2_SpriteRenderer = P2BattleStation.GetComponent<SpriteRenderer>();

		originalColor = Color.white;
		Offset = new Vector3(0, 1f, 0);

		dialogueText.gameObject.SetActive(false);
		moveText.gameObject.SetActive(false);
		DisableActionButtons();

		state = BattleState.START;
		StartCoroutine(SetupBattle());
	}

	void Update() {
		if (state == BattleState.P1_WIN || state == BattleState.P2_WIN) {
			if (Input.GetKeyDown(KeyCode.Z)) {
				SceneManager.LoadScene("MainMenuScene");
			} else if (Input.GetKeyDown(KeyCode.R)) {
				Start();
			} else if (Input.GetKeyDown(KeyCode.X)) {
				SceneManager.LoadScene("CharacterSelect");
			}
		}
	}

	IEnumerator SetupBattle() {
		Vector3 P1_StartPosition = new Vector3(-10, P1BattleStation.position.y, P1BattleStation.position.z);
		Vector3 P2_StartPosition = new Vector3(10, P2BattleStation.position.y, P2BattleStation.position.z);
		Vector3 P1_TargetPosition = P1BattleStation.position;
		Vector3 P2_TargetPosition = P2BattleStation.position;

		P1BattleStation.position = P1_StartPosition;
		P2BattleStation.position = P2_StartPosition;

		// Set up Player 1
		P1_GameObject = InstantiatePrefab(CharacterSelectManager.Instance.GetSelectedCharacterPrefab(1), P1BattleStation);
		P1_Unit = P1_GameObject.GetComponent<Unit>();
		
		
		// Set up Player 2
		P2_GameObject = InstantiatePrefab(CharacterSelectManager.Instance.GetSelectedCharacterPrefab(2), P2BattleStation);
		P2_Unit = P2_GameObject.GetComponent<Unit>();
		

		StartCoroutine(MoveToBattlePosition(P1BattleStation, P1_GameObject.transform, P1_TargetPosition, 0.25f, 0.2f));
		StartCoroutine(MoveToBattlePosition(P2BattleStation, P2_GameObject.transform, P2_TargetPosition, 0.25f, 0.2f));

		Debug.Log(P1BattleStation.position);
		Debug.Log(P2BattleStation.position);

		// Set up the character HUDs
		P1_HUD.SetHUD(P1_Unit);
		P2_HUD.SetHUD(P2_Unit);

		P1_SpriteRenderer.color = originalColor;
		P2_SpriteRenderer.color = originalColor;

		yield return StartCoroutine(DisplayMoveTextUnskip("An intense battle between " + P1_Unit.unitName + " and " + P2_Unit.unitName + " commences...", 0.05f));
		yield return new WaitForSeconds(turnDelay);

		StartCoroutine(BobbingEffect(P1_GameObject.transform, P1BattleStation.position + Offset));
		StartCoroutine(BobbingEffect(P2_GameObject.transform, P2BattleStation.position + Offset));

		state = BattleState.P1_TURN;
		StartCoroutine(PlayerTurn());
	}

	private GameObject InstantiatePrefab(GameObject prefab, Transform parentTransform) {
		return Instantiate(prefab, parentTransform.position + Offset, parentTransform.rotation, parentTransform);
	}

	IEnumerator PlayerTurn() {
		if (state == BattleState.P1_TURN) {
			yield return HandlePlayerTurn(P1_Unit, P2_Unit, P1_HUD, P2_HUD, "Player 1's Turn: Choose an action.", Color.yellow);
		} else if (state == BattleState.P2_TURN) {
			yield return HandlePlayerTurn(P2_Unit, P1_Unit, P2_HUD, P1_HUD, "Player 2's Turn: Choose an action.", Color.yellow);
		}
	}

	private IEnumerator HandlePlayerTurn(Unit currentPlayer, Unit opponent, BattleHUD currentPlayerHUD, BattleHUD opponentHUD, string dialogue, Color highlightColor) {
		if (currentPlayer.isFlinching) {
			yield return DisplayFlinchMessage(currentPlayer);
			yield break;
		}

		if (currentPlayer.isOnCooldown) {
			yield return DisplayCooldownMessage(currentPlayer, currentPlayerHUD);
			yield break;
		}

		PrepareForPlayerTurn(dialogue, highlightColor);
		SetMoveButtonText(currentPlayer);
		EnableActionButtons();
	}

	private IEnumerator DisplayFlinchMessage(Unit player) {
		yield return StartCoroutine(DisplayMoveText($"{player.unitName} flinched and couldn't move!", 0.05f));
		player.EndTurn();
		yield return new WaitForSeconds(turnDelay);
		SwitchTurn();
	}

	private IEnumerator DisplayCooldownMessage(Unit player, BattleHUD playerHUD) {
		yield return StartCoroutine(DisplayMoveText(player.cooldownMessage, 0.05f));
		player.EndTurn();
		playerHUD.CooldownOff();
		yield return new WaitForSeconds(turnDelay);
		SwitchTurn();
	}

	private void PrepareForPlayerTurn(string dialogue, Color highlightColor) {
		dialogueText.gameObject.SetActive(true);
		moveText.gameObject.SetActive(false);

		dialogueText.text = dialogue;
		P1_SpriteRenderer.color = (state == BattleState.P1_TURN) ? highlightColor : originalColor;
		P2_SpriteRenderer.color = (state == BattleState.P2_TURN) ? highlightColor : originalColor;
	}

	private void SetMoveButtonText(Unit player) {
		move1Button_Text.text = player.GetMove(0).moveName;
		move2Button_Text.text = player.GetMove(1).moveName;
		move3Button_Text.text = player.GetMove(2).moveName;
		move4Button_Text.text = player.GetMove(3).moveName;
	}

	void EndBattle() {
		DisableActionButtons();
		dialogueText.gameObject.SetActive(false);
		moveText.gameObject.SetActive(true);

		string message = state == BattleState.P1_WIN
			? $"{P1_Unit.unitName} (Player 1) has Won!\nPress 'R' to Restart the Match, 'X' for Character Select and 'Z' for Main Menu."
			: $"{P2_Unit.unitName} (Player 2) has Won!\nPress 'R' to Restart the Match, 'X' for Character Select and 'Z' for Main Menu.";

		StartCoroutine(DisplayMoveText(message, 0.05f));
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
		string message = move.moveMessage.Replace("(opp_name)", defender.unitName);
		bool isDead = false;

		// Calculate if the move hits or misses
		int hitChance = attacker.baseAccuracy * move.accuracy / 100;
		int randomHit = Random.Range(0, 100);

		// Check for recoil condition before executing the move
		if (move.recoil > 0 && move.recoil * attacker.maxHP >= attacker.currentHP) {
			yield return StartCoroutine(DisplayMoveText($"{attacker.unitName} is not healthy enough to use this move!", 0.05f));
			yield return new WaitForSeconds(turnDelay);
			SwitchTurn();
			actionInProgress = false;
			yield break;
		}

		bool moveHits = randomHit < hitChance;

		if (moveHits) {
			if (move.isDamaging) {
				int damage = ((move.damage * attacker.currentAtk) / defender.currentDef) + move.trueDamage; // Calculate effective damage (with Defender Defense)
				Debug.Log($"({move.damage} * {attacker.attack} / {defender.defense}) + {move.trueDamage} = {damage}");

				message = move.moveMessage.Replace("(opp_name)", defender.unitName).Replace("(value)", damage.ToString());

				if (Random.Range(1, 100) <= 1) {
					damage *= 2; 	// Critical hit chance (double damage)
					Debug.Log("Critical Hit (x2 Damage)");
					message += " It was a crit hit! It dealt double damage!";
				}
				
				isDead = defender.TakeDamage(damage);

				if (attacker == P1_Unit) {
					P2_HUD.SetHP(defender.currentHP);
				} else {
					P1_HUD.SetHP(defender.currentHP);
				}
			}

			if (move.isHealingMove) {
				int atkHealAmount = (move.selfHealAmount * attacker.maxHP / 100);
				int defHealAmount = (move.oppHealAmount * defender.maxHP / 100);
				if (atkHealAmount != 0) attacker.Heal(atkHealAmount);
				if (defHealAmount != 0) defender.Heal(defHealAmount);

				Debug.Log($"Attacker Heal Amount: {atkHealAmount}");
				Debug.Log($"Defender Heal Amount: {defHealAmount}");

				if (attacker == P1_Unit) {
					P1_HUD.SetHP(attacker.currentHP);
					P2_HUD.SetHP(defender.currentHP);
				} else {
					P2_HUD.SetHP(attacker.currentHP);
					P1_HUD.SetHP(defender.currentHP);
				}
			}

			if (move.isStatChange) {
				attacker.TakeBuff(move.selfAttackChange, move.selfDefenseChange, 0);
				defender.TakeBuff(move.oppAttackChange, move.oppDefenseChange, 0);

				P1_HUD.SetStatChange(P1_Unit);
				P2_HUD.SetStatChange(P2_Unit);
			}

			if (move.flinch > 0) {
				defender.AttemptFlinch(move);
			}
			
		} else {
			message = move.missMessage != null ? move.missMessage.Replace("(opp_name)", defender.unitName) : "The move missed!";
		}

		// Recoil handling after the move
		if (move.recoil > 0) {
			int recoilDamage = Mathf.CeilToInt(attacker.maxHP * move.recoil);
			attacker.TakeDamage(recoilDamage);

			if (attacker == P1_Unit) {
				P1_HUD.SetHP(attacker.currentHP);
			} else {
				P2_HUD.SetHP(attacker.currentHP);
			}
		}

		if (move.isCooldown) {
			attacker.isOnCooldown = true;
			
			if (attacker == P1_Unit) {
				P1_HUD.CooldownOn();
			} else {
				P2_HUD.CooldownOn();
			}
		}

		yield return StartCoroutine(DisplayMoveText(message, 0.05f));
		yield return new WaitForSeconds(turnDelay);

		if (isDead) {
			state = (state == BattleState.P1_TURN) ? BattleState.P1_WIN : BattleState.P2_WIN;
			EndBattle();
		} else {
			SwitchTurn();
		}
		actionInProgress = false;
	}


	IEnumerator DisplayMoveText(string text, float delay) {
		moveText.gameObject.SetActive(true);
		moveText.text = "";
		
		for (int i = 0; i < text.Length; i++) {
			moveText.text += text[i];
			
			if (Input.GetKeyDown(KeyCode.Space)) {
				moveText.text = text;
				break;
			}
			
			float elapsedTime = 0f;
			while (elapsedTime < delay) {
				elapsedTime += Time.deltaTime;
				yield return null;
				
				if (Input.GetKeyDown(KeyCode.Space)) {
					moveText.text = text;
					yield break;
				}
			}
		}
		yield return new WaitForSeconds(turnDelay);
	}

	IEnumerator DisplayMoveTextUnskip(string text, float delay) {
		moveText.gameObject.SetActive(true);
		moveText.text = "";
		foreach (char letter in text.ToCharArray()) {
			moveText.text += letter;
			yield return new WaitForSeconds(delay);
		}
		yield return new WaitForSeconds(turnDelay);
	}

	IEnumerator BobbingEffect(Transform battleStation, Vector3 originalPosition) {
		while (true) {
			float randomX = Random.Range(-bobbingDistance, bobbingDistance);
			float randomY = Random.Range(-bobbingDistance, bobbingDistance);
			float randomZ = Random.Range(-bobbingDistance, bobbingDistance);
			Vector3 randomOffset = new Vector3(randomX, randomY, randomZ);

			battleStation.position = originalPosition + randomOffset;

			yield return new WaitForSeconds(bobbingInterval);
		}
	}

	IEnumerator MoveToBattlePosition(Transform battleStation, Transform characterPrefab, Vector3 targetPosition, float stepSize, float stepDelay) {
		while (Vector3.Distance(battleStation.position, targetPosition) > stepSize) {
			battleStation.position = Vector3.MoveTowards(battleStation.position, targetPosition, stepSize);
			characterPrefab.position = Vector3.MoveTowards(battleStation.position + Offset, targetPosition + Offset, stepSize);
			yield return new WaitForSeconds(stepDelay);
		}
		battleStation.position = targetPosition;  // Ensure final position is exact
		characterPrefab.position = targetPosition + Offset;
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
		Daksh.Initialize();
		Aarav.Initialize();
		Hima.Initialize();
		Vrush.Initialize();
		Mrman.Initialize();
	}
}
