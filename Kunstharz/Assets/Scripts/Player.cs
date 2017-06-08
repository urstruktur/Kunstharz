using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Kunstharz {
	public class Player : NetworkBehaviour {
		public float deathTimeout = 8.0f;

		[SyncVar(hook = "OnStateChange")]
		public PlayerState state = PlayerState.AwaitingRoundStart;
		[SyncVar(hook = "OnWinsChange")]
		public int wins = 0;

		private Vector3 spawnPosition;
		private Quaternion spawnRotation;
		private float remainingDeathTimeout = float.MaxValue;
		private ModularCrosshair crosshair;
		private Gui gui;

		void Start() {
			print ("Player start");
		}

		[ClientRpc]
		public void RpcInitPlayer(Vector3 spawnPos, Quaternion spawnRot) {
			print (spawnPos);
			transform.parent = GameObject.Find ("Game").transform;
			spawnPosition = spawnPos;
			spawnRotation = spawnRot;
			transform.position = spawnPosition;
			transform.rotation = spawnRotation;
			SendMessageUpwards ("PlayerJoined", this);
			crosshair = GameObject.Find("Crosshair").GetComponent<ModularCrosshair>();
			gui = GameObject.Find ("GUI").GetComponent<Gui> ();
		}

		void Update() {
			if (remainingDeathTimeout != float.MaxValue) {
				remainingDeathTimeout -= Time.deltaTime;

				if (remainingDeathTimeout <= 0) {
					print ("Died after timing out");
					CmdSetState (PlayerState.TimedOut);
				}
			}
		}

		[Command]
		public void CmdRespawn() {
			state = PlayerState.SelectingMotion;
			RpcResetPosition ();
		}

		[ClientRpc]
		public void RpcResetPosition() {
			print ("Resetting position to " + spawnPosition);
			transform.position = spawnPosition;
			transform.rotation = spawnRotation;
		}

		void SelectedTarget(Target target) {
			if (state == PlayerState.SelectingShot) {
				SendMessage ("SetShootTarget", target);
				CmdSetState (PlayerState.ExecutingShot);

                ImageEffectSuperformula i = Camera.main.GetComponent<ImageEffectSuperformula>();
                if (i != null)
                {
                    i.Shoot();
                }

				crosshair.ShowShootCrosshair();
            } else {
				SendMessage ("SetFlyTarget", target);
				CmdSetState (PlayerState.SelectedMotion);

                if(Motion.allowMoveDebugStatic)
                {
                    GetComponent<Motion>().enabled = true;
                }

                ImageEffectShockwave i = Camera.main.GetComponent<ImageEffectShockwave>();
                if(i != null)
                {
                    i.Shock(target.position);
                }

			}
		}

		void MotionFinished (Player movedPlayer) {
			if (isLocalPlayer) {
				CmdSetState (PlayerState.ExecutedMotion);
			}
		}

		void HitPlayer(Player player) {
			CmdSetState (PlayerState.Victorious);
			CmdWon ();
		}

		[Command]
		public void CmdWon() {
			++wins;
		}

		[Command]
		public void CmdSetState(PlayerState state) {
			this.state = state;
		}

		void OnStateChange(PlayerState state) {
			if (state == PlayerState.SelectingMotion && isLocalPlayer) {
				remainingDeathTimeout = deathTimeout;
			} else {
				remainingDeathTimeout = float.MaxValue;
			}

			PlayerState oldState = this.state;
			this.state = state;

			SendMessageUpwards ("PlayerStateChanged", this, SendMessageOptions.DontRequireReceiver);
		}

		void OnWinsChange(int wins) {
			this.wins = wins;
			SendMessageUpwards ("PlayerWon", this);
		}
        
        // only executed locally
        public override void OnStartLocalPlayer()
        {
            gameObject.transform.FindChild("Mesh").GetComponent<MeshRenderer>().materials[1].color = Color.green;
        }

		[Command]
		public void CmdInstantiate(GameObject go) {
			NetworkServer.Spawn(go);
		}

		[Command]
		public void CmdDestroy(GameObject go) {
			NetworkServer.Destroy(go);
		}
    }
}
