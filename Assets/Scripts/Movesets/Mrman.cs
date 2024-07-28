// Aaryaman

// Move 1: Chill Like'At
// Aaryaman approaches the opponent with utter coolness and does a perfect dap with the opponent.
// The opponent loses guard and loses 1 stage of DEF. The echoes of the perfect dap raises ATK of both sides by 1 Stage.

// Move 2 : Mr Worldwide 
// Aaryaman uses his knowledge of the plane he has acquired from travelling all around the globe to.
// increase his ATK by 1 stage.

// Move 3 : Poker Pro
// Aaryaman starts a Poker game with the opponent, wagering 75 health points.
// Except he uses his skills in economics and poker intution to win the game every time. Opponent receives 75 HP of Damage exactly.

// Move 4 : Ankara Messi (BP :- 25)
// Aaryaman uses his chicken legs and soccer skills to repeatedly hit the opponent with devastating shoots.
// Deals damage per shot, number of shots varies from 2 to 6.

// Move Messages
// Move 1: Aaryaman starts walking towards (opp_name) with utter chillness. And all of a sudden CLAP, the sound of the perfect dap echoes the battlefield. Attack of both sides Increase by 1 stage. The opponent loses Defense by 1 stage.
// Move 2: Aaryaman screams "MR WORLDWIDE" in the middle of the battle. Bro fr think he Pitbull, but still he increases his attack by 1 stage.
// Move 3: Aaryaman claps his hands and a poker table appears on the battlefield. The poker battle commences! Aaryaman uses his economic skills to win the deal and deals exactly 75 HP of damage to (opp_name).
// Move 4: Skinny Legs Aaryaman summons a football, and starts performing a volley with skill, hitting the opponent repeatedly with the ball. It hit x amount of times, (value) damage dealt.

using UnityEngine;
using System.Collections.Generic;

public static class Mrman
{
	public static List<Move> moves = new List<Move>() {
		// Chill Like'At move
		new Move() {
			moveName = "Chill Like'At",
			moveDesc = "Aaryaman approaches the opponent with utter coolness and does a perfect dap with the opponent. The opponent loses guard and loses 1 stage of DEF. The echoes of the perfect dap raises ATK of both sides by 1 Stage.",
			moveMessage = "Aaryaman starts walking towards (opp_name) with utter chillness. And all of a sudden CLAP, the sound of the perfect dap echoes the battlefield. Attack of both sides Increase by 1 stage. The opponent loses Defense by 1 stage.",
            isCooldown = false,
			accuracy = 100,
			isDamaging = false,
			isHealingMove = false,
			isStatChange = true,
			selfAttackChange = 1,
			selfDefenseChange = 0,
			oppAttackChange = 1,
			oppDefenseChange = -1
		},

		// Mr Worldwide Move
		new Move() {
			moveName = "Mr Worldwide",
			moveDesc = "Aaryaman uses his knowledge of the plane he has acquired from travelling all around the globe to. increase his ATK by 1 stage.",
			moveMessage = " Aaryaman screams \"MR WORLDWIDE\" in the middle of the battle. Bro fr think he Pitbull, but still he increases his attack by 1 stage.",
			isCooldown = false,
			accuracy = 100,
			isDamaging = false,
			isHealingMove = false,
			isStatChange = true,
            selfAttackChange = 1,
			selfDefenseChange = 0,
			oppAttackChange = 0,
			oppDefenseChange = 0
		},

		// Poker Pro Move
		new Move() { 
			moveName = "Poker Pro",
			moveDesc = "Aaryaman starts a Poker game with the opponent, wagering 75 HP. Except he uses his skills in economics and poker intution to win the game every time. Opponent receives 75 HP of Damage exactly.",
			moveMessage = " Aaryaman claps his hands and a poker table appears on the battlefield. The poker battle commences! Aaryaman uses his economic skills to win the deal and deals exactly 75 HP of damage to (opp_name).",
			isCooldown = false,
			accuracy = 100,
			isDamaging = true,
			trueDamage = 75,
			isHealingMove = false,
			isStatChange = false,
		},

		// Ankara Messi Move
		new Move() {
			moveName = "Ankara Messi",
			moveDesc = "Aaryaman uses his chicken legs and soccer skills to repeatedly hit the opponent with devastating shoots. Deals damage per shot, number of shots varies from 2 to 6.",
			moveMessage = "Skinny Legs Aaryaman summons a football, and starts performing a volley with skill, hitting the opponent repeatedly with the ball.",
			isCooldown = false,
			accuracy = 100,
			isDamaging = true,
			damage = 25,
			isHealingMove = false,
			isStatChange = false,
		}
	};

	// Initialize function for any random/niche Move effect
	public static void Initialize() {
		moves[3].moveMessage = "";
		moves[3].damage = 25;

		int x = Random.Range(1, 100);
		Debug.Log(x);
		if (x <= 20) {
            moves[3].moveMessage += "He shoots it twice, dealing (value) damage!";
			moves[3].damage = 50;
		} else if (x <= 40) {
            moves[3].moveMessage += "He shoots it thrice, dealing (value) damage!";
            moves[3].damage = 75;
        } else if (x <= 60) {
            moves[3].moveMessage += "He shoots it 4 times, dealing (value) damage!";
            moves[3].damage = 100;           
        } else if (x <= 80) {
            moves[3].moveMessage += "He shoots it 5 times, dealing (value) damage!";
            moves[3].damage = 125;
        } else {
            moves[3].moveMessage += "He shoots it 6 times, dealing (value) damage!";
            moves[3].damage = 150;
        }
	}
}