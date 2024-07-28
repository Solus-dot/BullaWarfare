// Himanshu

// Move 1: Dangerous Drink
// Himanshu makes up a disgusting concoction of a Red Bull, Frappe and Russian Vodka and gulps it in one go.
// The pure amount of energy gives Himanshu a +3 ATK Boost!, but he loses 33% of his Total HP because of the damage to his liver.

// Move 2: Memories, Bro!
// Himanshu reminds the opponent of his small and fleeting life and reminds them how they don't have any memorable moments in their life.
// The opponent is disheartened by this and loses ATK by 1 stage.

// Move 3: Gambling is Fun
// Himanshu offers a diabolical gamble to the opponent, a coin toss,
// whose winner heals 15% of his total HP. Little does the opponent know it is a biased coin to favour himself.

// Move 4 : Master(de)bater (65 BP) 
// Himanshu uses his excellent master(de)bating skills and convinces the opponent to hurt himself.
// Deals moderate damage to the opponent.

// Move Messages
// Move 1: Himanshu takes out his steel glass and starts pouring a full can of Red Bull, a Frappe from CCD and 100% original Russian Vodka. (opp_name) is left speechless when Himanshu drinks up this drink in one gulp. Though he looks pale, Himanshu's body is flowing with energy! Attack boosted by 3 stages! Himanshu loses HP!
// Move 2: Himanshu approaches (opp_name) and says to them "Yaar, matlab teri life mai koi memories hi nahi hai, bro". (opp_name) is disheartened because of Himanshu's words. Attack dropped by 1 stage!
// Move 3: Himanshu switches on his gambling mode, approaching the opponent with a coin. The rules are simple, the winner gets a free oreo shake from the loser. The coin goes in the air and.......
// (If Himanshu wins) Himanshu has done it, he has done it, he won a free Oreo Shake ! He thinks to add it into his dangerous drink but for now just drinks it on its own. HP Recovered!
// (If Opp Wins) (opp_name) has done it! He has won a free oreo shake of Himanshu. This is why you don't gamble kids. (opp_name) recovers HP!
// Move 4: Himanshu the master (de)bater, starts a debate with (opp_name). THAT (opp_name) should hurt himself. Himanshu goes on a long ramble and makes up a solid argument for the House. (opp_name) realises he can't win this debate so he just complies and hurts himself. (value) damage dealt!

using UnityEngine;
using System.Collections.Generic;

public static class Hima
{
	public static List<Move> moves = new List<Move>() {
		// Dangerous Drink Move
		new Move() {
			moveName = "Dangerous Drink",
			moveDesc = "Himanshu makes up a disgusting concoction of a Red Bull, Frappe and Russian Vodka and gulps it in one go. The pure amount of energy gives Himanshu a +3 ATK Boost!, but he loses 33% of his Total HP because of the damage to his liver.",
			moveMessage = "Himanshu takes out his steel glass and starts pouring a full can of Red Bull, a Frappe from CCD and 100% original Russian Vodka. (opp_name) is left speechless when Himanshu drinks up this drink in one gulp. Though he looks pale, Himanshu's body is flowing with energy! Attack boosted by 3 stages! Himanshu loses HP!",
			isCooldown = false,
			accuracy = 100,
			isDamaging = false,
			isHealingMove = false,
			isStatChange = true,
			selfAttackChange = 3,
			selfDefenseChange = 0,
			oppAttackChange = 0,
			oppDefenseChange = 0,
            recoil = 0.25f
		},

		// Memories, Bro! Move
		new Move() {
			moveName = "Memories, Bro!",
			moveDesc = "Himanshu reminds the opponent of his small and fleeting life and reminds them how they don't have any memorable moments in their life. The opponent is disheartened by this and loses ATK by 1 stage.",
			moveMessage = "Himanshu approaches (opp_name) and says to them \"Yaar, matlab teri life mai koi memories hi nahi hai, bro\". (opp_name) is disheartened because of Himanshu's words. Attack dropped by 1 stage!",
			isCooldown = false,
			accuracy = 100,
			isDamaging = false,
			isHealingMove = false,
			isStatChange = true,
			selfAttackChange = 0,
			selfDefenseChange = 0,
			oppAttackChange = -1,
			oppDefenseChange = 0,
		},

		// Gambling is Fun Move
		new Move() { 
			moveName = "Gambling is Fun",
			moveDesc = "Himanshu offers a diabolical gamble to the opponent, a coin toss, whose winner heals 15% of his total HP. Little does the opponent know it is a biased coin to favour himself.",
			moveMessage = "Himanshu switches on his gambling mode, approaching the opponent with a coin. The rules are simple, the winner gets a free oreo shake from the loser. The coin goes in the air and... ",
			isCooldown = false,
			accuracy = 100,
			isDamaging = false,
			isHealingMove = true,
			isStatChange = false,
		},

		// Master(de)bater Move
		new Move() {
			moveName = "Master(de)bater",
			moveDesc = "Himanshu uses his excellent master(de)bating skills and convinces the opponent to hurt himself. Deals moderate damage to the opponent.",
			moveMessage = "Himanshu the master (de)bater, starts a debate with (opp_name). THAT (opp_name) should hurt himself. Himanshu goes on a long ramble and makes up a solid argument for the House. (opp_name) realises he can't win this debate so he just complies and hurts himself. (value) damage dealt!",
			isCooldown = false,
			accuracy = 100,
			isDamaging = true,
			damage = 100,
			isHealingMove = false,
			isStatChange = false,
		}
	};

	// Initialize function for any random/niche Move effect
	public static void Initialize() {
		moves[2].moveMessage = "Himanshu switches on his gambling mode, approaching the opponent with a coin. The rules are simple, the winner gets a free oreo shake from the loser. The coin goes in the air and...";
		moves[2].selfHealAmount = 0;
		moves[2].oppHealAmount = 0;

		int x = Random.Range(0, 100);
		if (x < 75) {
            // Himanshu win
            moves[2].moveMessage += "Himanshu has done it, he has done it, he won a free Oreo Shake ! He thinks to add it into his dangerous drink but for now just drinks it on its own. HP Recovered";
			moves[2].selfHealAmount = 20;
		} else {
            // Opponent win
            moves[2].moveMessage += "(opp_name) has done it! He has won a free oreo shake of Himanshu. This is why you don't gamble kids. (opp_name) recovers HP!";
            moves[2].oppHealAmount = 15;
        }
    }
}
