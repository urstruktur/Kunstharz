using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Kunstharz
{
	public class GameStateFinish : NetworkBehaviour, IGameState
	{
		public const int IDX = 3;

		private GameContext ctx;

		public void Enter(GameContext ctx) {
			this.ctx = ctx;
			SceneManager.LoadScene(0);
        }

		public void Exit(GameContext ctx) {
        }

		public void Selected(GameContext ctx, Player player, Vector3 direction) {
        }
	}
}

