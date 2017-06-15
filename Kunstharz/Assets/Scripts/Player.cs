using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Kunstharz {
	public class Player : NetworkBehaviour {
		public float deathTimeout = 8.0f;

		[SyncVar(hook = "OnNameChange")]
		public string playerName;

		[SyncVar(hook = "OnStateChange")]
		public PlayerState state = PlayerState.AwaitingRoundStart;
		[SyncVar(hook = "OnWinsChange")]
		public int wins = 0;

		private Vector3 spawnPosition;
		private Quaternion spawnRotation;
		private float remainingDeathTimeout = float.MaxValue;
		private ModularCrosshair crosshair;
		private Gui gui;

		private GameContext ctx;

		void Start() {
			ctx = GameContext.instance;
		}

		[Command]
		public void CmdSetPlayerName(string name) {
			playerName = name;
		}

		[ClientRpc]
		public void RpcInitPlayer(Vector3 spawnPos, Quaternion spawnRot) {
			transform.parent = GameObject.Find ("Game").transform;
			spawnPosition = spawnPos;
			spawnRotation = spawnRot;
			transform.position = spawnPosition;
			transform.rotation = spawnRotation;
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

		[ClientRpc]
		public void RpcResetPosition() {
			print ("Resetting position to " + spawnPosition);
			transform.position = spawnPosition;
			transform.rotation = spawnRotation;
		}

		[ClientRpc]
		public void RpcVisualizeMotionSelectionReady() {
			if (isLocalPlayer) {
				gui.ShowMoveInstruction(0.5f);
				gui.ShowTime(GameContext.instance.GetComponent<GameStateRound>().timeout);
			}
		}

		[ClientRpc]
		public void RpcVisualizeMotionSelected(Target target) {
			if (isLocalPlayer) {
				crosshair.ShowMoveIdleCrosshair();
			}
			Camera.main.GetComponent<ImageEffectShockwave>().Shock(target.position);
		}

		[ClientRpc]
		public void RpcVisualizeMotionMissed() {
			if (isLocalPlayer) {
				crosshair.ShowMoveDeniedCrosshair();
			}
		}

		[ClientRpc]
		public void RpcVisualizeShotReady() {
			if (isLocalPlayer) {
				gui.ShowShootInstruction(0.5f);
				crosshair.ShowShootCrosshair();
			}
		}

		[ClientRpc]
		public void RpcVisualizeShotHit() {
			if (isLocalPlayer) {
				Camera.main.GetComponent<ImageEffectSuperformula>().Shoot();
				crosshair.ShowShotFiredCrosshair();
			}
		}

		[ClientRpc]
		public void RpcVisualizeShotMissed() {
			if (isLocalPlayer) {
				crosshair.ShowShotFiredCrosshair();
			}
		}

		// This is called once by the server so it immediately has the right positions
		// for the players after respawn, do not use otherwise
		public void ResetPosition() {
			transform.position = spawnPosition;
			transform.rotation = spawnRotation;
		}

		[Command]
		public void CmdSelected(Vector3 direction) {
			ctx.Selected(this, direction);
		}

		/*[Command]
		void CmdSelectedTarget(Target target) {
			ctx.PlayerSelectedTarget(this, target);
		}

		void MotionFinished (Player movedPlayer) {
			ctx.PlayerFinishedMotion(this);

			if (isLocalPlayer) {
				CmdMotionFinished();
			}
		}

		[Command]
		void CmdMotionFinished () {
			ctx.PlayerFinishedMotion(this);
		}
		
		void SelectedTarget(Target target) {
			ctx.PlayerSelectedTarget(this, target);

			if(isClient) {
				CmdSelectedTarget(target);
			}

			if (state == PlayerState.SelectingShot) {
				SendMessage ("SetShootTarget", target);
				CmdSetState (PlayerState.ExecutingShot);

                ImageEffectSuperformula i = Camera.main.GetComponent<ImageEffectSuperformula>();
                if (i != null)
                {
                    i.Shoot();
                }

				crosshair.ShowShootCrosshair();
				gui.ShowShootInstruction(0.5f);
            } else {
				SendMessage ("SetFlyTarget", target);
                if (!Motion.allowMoveDebugStatic)
                {
                    CmdSetState(PlayerState.SelectedMotion);
                }

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
		*/

		[Command]
		public void CmdWon() {
			++wins;
		}

		[Command]
		public void CmdSetState(PlayerState state) {
			this.state = state;
		}

		void OnNameChange(string newName) {
			playerName = newName;
			gui.UpdatePlayerNames(ctx);
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

        void OnApplicationFocus(bool focus)
        {
            if (focus)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
