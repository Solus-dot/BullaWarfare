// Daksh

// Move 1: Mogger
// Daksh Mogger whips out his killer jawline and flexes in front of the opponent. Its so over for the opponent!,
// Reduces opponent ATK by 1 stage.

// Move 2: Anything For Laddu
// Daksh whips out his revered Besan ke Laddu that he brought from home and offers it to the opponent. The opponent gladly agrees and eats the savory sweet gladly, but heavily loses his guard in the process.
// Opponent Heals 15% of total HP, but loses DEF by 2 stages.

// Move 3: Dunk on'em Haters
// Daksh uses his basketball skills and dunks a heavy basketball straight to the opponent's face.
// Deals moderate damage to the opponent.

// Move 4: Ghaziabadi Glocker
// Daksh whips out a gun and shoots the opponent straight up, He hasn't actually taken any shooting lessons so his shots aren't straight and takes recoil.
// Only 75% accurate and 20% recoil but deals Massive Damage.

// Move Messages
// Move 1: Mogger Daksh starts mewing and flexes on (opp_name). BYE BYE! Its so over for (opp_name). Attack dropped by 1!
// Move 2: The sweet and savory smell of Daksh's Laddus have enveloped the arena. (opp_name) gladly takes the Laddus offered by Daksh and heavily loses his guard! (opp_name) recovers HP, Defense dropped by 2!
// Move 3: Daksh Pratap Jordan whips out his basketball and dribbles his way to (opp_name). (opp_name) looks ready to block him but is left surprised when Daksh just straight up dunks the ball into his head! (value) damage dealt.
// Move 4: Ghaziabad's very own whips out his family heirloom, a fucking gun and shoots towards (opp_name)!
// (If Hits) The bullet hits (opp_name) straight in the chest! Yikes, that's gotta hurt, (value) damage dealt. Daksh also takes recoil damage from the gun. 
// (If Miss) And the bullet misses (opp_name) completely! Perhaps Daksh should invest in shooting lessons. No damage dealt, Daksh takes recoil!

using UnityEngine;
using System.Collections.Generic;

public static class Daksh
{
	public static List<Move> moves = new List<Move>() {
		// Mogger Move
		new Move() {
			moveName = "Mogger",
			moveDesc = "Daksh Mogger whips out his killer jawline and flexes in front of the opponent. Its so over for the opponent!,\nReduces opponent ATK by 1 stage.",
			moveMessage = "Mogger Daksh starts mewing and flexes on (opp_name). BYE BYE! Its so over for (opp_name). Attack dropped by 1!",
			isCooldown = false,
			accuracy = 100,
			isDamaging = false,
			isHealingMove = false,
			isStatChange = true,
			selfAttackChange = 0,
			selfDefenseChange = 0,
			oppAttackChange = -1,
			oppDefenseChange = 0
		},

		// Anything For Laddu Move
		new Move() {
			moveName = "Anything For Laddu",
			moveDesc = "Daksh whips out his revered Besan ke Laddu that he brought from home and offers it to the opponent. The opponent gladly agrees and eats the savory sweet gladly, but heavily loses his guard in the process. Opponent Heals 15% of total HP, but loses DEF by 2 stages.",
			moveMessage = "The sweet and savory smell of Daksh's Laddus have enveloped the arena. (opp_name) gladly takes the Laddus offered by Daksh and heavily loses his guard! (opp_name) recovers HP, Defense dropped by 2!",
			isCooldown = false,
			accuracy = 100,			
			isDamaging = false,
			isHealingMove = true,
			oppHealAmount = 15,
			isStatChange = true,
			selfAttackChange = 0,
			selfDefenseChange = 0,
			oppAttackChange = 0,
			oppDefenseChange = -2
		},

		// Dunk on'em Haters Move
		new Move() {
			moveName = "Dunk on'em Haters",
			moveDesc = "Daksh uses his basketball skills and dunks a heavy basketball straight to the opponent's face.\nDeals moderate damage to the opponent.",
			moveMessage = "Daksh Pratap Jordan whips out his basketball and dribbles his way to (opp_name). (opp_name) looks ready to block him but is left surprised when Daksh just straight up dunks the ball into his head! (value) damage dealt.",
			isCooldown = false,
			accuracy = 100,
			isDamaging = true,
			damage = 20,
			isHealingMove = false,
			isStatChange = false,
		},

		// Ghaziabadi Glocker Move
		new Move() {
			moveName = "Ghaziabadi Glocker",
			moveDesc = "Daksh whips out a gun and shoots the opponent straight up, He hasn't actually taken any shooting lessons so his shots aren't straight and takes recoil.\nOnly 75% accurate and 20% recoil but deals Massive Damage.",
			moveMessage = "Ghaziabad's very own whips out his family heirloom, a fucking gun and shoots towards (opp_name)! The bullet hits (opp_name) straight in the chest! Yikes, that's gotta hurt, (value) damage dealt. Daksh also takes recoil damage from the gun.",
			missMessage = "Ghaziabad's very own whips out his family heirloom, a fucking gun and shoots towards (opp_name)! And the bullet misses (opp_name) completely! Perhaps Daksh should invest in shooting lessons. No damage dealt, Daksh takes recoil!",
			accuracy = 75,
			isDamaging = true,
			damage = 20,
			isHealingMove = false,
			isStatChange = false,
			recoil = 0.2f
		}
	};

	// Initialize function for any random/niche Move effect
	public static void Initialize() {}
}
