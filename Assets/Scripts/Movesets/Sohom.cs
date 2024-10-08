// Sohom 

// Move 1: Chokehold
// Comes sneakily behind the opponent and chokes them for some damage, 
// Causes opponent to lose DEF stat.

// Move 2: MCDonel
// Orders Mcdonald's in the middle of the fight, 
// Heals 30% of total HP.

// Move 3: Brainrot Spew
// Spews insane brainrot phrases at the opponent giving them brain damaged, 
// Causes the opponent to lose his ATK by 1 stage and 40% flinch chance.

// Move 4: Mr Fresher Smash
// Uses his status as Mr Fresher to summon hordes of fangirls to attack the opponent, 
// Deals massive damage but needs to be recharged after used.

//Move Messages
// Move 1: Sohom quickly sneaks behind (opp_name) and puts him in a chokehold. (opp_name) receives (value) damage. Defense dropped by 1!
// Move 2: (move_use) Sohom opens Zomato on his phone, Perhaps he's ordering something ? (move_eff) Sohom's order has arrived, It was mcdonel ! Sohom completely devours his meal, leaving no trace of and recovers HP.
// Move 3: Erm what the sigma? Sohom has started to spew filthy brainrot words to his opponent! The opponent does not have the skibidi rizz to handle it, (opp_name) loses Attack by 1 stage.
// Move 4: Sohom unveils his Mr Fresher sash. All the XX chromosomes around him get riled up and attack (opp_name). (opp_name) receives (value) damage. Sohom starts taking pictures with the wild females to calm them down!
// Recharge Turn: Sohom is taking pictures with the females, he can't move!!!

using UnityEngine;
using System.Collections.Generic;

public static class Sohom
{
	public static List<Move> moves = new List<Move>() {
		// Chokehold Move
		new Move() {
			moveName = "Chokehold",
			moveDesc = "Comes up sneakily behind the opponent and chokes them for some damage, Causes the opponent to lose DEF.",
			moveMessage = "Sohom quickly sneaks behind (opp_name) and puts him in a chokehold. (opp_name) receives (value) damage. Defense dropped by 1!",
			isCooldown = false,
			accuracy = 100,
			isDamaging = true,
			damage = 60,
			isHealingMove = false,
			isStatChange = true,
			selfAttackChange = 0,
			selfDefenseChange = 0,
			oppAttackChange = 0,
			oppDefenseChange = -1,
		},

		// MCDonel Move
		new Move() {
			moveName = "MCDonel",
			moveDesc = "Orders Mcdonald's in the middle of the fight, Heals 30% of total HP.",
			moveMessage = "Sohom opens Zomato on his phone, Perhaps he's ordering something? Sohom's order has arrived, It was McDonel! Sohom completely devours his meal, leaving no trace of and recovers HP.",
			isCooldown = false,
			accuracy = 100,
			isDamaging = false,
			isHealingMove = true,
			selfHealAmount = 30,
			isStatChange = false,
		},

		// Brainrot Spew Move
		new Move() { 
			moveName = "Brainrot Spew",
			moveDesc = "Spews insane brainrot phrases at the opponent, Causes the opponent to lose his ATK by 1 stage and 40% flinch chance.",
			moveMessage = "Erm what the sigma? Sohom has started to spew filthy brainrot words to his opponent! (opp_name) does not have the skibidi rizz to handle it, (opp_name) loses Attack by 1 stage.",
			isCooldown = false,
			accuracy = 100,
			isDamaging = false,
			isHealingMove = false,
			isStatChange = true,
			selfAttackChange = 0,
			selfDefenseChange = 0,
			oppAttackChange = -1,
			oppDefenseChange = 0,
			flinch = 40

		},

		// Mr Fresher Smash Move
		new Move() {
			moveName = "Mr. Fresher Smash",
			moveDesc = "Uses his status as Mr. Fresher to summon hordes of fangirls to attack the opponent, Deals massive damage but needs to be recharged after used.",
			moveMessage = "Sohom unveils his Mr Fresher sash. All the girls around him get riled up and attack (opp_name) dealing (value) damage. Sohom starts taking pictures with the wild females to calm them down.",
			isCooldown = true,
			accuracy = 100,
			isDamaging = true,
			damage = 135,
			isHealingMove = false,
			isStatChange = false,
		}
	};

	// Initialize function for any random/niche Move effect
	public static void Initialize() {}
}
