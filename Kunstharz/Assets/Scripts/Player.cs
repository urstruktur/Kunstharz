﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Kunstharz {
	public class Player : NetworkBehaviour {
		[SyncVar(hook = "OnStateChange")]
		public PlayerState state = PlayerState.SelectingMotion;

		void Start() {
            // set player as child of game
            var game = GameObject.Find ("Game").transform;
			transform.parent = game;
			SendMessageUpwards ("PlayerJoined", this);
		}

		void SelectedTarget(Target target) {
			if (state == PlayerState.SelectingShot) {
				SendMessage ("SetShootTarget", target);
			} else {
				SendMessage ("SetFlyTarget", target);
				CmdSetState (PlayerState.SelectedMotion);
			}
		}

		void MotionFinished (Player movedPlayer) {
			if (isLocalPlayer) {
				CmdSetState (PlayerState.ExecutedMotion);
			}
		}

		void HitPlayer(Player player) {
			CmdSetState (PlayerState.Victorious);
		}

		[Command]
		public void CmdSetState(PlayerState state) {
			this.state = state;
		}

		void OnStateChange(PlayerState state) {
			PlayerState oldState = this.state;
			this.state = state;
			Debug.Log ("Changed from " + oldState + " to " + state);
			SendMessageUpwards ("PlayerStateChanged", this);
		}

		[Command]
		public void CmdShot(string str) {
			RpcShot (str);
		}

		[ClientRpc]
		public void RpcShot(string str) {
			Debug.Log ("Debug: " + str + " has been shot!");
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
