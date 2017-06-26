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

		/// <summary>
		/// Determines how often you need to win a round to win the whole game.
		/// </summary>
		public int roundsWinCount = 3;

		public bool timeoutEnabled = true;

		public bool generousTimeoutAtGameStart = true;

		public bool generousTimeoutAtRoundStart = false;

		/// <summary>
		/// Time in seconds of inactivity after entering a selection state
		/// that kills a player.
		///
		/// Note that depending on the settings, either this or generousTimeout
		/// is used.
		/// </summary>
		public float standardTimeout = 7.0f;

		/// <summary>
		/// Time in seconds of inactivity after round start that causes a
		/// player to die.
		/// </summary>
		public float generousTimeout = 12.0f;

		/// <summary>
		/// If <code>true</code>, players move when both selected a target,
		/// if <code>false</code>, players move immediately.
		/// </summary>
		public bool synchronizedMotion = true;

		public Vector3 camLocalPosition = new Vector3(0.0f, 1.0f, 0.0f);

		/// <summary>
		/// Index of the last started round or -1 if no round has been started yet.
		/// This is not synced to the client and should only be used on the server.
		/// </summary>
		private int roundIdx = -1;

		/// <summary>
		/// Number of actions that have been started in this round. Every time both
		/// players start to move, this is increased by one.
		///
		/// This is not synced to the client and should only be used on the server.
		/// </summary>
		private int roundMoveCount = 0;

		private GameContext ctx;

		void Start() {
			roundIdx = -1;
			roundMoveCount = 0;

			if(timeoutEnabled && !synchronizedMotion) {
				Debug.LogError("Timeout can only be enabled when motion is synchronized, disabling timeout…");
				timeoutEnabled = false;
			}
		}

		public void Enter(GameContext ctx) {
			this.ctx = ctx;

			if(isServer) {
				++roundIdx;
				roundMoveCount = 0;

				StopAllCoroutines();

				ResetPlayerDeathReasons();
				DeterminePlayerSelectionStates();
			}
		}

		public void Exit(GameContext ctx) {
			if(isServer) {
				StopAllCoroutines();
			}
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
						other.deathReason = DeathReason.Shot;
						player.CmdWon();

						EndRound();
					} else {
						// missed ):
						player.RpcVisualizeShotMissed();
					}

                    // spawn particlesystem
                    if (!player.isLocalPlayer)
                    {
                        RpcSpawnShotVis(player.transform.position, player.transform.up, direction);
                    }
                }
			} else {
				if(player.state == PlayerState.SelectingShot) {
					player.RpcVisualizeShotMissed();

                    // spawn particlesystem
                    if (!player.isLocalPlayer)
                    {
                        RpcSpawnShotVis(player.transform.position, player.transform.up, direction);
                    }
                } else {
					player.RpcVisualizeMotionMissed();
				}
			}
		}

		[ClientRpc]
        private void RpcSpawnShotVis(Vector3 playerPos, Vector3 playerUp, Vector3 direction)
        {
            // spawn particlesystem
            try
            {
                GameObject particles = Instantiate(FindObjectOfType<Game>().shotParticles);
                particles.transform.position = playerPos + playerUp/2;
                particles.transform.rotation = Quaternion.LookRotation(direction);
                Destroy(particles, 5);
            }
            catch
            {
                Debug.LogError("Could not instantiate shot particles.");
            }
        }

		private void SelectMotionTarget(Player player, Target target) {
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
					++roundMoveCount;

					float maxDuration = Math.Max(m1.FlightDuration(target), m2.FlightDuration(target));

					StartCoroutine(ReevaluatePlayerStatesLater(maxDuration));
				}
			} else {
				player.state = PlayerState.ExecutingMotion;
				motion.RpcLaunch();
				++roundMoveCount;
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

		IEnumerator ReevaluatePlayerStatesLater(float duration) {
			StopCoroutine("CheckTimeouts");
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

			StartCoroutine("CheckTimeouts");
		}

		private bool LineOfSightExists() {
			Player p1 = ctx.localPlayer;
			Player p2 = ctx.remotePlayer;

			const int raycastCount = 5;

			Vector3 shooterPos = p1.transform.TransformPoint(camLocalPosition);
			var shooteeColl = p2.GetComponent<BoxCollider> ();
			Vector3 shooteePosMin = shooteeColl.bounds.min;
			Vector3 shooteePosMax = shooteeColl.bounds.max;

			for(int raycastIdx = 0; raycastIdx < raycastCount; ++raycastIdx) {
				float alpha = raycastIdx / (raycastCount - 1);
				Vector3 shooteePos = Vector3.Lerp(shooteePosMin, shooteePosMax, alpha);

				Vector3 dir = shooteePos - shooterPos;
				Vector3 start = shooterPos;
				RaycastHit hit;

				if (Physics.Raycast (start, dir, out hit, dir.magnitude) && hit.collider.GetComponent<Player> () == p2) {
					Debug.DrawRay (start, dir, Color.red, 1.0f);
					return true;
				} else {
					Debug.DrawRay (start, dir, Color.green, 1.0f);
				}
			}

			return false;
		}

		private IEnumerator CheckTimeouts() {
			if(timeoutEnabled) {
				bool generous = (generousTimeoutAtRoundStart && roundMoveCount == 0) ||
				                (generousTimeoutAtGameStart && roundIdx == 0 && roundMoveCount == 0);

				float timeout = generous ? generousTimeout : standardTimeout;
				ctx.localPlayer.RpcVisualizeTimeout(timeout);
				ctx.remotePlayer.RpcVisualizeTimeout(timeout);

				yield return new WaitForSeconds(timeout);

				var p1 = ctx.localPlayer;
				var p2 = ctx.remotePlayer;

				bool p1Acted = p1.state != PlayerState.SelectingMotion &&
				               p1.state != PlayerState.SelectingShot;
				bool p2Acted = p2.state != PlayerState.SelectingMotion &&
				               p2.state != PlayerState.SelectingShot;

				if(!p1Acted || !p2Acted) {
					if(!p1Acted && !p2Acted) {
						p1.state = PlayerState.Dead;
						p2.state = PlayerState.Dead;
						p1.deathReason = DeathReason.TimedOut;
						p2.deathReason = DeathReason.TimedOut;
						print("Both timed out");
					} else if(!p1Acted) {
						p1.state = PlayerState.Dead;
						p1.deathReason = DeathReason.TimedOut;
						p2.state = PlayerState.Victorious;
						p2.CmdWon();
						print("P1 timed out");
					} else if(!p2Acted) {
						p1.state = PlayerState.Victorious;
						p2.state = PlayerState.Dead;
						p2.deathReason = DeathReason.TimedOut;
						p1.CmdWon();
						print("P2 timed out");
					}

					// If either player timed out, start next round later
					EndRound();
				}
			}
		}

		private void ResetPlayerDeathReasons() {
			ctx.localPlayer.deathReason = DeathReason.None;
			ctx.remotePlayer.deathReason = DeathReason.None;
		}

		private void EndRound() {
			StartCoroutine(EndRoundAfterSomeTimeToEnsurePlayerIsSyncedToClient());
		}

		private IEnumerator EndRoundAfterSomeTimeToEnsurePlayerIsSyncedToClient() {
			yield return new WaitForSecondsRealtime(0.02f);
			if(ctx.localPlayer.wins >= roundsWinCount ||
			   ctx.remotePlayer.wins >= roundsWinCount) {

				// If someone won the game yet, go to finish
				ctx.currentStateIdx = GameStateFinish.IDX;
			} else {
				// Otherwise, start a new round later
				ctx.currentStateIdx = GameStateRoundTransition.IDX;
			}
		}
	}
}

