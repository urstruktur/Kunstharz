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

			ShowRoundTransitionUI();
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

		private void ShowRoundTransitionUI() {
			var crosshair = GameObject.Find("Crosshair").GetComponent<ModularCrosshair>();
			var gui = GameObject.Find ("GUI").GetComponent<Gui> ();

			print("Local: " + ctx.localPlayer.state);
			print("Remote: " + ctx.remotePlayer.state);

			if(ctx.localPlayer.state == PlayerState.Dead && ctx.remotePlayer.state == PlayerState.Victorious) {
				// Local player lost
				gui.ShowInstruction(Gui.InstructionType.Erased, GameContext.instance.GetComponent<GameStateRoundTransition>().transitionTime, true);
				if(ctx.localPlayer.deathReason == DeathReason.Shot) {
					// Lost because local player was shot
				} else {
					// Lost because local player was the only one to time out
				}
			} else if(ctx.remotePlayer.state == PlayerState.Dead && ctx.localPlayer.state == PlayerState.Victorious) {
				gui.ShowInstruction(Gui.InstructionType.Scored, GameContext.instance.GetComponent<GameStateRoundTransition>().transitionTime, true);
				if(ctx.remotePlayer.deathReason == DeathReason.Shot) {
					// Won because remote player was shot
				} else {
					// Won because remote player timed out
				}
			} else {
				// Both died, can only be the case when both timed out
				gui.ShowInstruction(Gui.InstructionType.Erased, GameContext.instance.GetComponent<GameStateRoundTransition>().transitionTime, true);
			}
		}
	}
}

