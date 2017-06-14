using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Kunstharz
{
	public class GameStateRound : NetworkBehaviour, IGameState
	{
		public const int IDX = 1;

		public bool timeoutEnabled = true;

		/// <summary>
		/// If <code>true</code>, players move when both selected a target,
		/// if <code>false</code>, players move immediately.
		/// </summary>
		public bool synchronizedMotion = true;

		private GameContext ctx;

		private Transform ghostTransform;

		public void Enter(GameContext ctx) {
			this.ctx = ctx;

			print("Entering round state");

			FindPlayers(ctx);
			GiveCameraToPlayer(ctx.localPlayer);
			HideGhostTransform();

			if(isServer) {
				SetInitalPlayerStatesForRound();
			}
		}

		public void Exit(GameContext ctx) {
			print("Exiting round state");
			HideGhostTransform();
		}

		public void PlayerSelectedPlayer(GameContext ctx, Player selectingPlayer, Player selectedPlayer) {
			print("PlayerSelectedPlayer");
		}

		public void PlayerSelectedTarget(GameContext ctx, Player player, Target target) {
			print("PlayerSelectedTarget");

			if(player.isLocalPlayer) {
				ghostTransform.gameObject.SetActive (true);
				ghostTransform.transform.position = target.position;
				ghostTransform.transform.up = target.normal;
			}

			if(isServer) {
				Motion motion = player.GetComponent<Motion> ();
				motion.RpcSetFlyTarget(target);
				player.state = PlayerState.SelectedMotion;

				bool bothPlayersSelectedMotion = ctx.localPlayer.state == PlayerState.SelectedMotion &&
				                                 ctx.remotePlayer.state == PlayerState.SelectedMotion;

				if(bothPlayersSelectedMotion) {
					ctx.localPlayer.state = PlayerState.ExecutingMotion;
					ctx.remotePlayer.state = PlayerState.ExecutingMotion;

					ctx.localPlayer.GetComponent<Motion>().RpcLaunch();
					ctx.remotePlayer.GetComponent<Motion>().RpcLaunch();
				} else if(!synchronizedMotion) {
					player.state = PlayerState.ExecutingMotion;
					motion.RpcLaunch();
				}
			}
		}

		public void PlayerFinishedMotion(GameContext ctx, Player player) {
			if(isServer) {
				player.state = PlayerState.SelectingMotion;
			}
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
			Vector3 pos = camTransform.localPosition;
			Quaternion orientation = camTransform.localRotation;

			camTransform.parent = activePlayer.transform;
			camTransform.localPosition = pos;
			camTransform.localRotation = orientation;

			camTransform.GetComponent<Controls> ().enabled = true;
		}

		private void HideGhostTransform() {
			ghostTransform = GameObject.Find ("GhostPlayer").transform;
			ghostTransform.gameObject.SetActive (false);
		}

		private void SetInitalPlayerStatesForRound() {
			bool isConfrontation = LineOfSightExists();
			PlayerState commonState = isConfrontation
			                             ? PlayerState.SelectingShot
										 : PlayerState.SelectingMotion;
			
			ctx.localPlayer.state = commonState;
			ctx.remotePlayer.state = commonState;
		}

		private bool LineOfSightExists() {
			Player p1 = ctx.localPlayer;
			Player p2 = ctx.remotePlayer;

			Vector3 p1Pos = p1.transform.position + p1.transform.forward * p1.GetComponent<BoxCollider> ().size.z * 0.7f;
			Vector3 p2Pos = p2.transform.position + p2.transform.forward * p2.GetComponent<BoxCollider> ().size.z * 0.7f;

			Vector3 dir = p2Pos - p1Pos;
			Vector3 start = p1Pos;
			RaycastHit hit;

			if (Physics.Raycast (start, dir, out hit, dir.magnitude) && hit.collider.GetComponent<Player> () == p2) {
				Debug.DrawRay (start, dir, Color.red);
				return true;
			} else {
				Debug.DrawRay (start, dir, Color.blue);
				return false;
			}
		}
	}
}

