// Khushal

// Move 1: Love Call
// Dials a call to his GF, and the lovey-dovey talks powers him up but also annoys the opponent.
// User gets 1 ATK with 20% HP recovery, Opposition gets 1 ATK,

// Move 2 : Shower Boy
// Comes out of the shower only wearing a towel and embarrasses the opponent. 
// Opponent's next move accuracy drops to 50%

// Move 3: Speeder Cuber
// Quickly solves his cubes and attacks the opponents using them,
// Deals low damage but has a 30% chance to flinch the opponent

// Move 4: Music Club Rage
// Plays sharp notes on his keyboard to stun the opponent then slams the keyboard into the opponent's face,
// Deals huge damage but has very low PP

// Move Messages
// Move 1: Khushal dials a call to his GF. He starts talking all lovey dovey with her which boosts his Will and Health. (opp_name) is angered by Khushal's happiness. Attack goes up by 1 on both sides. Khushal restores 20% Health!
// Move 2: Khushal you dirty dog! A towel-only Khushal has blinded (opp_name)! (opp_name)'s next move loses accuracy by 50%!
// Move 3: Speedcubing expert Khushal solves a barrage of cubes and throws them at (opp_name). (value) damage dealt!
// Move 4: Mclub Secy Khushal Wadhwa shows his Keyboard Skills to (opp_name) who boos him. Enraged, Khushal slams his keyboard straight into (opp_name)'s face. Ouch! (value) damage dealt!

using UnityEngine;
using System.Collections.Generic;

public static class Khush
{
	public static List<Move> moves = new List<Move>() {
		// Love Call Move
		new Move() {
			moveName = "Love Call",
			moveDesc = "Dials a call to his GF, and the lovey-dovey talks powers him up but also annoys the opponent.\nUser gets 1 Attack with 20% HP recovery, Opposition gets 1 Attack,",
			moveMessage = "Khushal dials a call to his GF. He starts talking all lovey dovey with her which boosts his Will and Health. (opp_name) is angered by Khushal's happiness. Attack goes up by 1 on both sides. Khushal restores 20% Health!",
			isCooldown = false,
			accuracy = 100,
			isDamaging = false,
			isHealingMove = true,
			selfHealAmount = 20,
			isStatChange = true,
			selfAttackChange = 1,
			selfDefenseChange = 0,
			oppAttackChange = 1,
			oppDefenseChange = 0
		},

		// Shower Boy Move
		new Move() {
			moveName = "Shower Boy",
			moveDesc = "Uses his silliness to lull the opponent into a false sense of security,\nLowers Defense.",
			moveMessage = "Khushal you dirty dog! A towel-only Khushal has blinded (opp_name)! (opp_name)'s next move loses accuracy by 50%!",
			isCooldown = false,
			accuracy = 100,
			isDamaging = false,
			isHealingMove = false,
			isStatChange = true,
			selfAttackChange = 0,
			selfDefenseChange = 0,
			oppAttackChange = 0,
			oppDefenseChange = -1
		},

		// Speeder Cuber Move
		new Move() {
			moveName = "Speeder Cuber",
			moveDesc = "Quickly solves his cubes and attacks the opponents using them,\nDeals low damage but has a 30% chance to flinch the opponent.",
			moveMessage = "Speedcubing expert Khushal solves a barrage of cubes and throws them at (opp_name). (value) damage dealt!",
			isCooldown = false,
			accuracy = 100,
			isDamaging = true,
			damage = 20,
			isHealingMove = false,
			isStatChange = false,
			flinch = 30
		},

		// Music Club Rage Move
		new Move() {
			moveName = "Music Club Rage",
			moveDesc = "Plays sharp notes on his keyboard to stun the opponent then slams the keyboard into the opponent's face,\nDeals huge damage but has very low PP",
			moveMessage = "Mclub Secy Khushal Wadhwa shows his Keyboard Skills to (opp_name) who boos him. Enraged, Khushal slams his keyboard straight into (opp_name)'s face. Ouch! (value) damage dealt!",
			isCooldown = false,
			accuracy = 100,
			isDamaging = true,
			damage = 20,
			isHealingMove = false,
			isStatChange = false,
		}
	};

	// Initialize function for any random/niche Move effect
	public static void Initialize() {}
}
