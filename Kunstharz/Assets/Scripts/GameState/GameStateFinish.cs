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
        }

		private IEnumerator StartMenuIfNoRematch() {
			yield return new WaitForSeconds(rematchTimeout);

			bool p1Approved = ctx.localPlayer.approvesRematch;
			bool p2Approved = ctx.remotePlayer.approvesRematch;

			if(!(p1Approved && p2Approved)) {
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
        }

		public void Selected(GameContext ctx, Player player, Vector3 direction) {
			player.approvesRematch = true;

			bool p1Approved = ctx.localPlayer.approvesRematch;
			bool p2Approved = ctx.remotePlayer.approvesRematch;

			if(p1Approved && p2Approved) {
				ctx.currentStateIdx = GameStateKickoff.IDX;
			}
        }
	}
}

