// Aditya

// Move 1: Premium Nigga
// Aditya uses the darkness to fade into the enemy's shadow causing the enemy to get chills.
// Enemy loses Attack by 1 stage.

// Move 2 : Sakht Commando
// Aditya takes off his shorts and goes Mushkil waqt Commando Sakt, the opponent is repelled by this.
// Enemy loses Defense by 1 stage and 50% to flinch them.

// Move 3 : Ramehameha
// Aditya uses his belief in Shree Ram and deals damage with a Ram powered Kamehameha.
// Deals moderate damage, double damage to muslims and heals for 15% HP.

// Move 4 : GKC BLAST
// Aditya uses his position as the GKC and uses all his brainpower to attack the opponent in a massive blast.
// Causes huge damage, but GKC loses both Attack and Defense.

// Move Messages

// Move 1: Aditya uses the darkness and fades into (opp_name)'s shadow. (opp_name) gets the chills. Attack lowered by 1.
// Move 2: Mushkil Waqt, Commando Saqt. In these hard times Aditya has no choice but to go commando. (opp_name) looks visibly disgusted. Defense dropped by 1!
// Move 3: Aditya holds his Bhagwad Gita close, he chants up a long lost Sanskrit verse and channels Lord Ram into him to attack (opp_name) with a Ramehameha! The holiness gives Aditya some energy ! (opp_name) takes (value) damage! Aditya recovers HP!
// Move 4: Aditya holds his head and starts channeling his Gyan. He starts screaming, (opp_name) looks terrified and Suddenly BOOOOM. (opp_name) takes (value) damage, Aditya loses Attack and Defense by 1 stage.

using UnityEngine;
using System.Collections.Generic;

public static class Aditi
{
	public static List<Move> moves = new List<Move>() {
		// Premium Nigga Move
		new Move() {
			moveName = "Premium Nigga",
			moveDesc = "Aditya uses the darkness to fade into the enemy's shadow causing the enemy to get chills.\nEnemy loses Attack by 1 stage.",
			moveMessage = "Aditya uses the darkness and fades into (opp_name)'s shadow. (opp_name) gets the chills. Attack lowered by 1.",
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

		// Sakht Commando Move
		new Move() {
			moveName = "Sakht Commando",
			moveDesc = "Aditya takes off his shorts and goes Mushkil waqt Commando Sakt, the opponent is repelled by this.\nEnemy loses Defense by 1 stage and 50% to flinch them.",
			moveMessage = "Mushkil Waqt, Commando Saqt. In these hard times Aditya has no choice but to go commando. (opp_name) looks visibly disgusted. Defense dropped by 1!",
			isCooldown = false,
			accuracy = 100,
			isDamaging = false,
			isHealingMove = false,
			isStatChange = true,
			selfAttackChange = 0,
			selfDefenseChange = 0,
			oppAttackChange = 0,
			oppDefenseChange = -1,
			flinch = 50
		},

		// Ramehameha Move
		new Move() { 
			moveName = "Ramehameha",
			moveDesc = "Aditya uses his belief in Shree Ram and deals damage with a Ram powered Kamehameha.\nDeals moderate damage, double damage to muslims and heals for 15% HP.",
			moveMessage = "Aditya holds his Bhagwad Gita close, he chants up a long lost Sanskrit verse and channels Lord Ram into him to attack (opp_name) with a Ramehameha! The holiness gives Aditya some energy ! (opp_name) takes (value) damage! Aditya recovers HP!",
			isCooldown = false,
			accuracy = 100,
			isDamaging = true,
			damage = 20,
			isHealingMove = true,
			healAmount = 15,
			isStatChange = false,
		},

		// GKC BLAST Move
		new Move() {
			moveName = "GKC BLAST",
			moveDesc = "Aditya uses his position as the GKC and uses all his brainpower to attack the opponent in a massive blast.\nCauses huge damage, but GKC loses both Attack and Defense.",
			moveMessage = "Aditya holds his head and starts channeling his Gyan. He starts screaming, (opp_name) looks terrified and Suddenly BOOOOM. (opp_name) takes (value) damage, Aditya loses Attack and Defense by 1 stage.",
			isCooldown = false,
			accuracy = 100,
			isDamaging = true,
			damage = 20,
			isHealingMove = false,
			isStatChange = true,
			selfAttackChange = -1,
			selfDefenseChange = -1,
			oppAttackChange = 0,
			oppDefenseChange = 0,
		}
	};

	// Initialize function for any random/niche Move effect
	public static void Initialize() {}
}