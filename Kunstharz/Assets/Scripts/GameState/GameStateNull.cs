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
		public void LocalPlayerSelectedLocation(GameContext ctx, Vector3 selectedLocation) {}
		public void RemotePlayerSelectedLocation(GameContext ctx, Vector3 selectedLocation) {}
	}
}

