// Vrushabh 

// Move 1: Dosa Undri
// Vrushabh Anna summons the power of the Rava Masala Dosa to recover his energy and spirit.
// Recovers 25% of total HP.

// Move 2 : Sober Fren
// Vrushabh gets the opponent to drink, and stays with him as the sober support as the opponent breaks down because of their shitty life.
// Opponent loses ATK and DEF by 1 Stage but also Restores 10% of Total HP because of the alcohol. 

// Move 3 : RCBakchod (BP :- 70)
// Vrushabh gathers his trauma of being a RCB fan into his tears and sprays these toxic drops onto the opponent.
// Deals moderate damage to the opponent.

// Move 4 : Proper All Rounder (BP :- 90)
// Vrushabh uses his superior prowess in every sport and combines them to attack the opponent with a random sport equipment.
// Deals good damage, has a 50% chance for a random effect (-1 ATK or -1 DEF or Flinch) but also has a 25% chance to just fail completely.

using UnityEngine;
using System.Collections.Generic;

public static class Vrush
{
	public static List<Move> moves = new List<Move>() {
		// Dosa Undri Move
		new Move() {
			moveName = "Dosa Undri",
			moveDesc = "Vrushabh Anna summons the power of the Rava Masala Dosa to recover his energy and spirit. Recovers 25% of total HP.",
			moveMessage = "The absolute authority mommy has appeared on the battlefield. (opp_name) shudders. Sarvesh asks (opp_name) to lower his attack and defense and he gladly complies. (opp_name) loses Attack and Defense by 1 stage!",
			isCooldown = false,
			accuracy = 100,
			isDamaging = false,
			isHealingMove = true,
            selfHealAmount = 25,
			isStatChange = false
		},

		// Sober Fren Move
		new Move() {
			moveName = "Sober Fren",
			moveDesc = "Vrushabh gets the opponent to drink, and stays with him as the sober support as the opponent breaks down because of their shitty life. Opponent loses ATK and DEF by 1 Stage but also Restores 10%.",
			moveMessage = "Sarvesh spots an unclaimed high land and quickly rushes to it and climbs it, (opp_name) watches in silence as Sarvesh takes a rest. 75% HP recovered, Sarvesh can't attack next turn!!",
			isCooldown = false,
			accuracy = 100,
			isDamaging = false,
			isHealingMove = true,
			oppHealAmount = 10,
			isStatChange = true,
            selfAttackChange = 0,
			selfDefenseChange = 0,
			oppAttackChange = -1,
			oppDefenseChange = -1
		},

		// RCBakchod Move
		new Move() { 
			moveName = "RCBakchod",
			moveDesc = "Vrushabh gathers his trauma of being a RCB fan into his tears and sprays these toxic drops onto the opponent.  Deals moderate damage to the opponent.",
			moveMessage = "Sarvesh takes a huge breath and starts preparing a massive shout. (opp_name) raises his guard. Suddenly Sarvesh shouts WOOOOW with full power and deafens (opp_name). (value) damage dealt.",
			isCooldown = false,
			accuracy = 100,
			isDamaging = true,
			damage = 70,
			isHealingMove = false,
			isStatChange = false
		},

		// Churan Heatwave Move
		new Move() {
			moveName = " Proper All Rounder",
			moveDesc = "Vrushabh uses his superior prowess in every sport and combines them to attack the opponent with a random sport equipment. Deals good damage, has a 50% chance for a random effect (-1 ATK or -1 DEF or Flinch) but also has a 25% chance to just fail completely.",
			moveMessage = "Sarvesh ingests the special Churan bestowed to him by his ancestors. He feels the heat inside him bubbling up and in a moment, the heat releases in a huge blast! (opp_name) takes (value) damage!!",
			isCooldown = false,
			accuracy = 75,
			isDamaging = true,
			damage = 90,
			isHealingMove = false,
			isStatChange = true,
			oppDefenseChange = 0, // Initialize Defense change to 0
		}
	};

	// Initialize function for any random/niche Move effect
	public static void Initialize() {
        moves[3].oppDefenseChange = 0;
        moves[3].oppAttackChange = 0;
        moves[3].flinch = 0;

		int x = Random.Range(1, 100);
		if (x <= 17) {
			moves[3].oppDefenseChange = -1;
		} else if (x <= 34) {
            moves[3].oppAttackChange = -1;
        } else if (x <= 51) {
            moves[3].flinch = 100;
        }
	}
}