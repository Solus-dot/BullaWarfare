// Harsh

// Move 1: Too Much Radio
// Plays a radiohead song which makes everyone around them sad,
// Opponent loses Defense stat.

// Move 2: Literally Me
// Boasts about being Ryan Gosling, angering the opponents,
// Opponent's Defense drops 2 stages while Attack goes up 1 stage.

// Move 3: QC Ka Choda
// Hurts the opponents by spitting random facts to the opponent,
// Deals decent damage to opponent.

// Move 4: Funny Valentine
// Makes an incredibly funny and self-depricating joke that they hurt themselves,
// Deals good damage but reduces the Defense stat of the user.

// Move Messages
// Move 1: Harsh turns on his Radiohead playlist, a british man sings about being sad, the mood turns gloomy (opp_name) loses Defense by 1 stage.
// Move 2: Harsh puts on his scorpion jacket, he has done it, he is Ryan Gosling! (opp_name) gets annoyed by Harsh's delusion and loses his guard. Defense dropped by 2, Attack upped by 1!!
// Move 3: Harsh spits a random fact about Kanye. (opp_name) is damaged by the useless trivia, (value) damage dealt.
// Move 4: Harsh cracks a joke about having no game, (opp_name) laughs so hard that it starts to hurt badly, (value) damage dealt. A hint of loneliness can be seen in HVA's eyes! True comedy is done only by the saddest people. Harsh's Defense drops by 1 stage.

using UnityEngine;
using System.Collections.Generic;

public static class Harsh
{
	public static List<Move> moves = new List<Move>() {
		// Too Much Radio Move
		new Move() {
			moveName = "Too Much Radio",
            moveDesc = "Plays a radiohead song which makes everyone around them sad,\nOpponent loses Defense stat.",
			moveMessage = "Harsh turns on his Radiohead playlist, a british man sings about being sad, the mood turns gloomy (opp_name) loses Defense by 1 stage.",
			cooldown = 0,
			accuracy = 100,
			isDamaging = false,
			isHealingMove = false,
			isStatChange = true,
			selfAttackChange = 0,
			selfDefenseChange = 0,
			oppAttackChange = 0,
			oppDefenseChange = -1
		},

		// Literally Me Move
		new Move() {
			moveName = "Literally Me",
			moveDesc = "Boasts about being Ryan Gosling, angering the opponents,\nOpponent's Defense drops 2 stages while Attack goes up 1 stage.",
			moveMessage = "Harsh puts on his scorpion jacket, he has done it, he is Ryan Gosling! (opp_name) gets annoyed by Harsh's delusion and loses his guard. Defense dropped by 2, Attack upped by 1!!",
			cooldown = 0,
			accuracy = 100,
			isDamaging = false,
			isHealingMove = false,
			isStatChange = true,
			selfAttackChange = 0,
			selfDefenseChange = 0,
			oppAttackChange = 1,
			oppDefenseChange = -2
		},

		// QC Ka Choda Move
		new Move() { 
			moveName = "QC Ka Choda",
			moveDesc = "Hurts the opponents by spitting random facts to the opponent,\nDeals decent damage to opponent.",
			moveMessage = "Harsh spits a random fact about Kanye. (opp_name) is damaged by the useless trivia, (value) damage dealt.",
			cooldown = 0,
			accuracy = 100,
			isDamaging = true,
            damage = 20,
			isHealingMove = false,
			isStatChange = false,
		},

		// Funny Valentine Move
		new Move() {
			moveName = "Funny Valentine",
			moveDesc = "Makes an incredibly funny and self-depricating joke that they hurt themselves,\nDeals good damage but reduces the Defense stat of the user.",
			moveMessage = "cracks a joke about having no game, (opp_name) laughs so hard that it starts to hurt badly, (value) damage dealt. A hint of loneliness can be seen in HVA's eyes! True comedy is done only by the saddest people. Harsh's Defense drops by 1 stage.",
			cooldown = 1,
			accuracy = 100,
			isDamaging = true,
			damage = 20,
			isHealingMove = false,
			isStatChange = true,
			selfAttackChange = 0,
			selfDefenseChange = -1,
			oppAttackChange = 0,
			oppDefenseChange = 0
		}
	};
}
