using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Kunstharz
{
	public class GameStateKickoff : NetworkBehaviour, IGameState
	{
		public float kickoffDuration = 5.0f;

		public const int IDX = 4;

		private GameContext ctx;

		public void Enter(GameContext ctx) {
			print("Starting kickoff");

			this.ctx = ctx;

			SetKickoffUI();

			if(isServer) {
				ctx.localPlayer.wins = 0;
				ctx.remotePlayer.wins = 0;
				ctx.localPlayer.RpcResetPosition();
				ctx.remotePlayer.RpcResetPosition();
				StartCoroutine(StartRoundLater());
			}
        }

		public void Exit(GameContext ctx) {
			print("After kickoff");
        }

		public void Selected(GameContext ctx, Player player, Vector3 direction) {
        }

		private IEnumerator StartRoundLater() {
			yield return new WaitForSeconds(kickoffDuration);
			ctx.currentStateIdx = GameStateRound.IDX;
		}

		private void SetKickoffUI() {
			var crosshair = GameObject.Find("Crosshair").GetComponent<ModularCrosshair>();
			var gui = GameObject.Find ("GUI").GetComponent<Gui> ();
			crosshair.ShowPrepareTimer(GameContext.instance.GetComponent<GameStateKickoff>().kickoffDuration);
			gui.ShowInstruction(Gui.InstructionType.PrepareGame, GameContext.instance.GetComponent<GameStateKickoff>().kickoffDuration, true);
		}
	}
}

