using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Kunstharz
{
	// TODO rounds and timeouts
	public class GameStateRound : NetworkBehaviour, IGameState
	{
		public const int IDX = 1;

		public bool timeoutEnabled = true;

		/// <summary>
		/// Time in seconds of inactivity after round start that causes a
		/// player to die.
		/// </summary>
		public float timeout = 7.0f;

		/// <summary>
		/// If <code>true</code>, players move when both selected a target,
		/// if <code>false</code>, players move immediately.
		/// </summary>
		public bool synchronizedMotion = true;

		private GameContext ctx;

		public Vector3 camLocalPosition = new Vector3(1.0f, 0.0f, 0.0f);

		void Start() {
			if(timeoutEnabled && !synchronizedMotion) {
				Debug.LogError("Timeout can only be enabled when motion is synchronized, disabling timeout…");
				timeoutEnabled = false;
			}
		}

		public void Enter(GameContext ctx) {
			this.ctx = ctx;

			FindPlayers(ctx);
			AssignLocalPlayerName();
			GiveCameraToPlayer(ctx.localPlayer);

			if(isServer) {
				DeterminePlayerSelectionStates();

				if(timeoutEnabled) {
					StartCoroutine(CheckTimeouts()); 
				}
			}
		}

		public void Exit(GameContext ctx) {
			print("Exiting round state");
		}

		public void Selected(GameContext ctx, Player player, Vector3 direction) {
			Vector3 origin = player.transform.TransformPoint(camLocalPosition) + 0.1f*direction;
			Debug.DrawRay(origin, direction*10, Color.magenta, 100.0f);
			RaycastHit hit;
			if (Physics.Raycast (origin, direction, out hit)) {
				if(player.state == PlayerState.SelectingMotion ||
				   player.state == PlayerState.SelectedMotion) {

					Target target;
					target.position = hit.point;
					target.normal = hit.normal;

					SelectMotionTarget(player, target);
				}

				if(player.state == PlayerState.SelectingShot) {
					if (hit.collider.CompareTag ("Player")) {
						// WIN
						var other = hit.collider.GetComponent<Player> ();

						player.RpcVisualizeShotHit();
						player.state = PlayerState.Victorious;
						other.state = PlayerState.Dead;
						player.CmdWon();

						print("Some player just won!");

						ctx.currentStateIdx = GameStateRoundTransition.IDX;
					} else {
						// missed ):
						player.RpcVisualizeShotMissed();
					}
				}
			} else {
				if(player.state == PlayerState.SelectingShot) {
					player.RpcVisualizeShotMissed();
				} else {
					player.RpcVisualizeMotionMissed();
				}
			}
		}

		private void SelectMotionTarget(Player player, Target target) {
			print("Selecting motion target " + target);
			var motion = player.GetComponent<Motion> ();

			motion.RpcSetFlyTarget(target);
			player.RpcVisualizeMotionSelected(target);

			if(synchronizedMotion) {
				player.state = PlayerState.SelectedMotion;

				var p1 = ctx.localPlayer;
				var p2 = ctx.remotePlayer;

				if(p1.state == PlayerState.SelectedMotion &&
				   p2.state == PlayerState.SelectedMotion) {

					p1.state = PlayerState.ExecutingMotion;
					p2.state = PlayerState.ExecutingMotion;
					
					var m1 = p1.GetComponent<Motion> ();
					var m2 = p2.GetComponent<Motion> ();
					
					m1.RpcLaunch();
					m2.RpcLaunch();

					float maxDuration = Math.Max(m1.FlightDuration(target), m2.FlightDuration(target));
					StartCoroutine(ReevaluatePlayerStatesLater(maxDuration));
				}
			} else {
				player.state = PlayerState.ExecutingMotion;
				motion.RpcLaunch();
				float duration = motion.FlightDuration(target);
				StartCoroutine(ReevaluatePlayerStatesLater(duration));
			}
		}

		public void PlayerFinishedMotion(GameContext ctx, Player player) {
			if(isServer && player.state == PlayerState.ExecutingMotion) {
				player.state = PlayerState.ExecutedMotion;

				bool bothExecuted = ctx.localPlayer.state == PlayerState.ExecutedMotion &&
				                    ctx.remotePlayer.state == PlayerState.ExecutedMotion;

				if(bothExecuted) {
					ctx.localPlayer.state = PlayerState.SelectingMotion;
					ctx.remotePlayer.state = PlayerState.SelectingMotion;
				} else if(!synchronizedMotion) {
					player.state = PlayerState.SelectingMotion;
				}
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
			//Vector3 pos = camTransform.localPosition;
			//Quaternion orientation = camTransform.localRotation;

			camTransform.parent = activePlayer.transform;
			camTransform.localPosition = camLocalPosition;
			//camTransform.localRotation = orientation;

			camTransform.GetComponent<Controls> ().enabled = true;

			//camLocalPosition = pos;
		}

		IEnumerator ReevaluatePlayerStatesLater(float duration) {
			yield return new WaitForSeconds(duration);
			DeterminePlayerSelectionStates();
		}

		private void DeterminePlayerSelectionStates() {
			bool isConfrontation = LineOfSightExists();
			PlayerState commonState = isConfrontation
			                             ? PlayerState.SelectingShot
										 : PlayerState.SelectingMotion;
			
			ctx.localPlayer.state = commonState;
			ctx.remotePlayer.state = commonState;

			if(isConfrontation) {
				ctx.localPlayer.RpcVisualizeShotReady();
				ctx.remotePlayer.RpcVisualizeShotReady();
			} else {
				ctx.localPlayer.RpcVisualizeMotionSelectionReady();
				ctx.remotePlayer.RpcVisualizeMotionSelectionReady();
			}
		}

		private bool LineOfSightExists() {
			Player p1 = ctx.localPlayer;
			Player p2 = ctx.remotePlayer;

			Vector3 p1Pos = p1.transform.TransformPoint(camLocalPosition);
			Vector3 p2Pos = p2.transform.TransformPoint(camLocalPosition);

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

		private IEnumerator CheckTimeouts() {
			yield return new WaitForSeconds(timeout);

			var p1 = ctx.localPlayer;
			var p2 = ctx.remotePlayer;

			bool p1WasInactive = p1.state == PlayerState.SelectingMotion ||
			                     p1.state == PlayerState.SelectingShot;

			
			bool p2WasInactive = p2.state == PlayerState.SelectingMotion ||
			                     p2.state == PlayerState.SelectingShot;

			if(p1WasInactive && p2WasInactive) {
				p1.state = PlayerState.Dead;
				p2.state = PlayerState.Dead;
				print("Both timed out");
			} else if(p1WasInactive) {
				p1.state = PlayerState.Dead;
				p2.state = PlayerState.Victorious;
				p2.CmdWon();
				print("P1 timed out");
			} else if(p2WasInactive) {
				p1.state = PlayerState.Victorious;
				p2.state = PlayerState.Dead;
				p1.CmdWon();
				print("P2 timed out");
			}

			if(p1WasInactive || p2WasInactive) {
				// If either player timed out, start next round later
				ctx.currentStateIdx = GameStateRoundTransition.IDX;
			}
		}

		private void AssignLocalPlayerName() {
			ctx.localPlayer.CmdSetPlayerName(ctx.localPlayerName);
		}
	}
}

