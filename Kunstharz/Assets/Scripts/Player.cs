using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Kunstharz {
	public class Player : NetworkBehaviour {

		void Start() {
            // set player as child of game
            var game = GameObject.Find ("Game").transform;
			transform.parent = game;
			SendMessageUpwards ("PlayerJoined", this);
		}

		/*[Command]
		public void CmdShot(string str) {
			SendMessage (str + " has been shot!");
		}*/

		[ClientRpc]
		public void RpcShot(string str) {
			Debug.Log ("Debug: " + str + " has been shot!");
		}

	}
}
