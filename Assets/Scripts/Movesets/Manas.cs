// Manas

// Move 1 : Mosquito Rack-hit
// Uses his trusty mosquito racket to shock the opponent,
// Deals damage to the opponent

// Move 2 : Silly Pookie
// Uses his silliness to lull the opponent into a false sense of security, 
// Lowers Defense.

// Move 3: Toxic Rant
// Uses a variety of various slurs and roasts to dishearten the opponent,
// Lowers Attack. If used after Silly Pookie, lowers both Attack and Defence

// Move 4: Cooler Throw
// In a fit of rage and fury about the ownership of the cooler,
// the user smashes the opponent into the cooler, dealing huge damage

using UnityEngine;
using System.Collections.Generic;

public class Manas : MonoBehaviour 
{
	public List<Move> moves = new List<Move>() {
		// Mosquito Rack-hit Move
		new Move() {
			moveName = "Mosquito Rack-hit",
			moveDesc = "Uses his trusty mosquito racket to shock the opponent,\nDeals damage to the opponent.",
			cooldown = 0,
			accuracy = 100,
			isDamaging = true,
			damage = 20,
			isHealingMove = false,
			isStatChange = false,
		},

		// Silly Pookie Move
		new Move() {
			moveName = "Silly Pookie",
			moveDesc = "Uses his silliness to lull the opponent into a false sense of security,\nLowers Defense.",
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

		// Toxic Rant Move
		new Move() {
			moveName = "Toxic Rant",
			moveDesc = "Uses a variety of various slurs and roasts to dishearten the opponent,\nLowers Attack.",
			cooldown = 0,
			accuracy = 100,
			isDamaging = false,
			isHealingMove = false,
			isStatChange = true,
			selfAttackChange = 0,
			selfDefenseChange = 0,
			oppAttackChange = -1,
			oppDefenseChange = 0
		},

		// Cooler Throw Move
		new Move() {
			moveName = "Cooler Throw",
			moveDesc = "In a fit of rage and fury about the ownership of the cooler,\nthe user smashes the opponent into the cooler, dealing huge damage",
			cooldown = 0,
			accuracy = 100,
			isDamaging = true,
			damage = 20,
			isHealingMove = false,
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
