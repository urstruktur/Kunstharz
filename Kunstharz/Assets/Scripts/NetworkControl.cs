using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Kunstharz
{
	public class NetworkControl : NetworkManager
	{
		public string selectedLevelName;

		private int connectedCount = 0;

		public override void OnServerConnect(NetworkConnection conn) {
			if (connectedCount < 2) {
				base.OnServerConnect (conn);

				++connectedCount;

				print ("Connected: " + connectedCount);

				if (connectedCount == 2) {
					print ("Changing scene");
					autoCreatePlayer = true;
					ServerChangeScene (selectedLevelName);
				}
			} else {
				print ("Got a connection, but already have two");
			}
		}

		public override void OnServerSceneChanged (string sceneName) {
			var essentialsPrefab = spawnPrefabs [0];
			NetworkServer.Spawn (Instantiate(essentialsPrefab));
		}

		public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
		{
			if (GameObject.Find ("Game") != null) {
				/*GameObject player = (GameObject)Instantiate (playerPrefab);
				NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);*/
				base.OnServerAddPlayer (conn, playerControllerId);

				var players = GameObject.FindGameObjectsWithTag ("Player");
				if (players.Length == 2) {
					foreach(var aPlayer in players) {
						aPlayer.GetComponent<Player> ().RpcInitPlayer ();
					}
				}
			}
		}
	}
}

