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

			FindPlayers(ctx);
			AssignLocalPlayerName();
			GiveCameraToPlayer(ctx.localPlayer);
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

		private void SetKickoffUI() {
			var crosshair = GameObject.Find("Crosshair").GetComponent<ModularCrosshair>();
			var gui = GameObject.Find ("GUI").GetComponent<Gui> ();
			crosshair.ShowPrepareTimer(GameContext.instance.GetComponent<GameStateKickoff>().kickoffDuration);
			gui.ShowInstruction(Gui.InstructionType.PrepareGame, GameContext.instance.GetComponent<GameStateKickoff>().kickoffDuration, true);
		}
	}
}

