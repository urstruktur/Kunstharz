using System;
using UnityEngine;

namespace Kunstharz
{
	public interface IGameState
	{
		void Enter(GameContext ctx);
		void Exit(GameContext ctx);
		void LocalPlayerSelectedLocation(GameContext ctx, Vector3 selectedLocation);
		void RemotePlayerSelectedLocation(GameContext ctx, Vector3 selectedLocation);
	}
}

