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

// Move Messages
// Move 1: Ayyo Baba!, Ay Vrushabh Anna is summoning the power of a Rava Masala Dosa ra. He looks ready to take (opp_name) to Maleshwaram Area and pop him with his gang. 25% HP Recovered!
// Move 2: Vrushabh pulls out a Bottle of Alcohol and pours a drink to (opp_name). (opp_name) drinks the alcohol, Recovers 15% HP. But what's this ? He is breaking down in front of Vrushabh. Alcohol truly brings out the true self of people. (opp_name) loses Attack and Defense by 1 Stage.
// Move 3: Royally Challenged Vrushabh remembers all the trauma that RCB has put him through and starts gathering it into his tears. (opp_name) hears faint sounds of "Ee Sala Cup Namde". Suddenly Vrushabh throws his toxic tears onto (opp_name), burning him!. (value) damage dealt!
// Move 4: Sporty Boy Vrushabh D Undri channels up his skills in various sports inside him. Suddenly he summons a...
// (Hits, but no effect) Basketball and slams it onto the opponent. (value) damage dealt!
// (Hits and Effect) His magical cricket bat and ball plays a chaotic short ball straight onto (opp_name)'s face. The magic of the bat has left a random debuff on (opp_name)! 
// (Fail) Kabaddi Mat! Vrushabh starts muttering Kabaddi and gets a sliver of touch on (opp_name). Good news for him is that he gets a point!!!, Bad news is that a touch doesn't really hurt (opp_name), 0 damage dealt

using UnityEngine;
using System.Collections.Generic;

public static class Vrush
{
	public static List<Move> moves = new List<Move>() {
		// Dosa Undri Move
		new Move() {
			moveName = "Dosa Undri",
			moveDesc = "Vrushabh Anna summons the power of the Rava Masala Dosa to recover his energy and spirit. Recovers 25% of total HP.",
			moveMessage = "Ayyo Baba!, Ay Vrushabh Anna is summoning the power of a Rava Masala Dosa ra. He looks ready to take (opp_name) to Maleshwaram Area and pop him with his gang. 25% HP Recovered!",
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
			moveMessage = "Vrushabh pulls out a Bottle of Alcohol and pours a drink to (opp_name). (opp_name) drinks the alcohol, Recovers 15% HP. But what's this ? He is breaking down in front of Vrushabh. Alcohol truly brings out the true self of people. (opp_name) loses Attack and Defense by 1 Stage.",
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
			moveMessage = "Royally Challenged Vrushabh remembers all the trauma that RCB has put him through and starts gathering it into his tears. (opp_name) hears faint sounds of \"Ee Sala Cup Namde\". Suddenly Vrushabh throws his toxic tears onto (opp_name), burning him!. (value) damage dealt!",
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
			moveMessage = "Sporty Boy Vrushabh D Undri channels up his skills in various sports inside him. Suddenly he summons a... Basketball and slams it onto the opponent. (value) damage dealt!",
			missMessage = "Sporty Boy Vrushabh D Undri channels up his skills in various sports inside him. Suddenly he summons a... Kabaddi Mat! Vrushabh starts muttering Kabaddi and gets a sliver of touch on (opp_name). Good news for him is that he gets a point!!!, Bad news is that a touch doesn't really hurt (opp_name), 0 damage dealt",
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
		moves[3].moveMessage = "Sporty Boy Vrushabh D Undri channels up his skills in various sports inside him. Suddenly he summons a...";
        moves[3].oppDefenseChange = 0;
        moves[3].oppAttackChange = 0;
        moves[3].flinch = 0;

		int x = Random.Range(1, 100);
		if (x <= 17) {
			moves[3].oppDefenseChange = -1;
			moves[3].moveMessage += " Magical cricket bat and ball plays a chaotic short ball straight onto (opp_name)'s face. The magic of the bat has left a random debuff on (opp_name)!";
		} else if (x <= 34) {
            moves[3].oppAttackChange = -1;
			moves[3].moveMessage += " Magical cricket bat and ball plays a chaotic short ball straight onto (opp_name)'s face. The magic of the bat has left a random debuff on (opp_name)!";
        } else if (x <= 51) {
            moves[3].flinch = 100;
			moves[3].moveMessage += " Basketball and slams it onto the opponent. (value) damage dealt!";
        } else {
			moves[3].moveMessage += " Basketball and slams it onto the opponent. (value) damage dealt!";
		}
	}
}