using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Kunstharz
{
	public class GameStateRoundTransition : NetworkBehaviour, IGameState
	{
		public const int IDX = 2;
		public const float transitionTime = 3.0f;

		/// <summary>
		/// Determines how often you need to win a round to win the whole game.
		/// </summary>
		public int roundsWinCount = 3;

		private GameContext ctx;

		public void Enter(GameContext ctx) {
			this.ctx = ctx;

			if(isServer) {
				if(ctx.localPlayer.wins >= roundsWinCount ||
				   ctx.remotePlayer.wins >= roundsWinCount) {

					StartCoroutine(FinishGameLater());
				} else {
					StartCoroutine(StartNextRoundLater());
				}
			}
        }

		public void Exit(GameContext ctx) {
        }

		public void Selected(GameContext ctx, Player player, Vector3 direction) {
        }

		private IEnumerator FinishGameLater() {
			yield return new WaitForSeconds(transitionTime);

			ctx.currentStateIdx = GameStateFinish.IDX;
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

