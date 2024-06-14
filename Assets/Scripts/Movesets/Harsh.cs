// Harsh

// Move 1: Too Much Radio
// Plays a radiohead song which makes everyone around them sad,
// Opponent loses Defense stat.

// Move 2: Literally Me
// Boasts about being Ryan Gosling, angering the opponents,
// Opponent's Defense drops 2 stages while Attack goes up 1 stage.

// Move 3: QC Ka Choda
// Hurts the opponents by spitting random facts to the opponent,
// Deals decent damage to opponent.

// Move 4: Funny Valentine
// Makes an incredibly funny and self-depricating joke that they hurt themselves,
// Deals good damage but reduces the Defense stat of the user.

using UnityEngine;
using System.Collections.Generic;

public static class Harsh
{
	public static List<Move> moves = new List<Move>() {
		// Too Much Radio Move
		new Move() {
			moveName = "Too Much Radio",
            moveDesc = "Plays a radiohead song which makes everyone around them sad,\nOpponent loses Defense stat.",
			cooldown = 0,
			accuracy = 100,
			isDamaging = false,
			isHealingMove = false,
			isStatChange = true,
			selfAttackChange = 0,
			selfDefenseChange = 0,
			oppAttackChange = 0,
			oppDefenseChange = -1
		},

		// Literally Me Move
		new Move() {
			moveName = "Literally Me",
			moveDesc = "Boasts about being Ryan Gosling, angering the opponents,\nOpponent's Defense drops 2 stages while Attack goes up 1 stage.",
			cooldown = 0,
			accuracy = 100,
			isDamaging = false,
			isHealingMove = false,
			isStatChange = true,
			selfAttackChange = 0,
			selfDefenseChange = 0,
			oppAttackChange = 1,
			oppDefenseChange = -2
		},

		// QC Ka Choda Move
		new Move() { 
			moveName = "QC Ka Choda",
			moveDesc = "Hurts the opponents by spitting random facts to the opponent,\nDeals decent damage to opponent.",
			cooldown = 0,
			accuracy = 100,
			isDamaging = true,
            damage = 20,
			isHealingMove = false,
			isStatChange = false,
		},

		// Funny Valentine Move
		new Move() {
			moveName = "Funny Valentine",
			moveDesc = "Makes an incredibly funny and self-depricating joke that they hurt themselves,\nDeals good damage but reduces the Defense stat of the user.",
			cooldown = 1,
			accuracy = 100,
			isDamaging = true,
			damage = 20,
			isHealingMove = false,
			isStatChange = true,
			selfAttackChange = 0,
			selfDefenseChange = -1,
			oppAttackChange = 0,
			oppDefenseChange = 0
		}
	};
}
