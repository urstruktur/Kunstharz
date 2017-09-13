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

		public float rematchTimeout = 10.0f;

		private GameContext ctx;

		bool IsStateActive() {
			return ctx.currentStateIdx == IDX;
		}

		public void Enter(GameContext ctx) {
			print("Enter finish");
			this.ctx = ctx;

			if(isServer) {
				ctx.localPlayer.approvesRematch = false;
				ctx.remotePlayer.approvesRematch = false;
			}

			StartCoroutine(StartMenuIfNoRematch());
			ShowFinishUI();
        }

		private IEnumerator StartMenuIfNoRematch() {
			yield return new WaitForSeconds(rematchTimeout);

			bool p1Approved = ctx.localPlayer.approvesRematch;
			bool p2Approved = ctx.remotePlayer.approvesRematch;

            Soundsystem ss = FindObjectOfType<Soundsystem>();
            if (ss != null)
            {
                ss.StopAllSounds();
            }

            if (!(p1Approved && p2Approved)) {
				print("loading menu");
				NetworkManager.Shutdown();
				SceneManager.LoadScene(0);
			} else {
				print("Not loading menu");
			}
		}

		public void Exit(GameContext ctx) {
			ctx = null;
			print("Exit finish");

			HideFinishGui();
        }

		public void Selected(GameContext ctx, Player player, Vector3 direction) {
			player.approvesRematch = true;

			bool p1Approved = ctx.localPlayer.approvesRematch;
			bool p2Approved = ctx.remotePlayer.approvesRematch;

			if(p1Approved && p2Approved) {
				ctx.currentStateIdx = GameStateKickoff.IDX;
			}
        }

		private void ShowFinishUI() {
			int roundsWinCount = GameContext.instance.GetComponent<GameStateRound>().roundsWinCount;

			var crosshair = GameObject.Find("Crosshair").GetComponent<ModularCrosshair>();
			var gui = GameObject.Find ("GUI").GetComponent<Gui> ();

			if (ctx.localPlayer.state == PlayerState.Victorious) {
				gui.ShowInstruction(Gui.InstructionType.Win, GameContext.instance.GetComponent<GameStateFinish>().rematchTimeout, true);
				crosshair.ShowFinishedTimer(GameContext.instance.GetComponent<GameStateFinish>().rematchTimeout);
			} else if (ctx.localPlayer.state == PlayerState.Dead) {
				gui.ShowInstruction(Gui.InstructionType.Lose, GameContext.instance.GetComponent<GameStateFinish>().rematchTimeout, true);
				crosshair.ShowFinishedTimer(GameContext.instance.GetComponent<GameStateFinish>().rematchTimeout);
			} else {
				Debug.LogError("In finish state but wins < roundWinCount? What is this?");
			}
		}

		private void HideFinishGui() {
			var crosshair = GameObject.Find("Crosshair").GetComponent<ModularCrosshair>();
			crosshair.HideFinishedTimer();
		}
	}
}

