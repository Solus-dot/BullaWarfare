// Aryavart

// Move 1: Shudh Dudh
// Pulls out his 2 Litre bottle of pure buffallo milk and drinks it one gulp, 
// ATK and DEF up by 1 stage.

// Move 2: Puppy Eyes 
// Says "Aisa karega mere saath" in a very sweet voice,
// Lowers ATK of opponent by one stage.

// Move 3: Baddieminton
// Straightforward move where he uses his badminton racket to attack the opponent,
// Deals moderate damage to opponent.

// Move 4: Feminine Jaat
// Calls upon the Jaat gang of IITK using his "Feminine Jaat" powers,
// Deals MASSSIVE damage, but causes 35% of hp recoil damage to the user

// Move Messages
// Move 1: Aryavart pulls out his huge bottle of Shudh Desi Dudh and drinks it one gulp. (opp_name) is left ineffable. Aryavart's Attack and Defense go up by 1 stage!
// Move 2: (opp_name) says a bad thing to Aryavart, Aryavart responds by "Aisa krega mere saath" and (opp_name) instantly feels regret. Attack dropped by 1 stage.
// Move 3: Fresher's Inferno Champion Aryavart pulls out his badminton rackets and hits (opp_name) in the face. (value) damage dealt!
// Move 4: Arya calls up his fellow Jaats saying "Matter Hogaya". The full Jaat gang of IITK appears on the battlefield, and beats the shit out of (opp_name). Confusion causes Aryavart to take damage as well. (value) damage dealt, Aryavart receives recoil!

using UnityEngine;
using System.Collections.Generic;

public static class Arya
{
    public static List<Move> moves = new List<Move>() {
		// Shudh Dudh Move
		new Move() {
			moveName = "Shudh Dudh",
            moveDesc = "Pulls out his 2 Litre bottle of pure buffallo milk and drinks it one gulp, \nAttack and Defense up by 1 stage.",
			moveMessage = "Aryavart pulls out his huge bottle of Shudh Desi Dudh and drinks it one gulp. (opp_name) is left ineffable. Aryavart's Attack and Defense go up by 1 stage!",
			isCooldown = false,
			accuracy = 100,
			isDamaging = false,
			isHealingMove = false,
			isStatChange = true,
			selfAttackChange = 1,
			selfDefenseChange = 1,
			oppAttackChange = 0,
			oppDefenseChange = 0
		},

		// Puppy Eyes Move
		new Move() {
			moveName = "Puppy Eyes",
			moveDesc = "Says \"Aisa karega mere saath\" in a very sweet voice,\n Lowers Attack of opponent by one stage.",
			moveMessage = "(opp_name) says a bad thing to Aryavart, Aryavart responds by \"Aisa krega mere saath\" and (opp_name) instantly feels regret. Attack dropped by 1 stage.",
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

		// Baddieminton Move
		new Move() { 
			moveName = "Baddieminton",
			moveDesc = "Straightforward move where he uses his badminton racket to attack the opponent,\ndeals moderate damage to opponent.",
			moveMessage = "Fresher's Inferno Champion Aryavart pulls out his badminton rackets and hits (opp_name) in the face. (value) damage dealt!.",
			isCooldown = false,
			accuracy = 100,
			isDamaging = true,
            damage = 20,
			isHealingMove = false,
			isStatChange = false,
		},

		// Feminine Jaat Move
		new Move() {
			moveName = "Feminine Jaat",
			moveDesc = " Calls upon the Jaat gang of IITK using his Feminine Jaat powers,\nDeals MASSSIVE damage, but causes 35% recoil damage to the user.",
			moveMessage = "Arya calls up his fellow Jaats saying \"Matter Hogaya\". The full Jaat gang of IITK appears on the battlefield, and beats the shit out of (opp_name). Confusion causes Aryavart to take damage as well. (value) damage dealt, Aryavart receives recoil!",
			isCooldown = false,
			accuracy = 100,
			isDamaging = true,
			damage = 20,
			isHealingMove = false,
			isStatChange = false,
            recoil = 0.35f
		}
	};

	// Initialize function for any random/niche Move effect
	public static void Initialize() {}
}