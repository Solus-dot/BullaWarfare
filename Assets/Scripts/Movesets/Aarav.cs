// Aarav 

// Move 1:  The Pegged One
// Aarav acts submissive towards the opponent. The opponent is charmed by the submissiveness, but Aarav's ego also takes a hit.
// Opponent loses ATK by 2 Stages, Aarav loses DEF by 1.

// Move 2: Amiri Flex
// Aarav whips out his Amiri jacket and flexes his luxury in front of the opponent.
// Boosts his ego in the process. DEF increased by 1 stage.

// Move 3: TT Spin (50 BP)
// Aarav calculates the golden ratio and applies the perfect spin on a TT ball and hits the opponent with it.
// Deals moderate damage, 30% chance to flinch the opponent. 

// Move 4: Dassa Nuke (175 BP)
// Institute Rank 3, Aarav channels all his dassas into one giant academic nuke and throws it at the opponent.
// Deals insane damage but because of the lost Dassas, Aarav's ATK reduces by 2 stages.

//Move Messages
// Move 1: Aarav gets down on all fours and acts as submissive and breedable as he could. Never dropping the pegging allegations. (opp_name) loses Attack by 2, Aarav loses defence by 1 because of the lost ego. 
// Move 2: Aarav takes his Amiri jacket out of his bag and wears it in front of (opp_name). Aww this rich Pune bastard, his ego is so high right now! Aarav's Defense increased by 1!
// Move 3: Aarav imagines the Fibonacci sequence, he has visualised the golden ratio and can now apply it on his TT Serve. He hits (opp_name) with this spinning ball and deals damage. (value) damage dealt!
// Move 4: Aarav Oswal. Branch Changer from MTH to CSE, Institute Rank 3 gathers up all the 10s in his report card and shrinks it into an academic nuke. BOOOOOM, The acad nuke has gone off on (opp_name)! (value) damage dealt! Aarav becomes dumber because of the lost dassas. Attack dropped by 2.

using UnityEngine;
using System.Collections.Generic;

public static class Aarav
{
	public static List<Move> moves = new List<Move>() {
		// The Pegged One Move
		new Move() {
			moveName = "The Pegged One",
			moveDesc = "Aarav acts submissive towards the opponent. The opponent is charmed by the submissiveness, but Aarav's ego also takes a hit. Opponent loses ATK by 2 Stages, Aarav loses DEF by 1.",
			moveMessage = "Aarav gets down on all fours and acts as submissive and breedable as he could. Never dropping the pegging allegations. (opp_name) loses Attack by 2, Aarav loses defence by 1 because of the lost ego.",
			isCooldown = false,
			accuracy = 100,
			isDamaging = false,
			isHealingMove = false,
			isStatChange = true,
			selfAttackChange = 0,
			selfDefenseChange = -1,
			oppAttackChange = -2,
			oppDefenseChange = 0
		},

		// Amiri Flex Move
		new Move() {
			moveName = "Amiri Flex",
			moveDesc = "Aarav whips out his Amiri jacket and flexes his luxury in front of the opponent. Boosts his ego in the process. DEF increased by 1 stage.",
			moveMessage = "Aarav takes his Amiri jacket out of his bag and wears it in front of (opp_name). Aww this rich Pune bastard, his ego is so high right now! Aarav's Defense increased by 1!",
			isCooldown = false,
			accuracy = 100,
			isDamaging = false,
			isHealingMove = false,
			isStatChange = true,
			selfAttackChange = 0,
			selfDefenseChange = 1,
			oppAttackChange = 0,
			oppDefenseChange = 0,
		},

		// TT Spin Move
		new Move() { 
			moveName = "TT Spin",
			moveDesc = "Aarav calculates the golden ratio and applies the perfect spin on a TT ball and hits the opponent with it. Deals moderate damage, 30% chance to flinch the opponent.",
			moveMessage = "Aarav imagines the Fibonacci sequence, he has visualised the golden ratio and can now apply it on his TT Serve. He hits (opp_name) with this spinning ball and deals damage. (value) damage dealt!",
			isCooldown = false,
			accuracy = 100,
			isDamaging = true,
			damage = 50,
			isHealingMove = false,
			isStatChange = false,
            flinch = 30
		},

		// Dassa Nuke Move
		new Move() {
			moveName = "Dassa Nuke",
			moveDesc = "Institute Rank 3, Aarav channels all his dassas into one giant academic nuke and throws it at the opponent. Deals insane damage but because of the lost Dassas, Aarav's ATK reduces by 2 stages.",
			moveMessage = "Aarav Oswal. Branch Changer from MTH to CSE, Institute Rank 3 gathers up all the 10s in his report card and shrinks it into an academic nuke. BOOOOOM, The acad nuke has gone off on (opp_name)! (value) damage dealt! Aarav becomes dumber because of the lost dassas. Attack dropped by 2.",
			isCooldown = false,
			accuracy = 100,
			isDamaging = true,
			damage = 175,
			isHealingMove = false,
			isStatChange = true,
			selfAttackChange = -2,
			selfDefenseChange = 0,
			oppAttackChange = 0,
			oppDefenseChange = 0,
		}
	};

	// Initialize function for any random/niche Move effect
	public static void Initialize() {}
}