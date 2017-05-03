using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

namespace Kunstharz{
	public class TestLobbyManager : MonoBehaviour {

		// -------------- Game Class ----------------------------
		private class KunstharzGame {
			public IPAddress address { get; set; }
			public bool isAlive { get; set; }
			public KunstharzGame(IPAddress address){
				this.address = address;
				isAlive = true;
			}
		}

		public enum ConnectionResponse { Connected, NoPortAvailable, InvalidVersion, InvalidProtocol, Error };

		// CONNECTION INTERVALES
		private enum INTERVALES {
			PING = 500,
			CHECK_GAME = 2000,
			SEARCH_FOR_GAMES = 3000
		}
		// CONNECTION PORTS
		private enum PORTS {
			REGISTER = 11000,
			PING_REQUEST = 11001,
			PING_RESPONSE = 11002,
			CONNECTION = 11003
		}
		// CONNECTION COMMANDS
		private enum COMMANDS {
			REGISTER_SUCCESS = 1,
			INVALID_VERSION = 255,
			NO_PORT_AVAILABLE = 254,
			GAME_CLOSED = 0
		}
		// CONNECTION VERSIONS
		private enum VERSION {
			PING = 1,
			REGISTER = 1
		}
		// DICT FOR ALL GAMES
		private volatile Dictionary<string, KunstharzGame> games = new Dictionary<string, KunstharzGame>();
		private const string NO_GAMES = "NO GAME FOUND SO FAR!";
		
		// CONNECTION PROPERTIES
		private UdpClient pingListener = new UdpClient();
		private UdpClient pingSocket = new UdpClient();
		private IPAddress ipAddress;
		
		[SerializeField]
		public bool searchingForGames;

		//NetworkClient client;

		void Awake () {
			ipAddress = IPAddress.Parse(Network.player.ipAddress);
		}

		// Use this for initialization
		void Start () {
			pingListener = SocketHelper.CreateUDPServer((int)PORTS.PING_RESPONSE, (endPoint, receivedBytes) => {
				FoundGame(endPoint, receivedBytes);
			});

			pingSocket = SocketHelper.CreateUDPServer((int)PORTS.PING_REQUEST, (endPoint, receivedBytes) => {
				if (SocketHelper.CheckVersion(ref receivedBytes, (int)VERSION.PING)){
					HandlePingConnection(endPoint);
				}
        	});	

			SearchForGames();
		}
		
		// Update is called once per frame
		void Update () {
			
		}

		void OnApplicationQuit(){
			searchingForGames = false;
			Debug.Log("Application is quitting, all Sockets will be closed!");
			pingListener.Close();
			pingSocket.Close();	
		}

		// ------------- Match Making Part -----------------------

		private void HandlePingConnection(IPEndPoint endPoint) {
			string gameName = "Kunstharz Testname"; 

			byte[] gameNameData = UTF8Encoding.UTF8.GetBytes(gameName);
			byte[] gameNameLengthData;
			
			try {
				gameNameLengthData = BitConverter.GetBytes(Convert.ToUInt16(gameNameData.Length));
			} catch(OverflowException e) {
				gameName = gameName.Substring(0,8);
				gameNameData = UTF8Encoding.UTF8.GetBytes(gameName);
				gameNameLengthData = BitConverter.GetBytes(Convert.ToUInt16(gameNameData.Length));
			}

			MemoryStream memoryStream = new MemoryStream();
			memoryStream.Write(gameNameLengthData, 0, gameNameLengthData.Length);
			memoryStream.Write(gameNameData, 0, gameNameData.Length);

			// Sending back with UPD Game Name
			Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			socket.SendTo(memoryStream.ToArray(), new IPEndPoint(endPoint.Address, (int)PORTS.PING_RESPONSE));
			socket.Close();
			
			memoryStream.Close();
		}

		public void SearchForGames() {

			UnityThreadHelper.CreateThread(() => {
				while (searchingForGames) {
					Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
					string ipAddressString = this.ipAddress.ToString();
					string[] ipAddressSubStrings = ipAddressString.Split('.');

					UnityThreadHelper.Dispatcher.Dispatch(() => {
							Debug.Log(ipAddress);
					});

					if(ipAddressSubStrings.Length != 4){
						UnityThreadHelper.Dispatcher.Dispatch(() => {
							Debug.Log("Wrong IP-Address");
						});
					}

					string newIpAddress = ipAddressSubStrings[0] + "." + ipAddressSubStrings[1] + "." + ipAddressSubStrings[2] + ".";

					for (int i = 0; i < 256; i++) {
						IPAddress ipAddress = IPAddress.Parse(newIpAddress + i);
						try {
							socket.EnableBroadcast = true;
							socket.SendTo(new byte[]{(byte)VERSION.PING}, new IPEndPoint(ipAddress, (int)PORTS.PING_REQUEST));
						} catch (SocketException e) {
							Debug.Log(e);
						}
					}

					socket.Close();
					Thread.Sleep((int)INTERVALES.SEARCH_FOR_GAMES);
				}

			});

			UnityThreadHelper.CreateThread(() => {
				while(searchingForGames){
					Thread.Sleep((int)INTERVALES.CHECK_GAME);

					lock (games) {
						Dictionary<string, KunstharzGame> gamesCopy = new Dictionary<string,KunstharzGame>(games);
						foreach (KeyValuePair<string, KunstharzGame> game in gamesCopy) {
							if(!game.Value.isAlive) {
								KunstharzGame kunstgame = games[game.Key];
								games.Remove(game.Key);
							}
						}
					}
				}
			});		
		}

		// PRIVATE METHODS

		private void FoundGame(IPEndPoint endPoint, byte[] receivedBytes){
			byte[] gameNameLengthBuffer = new byte[2];
			Array.Copy(receivedBytes, 0, gameNameLengthBuffer, 0, 2);
			UInt16 gameNameLength = BitConverter.ToUInt16(gameNameLengthBuffer, 0);

			byte[] gameNameBuffer = new byte[gameNameLength];
			Array.Copy(receivedBytes, 2, gameNameBuffer, 0, gameNameLength);
			string gameName = System.Text.Encoding.UTF8.GetString(gameNameBuffer);

			lock(games){
				if(!games.ContainsKey(gameName)){
					games.Add(gameName, new KunstharzGame(endPoint.Address));
				} else {
					KunstharzGame kunstharzGame = games[gameName];
					kunstharzGame.isAlive = true;
				}
			}
		}

		private bool CheckVersion(Socket handler, int version){
			byte[] versionBuffer = new byte[1];
			handler.Receive(versionBuffer);
			return (versionBuffer[0] == version);
    	}

		/* 
		// ------------- Server & Client Demo Setups -------------
		
		private void SetupServer () {
			NetworkServer.Listen(7070);
		}

		private void SetupClient () {
			client = new NetworkClient();
			client.RegisterHandler(MsgType.Connect, OnConnected);     
			client.Connect("127.0.0.1", 7070);
		}

		public void SetupLocalClient(){
			client = ClientScene.ConnectLocalServer();
			client.RegisterHandler(MsgType.Connect, OnConnected);
		}

		private void DoAllSetups () {
			SetupServer();
			SetupClient();
		}

		public void OnConnected (NetworkMessage netMsg) {
			Debug.Log("Connected to server");
		}*/
	}

}
