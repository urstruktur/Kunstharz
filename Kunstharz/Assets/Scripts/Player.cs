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
			Debug.Log ("Debug: " + str + " has been shot!");
			SendMessage (str + " has been shot!");
		}

		public class MyMsgType {
			public static short Message = MsgType.Highest + 1;
		};

		public class ScoreMessage : MessageBase {
			public string message;
		}

		public void SendMessage(string strg) {
			ScoreMessage msg = new ScoreMessage();
			msg.message = strg;

			NetworkServer.SendToAll(MyMsgType.Message, msg);
		}
	}
}
