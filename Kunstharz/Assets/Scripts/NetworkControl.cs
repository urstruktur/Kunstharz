using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace Kunstharz
{
	public class NetworkControl : NetworkManager
	{
		public string selectedLevelName;

		private List<NetworkConnection> connections = new List<NetworkConnection> ();
		private List<short> playerControllerIds = new List<short> ();

		private int connectedCount = 0;

		public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
		{
			connections.Add (conn);
			playerControllerIds.Add (playerControllerId);

			++connectedCount;
			if (connectedCount == 2) {
				var essentialsPrefab = spawnPrefabs [0];
				var levelPrefab = spawnPrefabs [1];

				NetworkServer.Spawn (Instantiate (essentialsPrefab));
				var level = Instantiate (levelPrefab);
				NetworkServer.Spawn (level);

				for (int i = 0; i < connections.Count; ++i) {
					NetworkConnection playerConn = connections[i];
					short playerId = playerControllerIds[i];
					base.OnServerAddPlayer(playerConn, playerId);
				}

				var players = GameObject.FindGameObjectsWithTag ("Player");
				var startPositions = level.GetComponentsInChildren<NetworkStartPosition> ();
				int idx = 0;
				foreach(var aPlayer in players) {
					Transform spawn = startPositions[idx].transform;
					aPlayer.GetComponent<Player> ().RpcInitPlayer (spawn.position, spawn.rotation);
					++idx;
				}

			}
		}
	}
}

