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
		private bool p1Approved;
		private bool p2Approved;

		bool IsStateActive() {
			return ctx.currentStateIdx == IDX;
		}

		public void Enter(GameContext ctx) {
			print("Enter finish");
			this.ctx = ctx;

			p1Approved = false;
			p2Approved = false;

			StartCoroutine(StartMenuIfNoRematch());
        }

		private IEnumerator StartMenuIfNoRematch() {
			yield return new WaitForSeconds(rematchTimeout);

			if(IsStateActive()) {
				print("loading menu");
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
			print("Selected");
			if(player == ctx.localPlayer) {
				print("P1 approved");
				p1Approved = true;
			} else if(player == ctx.remotePlayer) {
				print("P2 approved");
				p2Approved = true;
			}

			if(p1Approved && p2Approved) {
				ctx.currentStateIdx = GameStateKickoff.IDX;
				NetworkManager.Shutdown();
			}
        }
	}
}

