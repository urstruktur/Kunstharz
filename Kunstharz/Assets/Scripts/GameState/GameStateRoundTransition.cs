using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Kunstharz
{
	public class GameStateRoundTransition : NetworkBehaviour, IGameState
	{
		public const int IDX = 2;
		public float transitionTime = 3.0f;

		private GameContext ctx;

		public void Enter(GameContext ctx) {
			this.ctx = ctx;

			if(isServer) {
				StartCoroutine(StartNextRoundLater());
			}
        }

		public void Exit(GameContext ctx) {
        }

		public void Selected(GameContext ctx, Player player, Vector3 direction) {
        }

		private IEnumerator StartNextRoundLater() {
			yield return new WaitForSeconds(transitionTime);

			ctx.localPlayer.RpcResetPosition();
			ctx.remotePlayer.RpcResetPosition();

			// Ensure local positions are already correct
			ctx.localPlayer.ResetPosition();
			ctx.remotePlayer.ResetPosition();

			ctx.currentStateIdx = GameStateRound.IDX;
		}
	}
}

