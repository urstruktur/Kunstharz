using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

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

			FindPlayers(ctx);
			AssignLocalPlayerName();
			GiveCameraToPlayer(ctx.localPlayer);

			if(isServer) {
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

		private void FindPlayers(GameContext ctx) {
			foreach(var playerGO in GameObject.FindGameObjectsWithTag("Player")) {
				Player player = playerGO.GetComponent<Player> ();
				if(player.isLocalPlayer) {
					ctx.localPlayer = player;
				} else {
					ctx.remotePlayer = player;
				}
			}
		}

		private void GiveCameraToPlayer(Player activePlayer) {
			Transform camTransform = Camera.main.transform;

			camTransform.parent = activePlayer.transform;
			camTransform.localPosition = GetComponent<GameStateRound> ().camLocalPosition;
			camTransform.GetComponent<Controls> ().enabled = true;
		}

		private void AssignLocalPlayerName() {
			ctx.localPlayer.CmdSetPlayerName(ctx.localPlayerName);
		}
	}
}

