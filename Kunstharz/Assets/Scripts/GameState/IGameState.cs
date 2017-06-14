using System;
using UnityEngine;

namespace Kunstharz
{
	public interface IGameState
	{
		void Enter(GameContext ctx);
		void Exit(GameContext ctx);
		void Selected(GameContext ctx, Player player, Vector3 direction);
	}
}

