// Ravi 

// Move 1: Bhangra 
// Dances in front of the opponent like a true chad,
// Boosts Attack by one stage.

// Move 2: Equity Promise
// Promises some %share of his future package to the opponent 
// In exchange for Opponent losing his Attack stat by 2 stages. Can only be used once per opponent.

// Move 3: Alt F4 IRL
// Uses an illegal shortcut arriving from the depths of /dev/null,
// Deals moderate damage the opponent.

// Move 4: Volleyball Smash
// Hits the opponent with a smashed volleyball, dealing insane damage,
// But because of his terrible accuracy can only hit it 50% of the time.

//Move Messages
// Move 1: Ravi drinks a gallon of lassi, and starts dancing to a Daler Mehendi song, (opp_name) is flabbergasted while Ravi's dance boosts up his confidence, Ravi's attack goes up by 1 stage !
// Move 2: Ravi pulls out his placement offers and shows it to (opp_name). Infatuated, begs Ravi for 5% of his offer. Ravi agrees and the opponent is left with a stable future in exchange for all their ego. (opp_name) loses Attack by 2 stages!
// Move 3: Ravi takes out his illegal Linux laptop, and uses a bizarre shortcut key to hack into (opp_name)'s body. (opp_name) receives (value) damage.
// Move 4: Ravi whips out a volleyball, ready to perform a devastating jump serve. Sweat starts pouring out of (opp_name). Ravi jumps in the air and...
// (if hits) Hits (opp_name) right in the face! (opp_name) receives (value) damage.
// (if miss) and completely MISSES it. (opp_name) laughs at Ravi's impotence.

using UnityEngine;
using System.Collections.Generic;

public static class Ravi
{
	public static List<Move> moves = new List<Move>() {
		// Bhangra Move
		new Move() {
			moveName = "Bhangra",
			moveDesc = "Dances in front of the opponent like a true chad, \nBoosts Attack by One Stage.",
			moveMessage = "Ravi starts dancing to a Daler Mehendi song, (opp_name) is flabbergasted while Ravi's dance boosts up his confidence, Ravi's attack goes up 1 stage!",
			cooldown = 0,
			accuracy = 100,
			isDamaging = false,
			isHealingMove = false,
			isStatChange = true,
			selfAttackChange = 1,
			selfDefenseChange = 0,
			oppAttackChange = 0,
			oppDefenseChange = 0
		},

		// Equity Promise Move
		new Move() {
			moveName = "Equity Promise",
			moveDesc = "Promises some %share of his future package to the opponent, \nIn exchange for Opponent losing his attack stat by 2 stages.",
			moveMessage = "Ravi pulls out his placement offers and shows it to (opp_name). Infatuated, begs Ravi for 5% of his offer. Ravi agrees and the opponent is left with a stable future in exchange for their ego. (opp_name) loses Attack by 2 stages!",
			cooldown = 0,
			accuracy = 100,			
			isDamaging = false,
			isHealingMove = false,
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
			moveMessage = "Ravi takes out his illegal Linux laptop, and uses a bizarre shortcut key to hack into (opp_name)'s body. (opp_name) receives (value) damage.",
			cooldown = 0,
			accuracy = 100,
			isDamaging = true,
			damage = 20,
			isHealingMove = false,
			isStatChange = false,
		},

		// Volleyball Smash Move
		new Move() {
			moveName = "Volleyball Smash",
			moveDesc = "Hits the opponent with a smashed volleyball, dealing insane damage, \nBut because of his terrible accuracy can only hit it 50% of the time.",
			moveMessage = "Ravi whips out a volleyball, ready to perform a devastating jump serve. Sweat starts pouring out of (opp_name). Ravi jumps in the air and... Hits (opp_name) right in the face! (opp_name) receives (value) damage.",
			missMessage = "Ravi whips out a volleyball, ready to perform a devastating jump serve. Sweat starts pouring out of (opp_name). Ravi jumps in the air and... and completely MISSES it. (opp_name) laughs at Ravi's impotence.",
			cooldown = 0,
			accuracy = 50,
			isDamaging = true,
			damage = 20,
			isHealingMove = false,
			isStatChange = false,
		}
	};
}
