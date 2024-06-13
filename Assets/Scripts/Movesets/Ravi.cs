// Ravi 

// Move 1: Bhangra 
// Dances in front of the opponent like a true chad,
// Boosts Attack by One Stage

// Move 2: Equity Promise
// Promises some %share of his future package to the opponent 
// in exchange for Opponent losing his attack stat by 2 stages. Can only be used once per opponent

// Move 3: Alt F4 IRL
// Uses an illegal shortcut arriving from the depths of /dev/null,
// Deals moderate damage the opponent.

// Move 4: Volleyball Smash
// Hits the opponent with a smashed volleyball, dealing insane damage,
// but because of his terrible accuracy can only hit it 50% of the time

using UnityEngine;
using System.Collections.Generic;

public class Ravi : MonoBehaviour 
{
	public List<Move> moves = new List<Move>() {
		// Bhangra Move
		new Move() {
			moveName = "Bhangra",
			moveDesc = "Dances in front of the opponent like a true chad, \nBoosts Attack by One Stage.",
			isDamaging = false,
			damage = 0,
			cooldown = 0,
			accuracy = 100,
			isHealingMove = false,
			healAmount = 0,
			isStatChange = true,
			selfAttackChange = 1,
			selfDefenseChange = 0,
			oppAttackChange = 0,
			oppDefenseChange = 0
		},

		// Equity Promise Move
		new Move() {
			moveName = "Equity Promise",
			moveDesc = "Promises some %share of his future package to the opponent, \nin exchange for Opponent losing his attack stat by 2 stages.",
			isDamaging = false,
			damage = 0,
			cooldown = 0,
			accuracy = 100,
			isHealingMove = false,
			healAmount = 0,
			isStatChange = true,
			selfAttackChange = 0,
			selfDefenseChange = 0,
			oppAttackChange = -2,
			oppDefenseChange = 0
		},

		// Alt F4 IRL Move
		new Move() {
			moveName = "Alt F4 IRL",
			moveDesc = "Uses an illegal shortcut arriving from the depths of /dev/null,\nDeals moderate damage the opponent.",
			isDamaging = true,
			damage = 20,
			cooldown = 0,
			accuracy = 80,
			isHealingMove = false,
			healAmount = 0,
			isStatChange = false,
		},

		// Volleyball Smash Move
		new Move() {
			moveName = "Volleyball Smash",
			moveDesc = "Hits the opponent with a smashed volleyball, dealing insane damage, \nbut because of his terrible accuracy can only hit it 50% of the time.",
			isDamaging = true,
			damage = 20,
			cooldown = 0,
			accuracy = 50,
			isHealingMove = false,
			healAmount = 0,
			isStatChange = false,
		}
	};

	public Move GetMoveAt(int index) {
		if (index >= 0 && index < moves.Count) {
			return moves[index];
		}
		return null;
	}
}
