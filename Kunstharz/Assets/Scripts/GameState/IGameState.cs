using System;
using UnityEngine;

namespace Kunstharz
{
	public interface IGameState
	{
		void Enter(GameContext ctx);
		void Exit(GameContext ctx);
		void PlayerSelectedTarget(GameContext ctx, Player selectingPlayer, Target target);
		void PlayerSelectedPlayer(GameContext ctx, Player selectingPlayer, Player selectedPlayer);
		void PlayerFinishedMotion(GameContext ctx, Player player);
	}
}

