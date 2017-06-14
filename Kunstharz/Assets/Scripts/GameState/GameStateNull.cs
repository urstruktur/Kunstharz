using System;
using UnityEngine;

namespace Kunstharz
{
	public class GameStateNull : IGameState
	{
		public const int IDX = -1;

		public static IGameState instance {
			get { return instance_; }
		}

		private static  GameStateNull instance_ = new GameStateNull();

		private GameStateNull() {}

		public void Enter(GameContext ctx) {}
		public void Exit(GameContext ctx) {}
		public void PlayerSelectedTarget(GameContext ctx, Player selectingPlayer, Target target) {}
		public void PlayerSelectedPlayer(GameContext ctx, Player selectingPlayer, Player selectedPlayer) {}
		public void PlayerFinishedMotion(GameContext ctx, Player player) {}
	}
}

