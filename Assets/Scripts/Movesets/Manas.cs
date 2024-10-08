// Manas

// Move 1 : Mosquito Rack-hit
// Uses his trusty mosquito racket to shock the opponent,
// Deals damage to the opponent.

// Move 2 : Silly Pookie
// Uses his silliness to lull the opponent into a false sense of security, 
// Lowers DEF of opponent by 1 stage and has a 20% flinch chance.

// Move 3: Toxic Rant
// Uses a variety of various slurs and roasts to dishearten the opponent,
// Lowers both ATK and DEF of the opponent but cannot use a move for a turn.

// Move 4: Cooler Throw
// In a fit of rage and fury about the ownership of the cooler,
// The user smashes the opponent into the cooler, dealing huge damage.

// Move Messages
// Move 1: Manas whips out his famed mosquito racket, and shocks (opp_name). Dealt (value) damage.
// Move 2: Manas activates his inner silliness. Aww look at that silly little boy :3. (opp_name) thinks this will be easy, Defense dropped by 1.
// Move 3: Manas unleashes the Bihari inside of him. Dear God! What am I even hearing? (opp_name) questions his will, Attack dropped by 1!
// Move 4: Manas remembers the debt that (opp_name) owes him for the cooler in his room. Enraged, Manas grabs a hold of (opp_name)'s hair and slams him into the cooler! (value) damage dealt!
// Recharge Turn: Manas has exhausted himself with the lethal spew of insults he threw so furiously. He can't move this turn!

using UnityEngine;
using System.Collections.Generic;

public static class Manas 
{
	public static List<Move> moves = new List<Move>() {
		// Mosquito Rack-hit Move
		new Move() {
			moveName = "Mosquito Rack-hit",
			moveDesc = "Uses his trusty mosquito racket to shock the opponent, Deals damage to the opponent.",
			moveMessage = "Manas whips out his famed mosquito racket, and shocks (opp_name). Dealt (value) damage.",
			isCooldown = false,
			accuracy = 100,
			isDamaging = true,
			damage = 50,
			isHealingMove = false,
			isStatChange = false,
		},

		// Silly Pookie Move
		new Move() {
			moveName = "Silly Pookie",
			moveDesc = "Uses his silliness to lull the opponent into a false sense of security,Lowers DEF of opponent by 1 stage and has a 20% flinch chance.",
			moveMessage = "Manas activates his inner silliness. Aww look at that silly little boy :3. (opp_name) thinks this will be easy, Defense dropped by 1.",
			isCooldown = false,
			accuracy = 100,
			isDamaging = false,
			isHealingMove = false,
			isStatChange = true,
			selfAttackChange = 0,
			selfDefenseChange = 0,
			oppAttackChange = 0,
			oppDefenseChange = -1,
			flinch = 20
		},

		// Toxic Rant Move
		new Move() {
			moveName = "Toxic Rant",
			moveDesc = "Uses a variety of various slurs and roasts to dishearten the opponent, Lowers both ATK and DEF of the opponent but cannot use a move for a turn",
			moveMessage = "Manas unleashes the Bihari inside of him. Dear God! What am I even hearing? (opp_name) questions his will, Attack dropped by 1!",
			isCooldown = true,
			accuracy = 100,
			isDamaging = false,
			isHealingMove = false,
			isStatChange = true,
			selfAttackChange = 0,
			selfDefenseChange = 0,
			oppAttackChange = -1,
			oppDefenseChange = -1
		},

		// Cooler Throw Move
		new Move() {
			moveName = "Cooler Throw",
			moveDesc = "In a fit of rage and fury about the ownership of the cooler, The user smashes the opponent into the cooler, dealing huge damage",
			moveMessage = "Manas remembers the debt that (opp_name) owes him for the cooler in his room. Enraged, Manas grabs a hold of (opp_name)'s hair and slams him into the cooler! (value) damage dealt!",
			isCooldown = false,
			accuracy = 100,
			isDamaging = true,
			damage = 95,
			isHealingMove = false,
			isStatChange = false,
		}
	};

	// Initialize function for any random/niche Move effect
	public static void Initialize() {

	}
}
