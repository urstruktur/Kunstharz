using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Kunstharz {
	public class Player : NetworkBehaviour {
		[SyncVar(hook = "OnNameChange")]
		public string playerName;

		[SyncVar(hook = "OnStateChange")]
		public PlayerState state = PlayerState.AwaitingRoundStart;

		[SyncVar]
		public DeathReason deathReason = DeathReason.None;

		[SyncVar(hook = "OnWinsChange")]
		public int wins = 0;

		[SyncVar]
		public bool approvesRematch = false;

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

		[ClientRpc]
		public void RpcResetPosition() {
			print ("Resetting position to " + spawnPosition);
			transform.position = spawnPosition;
			transform.rotation = spawnRotation;
		}

		[ClientRpc]
		public void RpcVisualizeMotionSelectionReady() {
			if (isLocalPlayer) {
				crosshair.ShowMoveCrosshair();
				gui.ShowInstruction(Gui.InstructionType.Move, 0.5f, false);
			}
		}

		[ClientRpc]
		public void RpcVisualizeTimeout(float timeout) {
			if(isLocalPlayer) {
				gui.ShowTime(timeout);
			}
		} 

		[ClientRpc]
		public void RpcVisualizeMotionSelected(Target target) {
			if (isLocalPlayer) {
				crosshair.ShowMoveIdleCrosshair();
				gui.HideTime();
			}
			Camera.main.GetComponent<ImageEffectShockwave>().Shock(target.position);
		}

		[ClientRpc]
		public void RpcVisualizeMotionMissed() {
			if (isLocalPlayer) {
				crosshair.ShowMoveDeniedCrosshair();

                Soundsystem ss = FindObjectOfType<Soundsystem>();
                if(ss != null)
                {
                    ss.playMotionMissed();
                }
			}
		}

		[ClientRpc]
		public void RpcVisualizeShotReady() {
			if (isLocalPlayer) {
				gui.ShowInstruction(Gui.InstructionType.Shoot, 0.5f, false);
                Camera.main.GetComponent<ImageEffectShockwave>().InvertImage();

                Soundsystem ss = FindObjectOfType<Soundsystem>();
                if (ss != null)
                {
                    ss.playLineOfSight();
                }

                crosshair.ShowShootCrosshair();
			}
		}

		[ClientRpc]
		public void RpcVisualizeShotHit() {
			if (isLocalPlayer) {
				Camera.main.GetComponent<ImageEffectSuperformula>().Shoot();
				crosshair.ShowShotFiredCrosshair();
            }

            Camera.main.GetComponent<ImageEffectShockwave>().NormalizeImage();
            Soundsystem ss = FindObjectOfType<Soundsystem>();
            if (ss != null)
            {
                ss.playShot(this.gameObject);
            }
        }

		[ClientRpc]
		public void RpcVisualizeShotMissed() {
			if (isLocalPlayer) {
				crosshair.ShowShotFiredCrosshair();
			}

            Soundsystem ss = FindObjectOfType<Soundsystem>();
            if (ss != null)
            {
                ss.playShot(this.gameObject);
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
			this.state = state;

			gui.UpdatePlayerStates (GameContext.instance);

			SendMessageUpwards ("PlayerStateChanged", this, SendMessageOptions.DontRequireReceiver);
		}

		void OnWinsChange(int wins) {
			this.wins = wins;
			SendMessageUpwards ("PlayerWon", this);
		}
        
        // only executed locally
        public override void OnStartLocalPlayer()
        {
            //gameObject.transform.FindChild("Mesh").GetComponent<MeshRenderer>().materials[1].color = Color.green;
            // hide own player
			foreach(var renderer in GetComponentsInChildren<MeshRenderer> ()) {
				renderer.enabled = false;
			}
            //gameObject.transform.FindChild("Mesh").GetComponent<MeshRenderer>().enabled = false;
            FindObjectOfType<Soundsystem>().playStartGame();
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
