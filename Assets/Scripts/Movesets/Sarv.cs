// Sarvesh

// Move 1: Domi Mommy
// As the dom mommy of the group, Sarvesh holds absolute authority over the opponent.
// Asks the opponent to lower both their Attack and Defence by 1 and the opponent gladly accepts.

// Move 2: Veteran Climber
// Sarvesh uses his veteran climbing abilities and takes the high ground and rests,
// Sarvesh recovers 75% of total HP but is unable to attack the next turn.

// Move 3: WOOOOOW
// Sarvesh gathers up air in his chest and with full volume screams WOOOOOOW into the opponent's ear.
// Deals damage, 20% flinch chance.

// Move 4: Churan Heatwave
// Sarvesh ingests his special Churan passed onto him by his ancestors and releases a massive heatwave which damages the opponent.
// Deals good damage and has a 50% chance to lower enemy's Defense.

// Move Messages
// Move 1: The absolute authority mommy has appeared on the battlefield. (opp_name) shudders. Sarvesh asks (opp_name) to lower his attack and defense and he gladly complies. (opp_name) loses Attack and Defense by 1 stage!
// Move 2: Sarvesh spots an unclaimed high land and quickly rushes to it and climbs it, (opp_name) watches in silence as Sarvesh takes a rest. 75% HP recovered, Sarvesh can't attack next turn!!
// Move 3: Sarvesh takes a huge breath and starts preparing a massive shout. (opp_name) raises his guard. Suddenly Sarvesh shouts WOOOOW with full power and deafens (opp_name). (value) damage dealt.
// Move 4: Sarvesh ingests the special Churan bestowed to him by his ancestors. He feels the heat inside him bubbling up and in a moment, the heat releases in a huge blast! (opp_name) takes (value) damage!! 
// Recharge Turn: Sarvesh is getting off the high ground, he can't attack this turn!

using UnityEngine;
using System.Collections.Generic;

public static class Sarv
{
	public static List<Move> moves = new List<Move>() {
		// Domi Mommy Move
		new Move() {
			moveName = "Domi Mommy",
			moveDesc = "As the dom mommy of the group, Sarvesh holds absolute authority over the opponent.\nAsks the opponent to lower both their Attack and Defence by 1 and the opponent gladly accepts.",
			moveMessage = "The absolute authority mommy has appeared on the battlefield. (opp_name) shudders. Sarvesh asks (opp_name) to lower his attack and defense and he gladly complies. (opp_name) loses Attack and Defense by 1 stage!",
			isCooldown = false,
			accuracy = 100,
			isDamaging = false,
			isHealingMove = false,
			isStatChange = true,
			selfAttackChange = 0,
			selfDefenseChange = 0,
			oppAttackChange = -1,
			oppDefenseChange = -1
		},

		// Veteran Climber Move
		new Move() {
			moveName = "Veteran Climber",
			moveDesc = "Sarvesh uses his veteran climbing abilities and takes the high ground and rests,\nSarvesh recovers 75% of total HP but is unable to attack the next turn.",
			moveMessage = "Sarvesh spots an unclaimed high land and quickly rushes to it and climbs it, (opp_name) watches in silence as Sarvesh takes a rest. 75% HP recovered, Sarvesh can't attack next turn!!",
			isCooldown = true,
			accuracy = 100,
			isDamaging = false,
			isHealingMove = true,
			healAmount = 75,
			isStatChange = false,
		},

		// WOOOOOW Move
		new Move() { 
			moveName = "WOOOOOW",
			moveDesc = "Sarvesh gathers up air in his chest and with full volume screams WOOOOOOW into the opponent's ear.\nDeals damage, 20% flinch chance.",
			moveMessage = "Sarvesh takes a huge breath and starts preparing a massive shout. (opp_name) raises his guard. Suddenly Sarvesh shouts WOOOOW with full power and deafens (opp_name). (value) damage dealt.",
			isCooldown = false,
			accuracy = 100,
			isDamaging = true,
			damage = 20,
			isHealingMove = false,
			isStatChange = false,
		},

		// Churan Heatwave Move
		new Move() {
			moveName = "Churan Heatwave",
			moveDesc = "Sarvesh ingests his special Churan passed onto him by his ancestors and releases a massive heatwave which damages the opponent.\nDeals good damage and has a 50% chance to lower enemy's Defense.",
			moveMessage = "Sarvesh ingests the special Churan bestowed to him by his ancestors. He feels the heat inside him bubbling up and in a moment, the heat releases in a huge blast! (opp_name) takes (value) damage!!",
			isCooldown = false,
			accuracy = 100,
			isDamaging = true,
			damage = 20,
			isHealingMove = false,
			isStatChange = true,
			oppDefenseChange = 0, // Initialize Defense change to 0
		}
	};

	// Initialize function for any random/niche Move effect
	public static void Initialize() {
		int x = Random.Range(0, 100);
		Debug.Log(x);
		if (x < 50) {
			moves[3].moveMessage = "Sarvesh ingests the special Churan bestowed to him by his ancestors. He feels the heat inside him bubbling up and in a moment, the heat releases in a huge blast! (opp_name) takes (value) damage!! (opp_name) gets burnt by the heatwave! Defense dropped by 1!";
			moves[3].oppDefenseChange = -1;
		}
	}
}