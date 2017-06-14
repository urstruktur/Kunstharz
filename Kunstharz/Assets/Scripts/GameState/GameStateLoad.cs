using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Kunstharz
{
	public class GameStateLoad : NetworkBehaviour, IGameState
	{
		public const int IDX = 0;

		public GameObject essentialsPrefab;
		public GameObject playerPrefab;
		public GameObject[] levelPrefabs;

		private NetworkStartPosition[] startPositions;

		private GameContext ctx;

		public void Enter(GameContext ctx) {
			this.ctx = ctx;
			print("Entering load state");

			HideMainMenu();

			if(isServer) {
				//RegisterSpawnPrefabs();
				SpawnEnvironment();
				SpawnPlayers();
				StartCoroutine("ChangeToRoundNextFrame");
			}
		}

		IEnumerator ChangeToRoundNextFrame() {
			yield return new WaitForEndOfFrame();
			ctx.currentStateIdx = GameStateRound.IDX;
		}

		public void Exit(GameContext ctx) {
			print("Exiting load state");
		}

		void RegisterSpawnPrefabs() {
			ClientScene.RegisterPrefab(essentialsPrefab);
			ClientScene.RegisterPrefab(playerPrefab);
			foreach(var level in levelPrefabs) {
				ClientScene.RegisterPrefab(level);
			}
		}

		void SpawnEnvironment() {
			NetworkServer.Spawn (Instantiate (essentialsPrefab));

			var levelIdx = GameObject.Find("Menu Script").GetComponent<Menu> ().selectedLevelIdx;
			var levelPrefab = levelPrefabs[levelIdx];

			var level = Instantiate (levelPrefab);
			NetworkServer.Spawn (level);

			startPositions = level.GetComponentsInChildren<NetworkStartPosition> ();
		}

		void SpawnPlayers() {
			int idx = 0;
			foreach(NetworkConnection conn in NetworkServer.connections) {
				var playerGO = Instantiate (playerPrefab);
				NetworkServer.AddPlayerForConnection(conn, playerGO, 0);
				Transform spawn = startPositions[idx].transform;
				var player = playerGO.GetComponent<Player> ();
				player.RpcInitPlayer (spawn.position, spawn.rotation);
				++idx;
			}
		}

		void HideMainMenu() {
			// DEBUG
			GameObject.Destroy(GameObject.Find("General"));
			GameObject.Destroy(GameObject.Find("Cameras"));
			GameObject.Destroy(GameObject.Find("General Canvas"));
			GameObject.Destroy(GameObject.Find("Effect Canvas"));
			GameObject.Destroy(GameObject.Find("NetworkDiscovery"));
		}

		public void PlayerSelectedTarget(GameContext ctx, Player selectingPlayer, Target target) {
			throw new NotImplementedException("When loading, did not expect to see action");
		}

		public void PlayerSelectedPlayer(GameContext ctx, Player selectingPlayer, Player selectedPlayer) {
			throw new NotImplementedException("When loading, did not expect to see action");
		}

		public void PlayerFinishedMotion(GameContext ctx, Player player) {
			throw new NotImplementedException("When loading, did not expect to see action");
		}
	}
}

