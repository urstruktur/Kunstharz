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
