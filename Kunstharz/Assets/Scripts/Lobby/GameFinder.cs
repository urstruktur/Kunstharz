using UnityEngine;
using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace Kunstharz
{

    public class GameFinder : MonoBehaviour
    {

        // -------------- Game Class ----------------------------
        private class KunstharzGame
        {
            public IPAddress address { get; set; }
            public bool isAlive { get; set; }
            public KunstharzGame(IPAddress address)
            {
                this.address = address;
                isAlive = true;
            }
        }

        // CONNECTION INTERVALES
        private enum INTERVALES
        {
            CHECK_GAME = 2000,
            PING = 500,
            SEARCH_FOR_GAMES = 3000
        }

        // CONNECTION PORTS
        private enum PORTS
        {
            PING_REQUEST = 11001,
            PING_RESPONSE = 11002
        }

        // CONNECTION COMMANDS
        private enum COMMANDS
        {
            INVALID_VERSION = 255,
            NO_PORT_AVAILABLE = 254,
            GAME_CLOSED = 0
        }

        // CONNECTION VERSIONS
        private enum VERSION
        {
            PING = 1
        }

        // DICT FOR ALL GAMES
        private volatile Dictionary<string, KunstharzGame> games = new Dictionary<string, KunstharzGame>();
        private const string NO_GAMES = "NO GAME FOUND SO FAR!";

        private UdpClient pingListener;
        private IPAddress ipAddress;

        [SerializeField]
        private bool searchingForGames;

        private Thread searchThread;
        private Thread checkThread;

        void Awake()
        {
            ipAddress = IPAddress.Parse(Network.player.ipAddress);
        }

        // Use this for initialization
        void Start()
        {
        }

        void OnEnable()
        {
            Debug.Log("Enable finder...");
            pingListener = new UdpClient();

            pingListener = SocketHelper.CreateUDPServer((int)PORTS.PING_RESPONSE, (endPoint, receivedBytes) =>
            {
                FoundGame(endPoint, receivedBytes);
            });

            SearchForGames();
            CheckExistingGames();
        }

        void OnDisable()
        {
            Debug.Log("Disable finder...");
            searchThread.Abort();
            checkThread.Abort();
            pingListener.Close();
        }

        // -------------------  private methods  -----------------------


        private void SearchForGames()
        {
            Debug.Log("Searching for games...");
            searchThread = new Thread(() =>
             {
                 while (searchingForGames)
                 {
                     Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                     string ipAddressString = this.ipAddress.ToString();
                     string[] ipAddressSubStrings = ipAddressString.Split('.');

                     Debug.Log(ipAddress);

                     if (ipAddressSubStrings.Length != 4)
                     {
                         Debug.Log("Wrong IP-Address");
                     }

                     string newIpAddress = ipAddressSubStrings[0] + "." + ipAddressSubStrings[1] + "." + ipAddressSubStrings[2] + ".";

                     for (int i = 0; i < 256; i++)
                     {
                         IPAddress ipAddress = IPAddress.Parse(newIpAddress + i);
                         try
                         {
                             socket.EnableBroadcast = true;
                             socket.SendTo(new byte[] { (byte)VERSION.PING }, new IPEndPoint(ipAddress, (int)PORTS.PING_REQUEST));
                         }
                         catch (SocketException e)
                         {
                             Debug.Log(e);
                         }
                     }

                     socket.Close();
                     Thread.Sleep((int)INTERVALES.SEARCH_FOR_GAMES);
                 }
             });

             searchThread.IsBackground = true;
             searchThread.Start();
        }

        private void CheckExistingGames()
        {
            Debug.Log("Check existing games...");
            checkThread = new Thread(() => 
            {
                lock (games)
                {
                    Dictionary<string, KunstharzGame> gamesCopy = new Dictionary<string, KunstharzGame>(games);
                    foreach (KeyValuePair<string, KunstharzGame> game in gamesCopy)
                    {
                        if (!game.Value.isAlive)
                        {
                            KunstharzGame kunstgame = games[game.Key];
                            games.Remove(game.Key);
                        }
                    }
                }
            });

            checkThread.IsBackground = true;
            checkThread.Start();
        }

    private void FoundGame(IPEndPoint endPoint, byte[] receivedBytes)
    {
        
        byte[] gameNameLengthBuffer = new byte[2];
        Array.Copy(receivedBytes, 0, gameNameLengthBuffer, 0, 2);
        UInt16 gameNameLength = BitConverter.ToUInt16(gameNameLengthBuffer, 0);

        byte[] gameNameBuffer = new byte[gameNameLength];
        Array.Copy(receivedBytes, 2, gameNameBuffer, 0, gameNameLength);
        string gameName = System.Text.Encoding.UTF8.GetString(gameNameBuffer);

        Debug.Log("Found game: " + gameName);

        lock (games)
        {
            if (!games.ContainsKey(gameName))
            {
                games.Add(gameName, new KunstharzGame(endPoint.Address));
            }
            else
            {
                KunstharzGame kunstharzGame = games[gameName];
                kunstharzGame.isAlive = true;
            }
        }
    }

    private bool CheckVersion(Socket handler, int version)
    {
        byte[] versionBuffer = new byte[1];
        handler.Receive(versionBuffer);
        return (versionBuffer[0] == version);
    }


}
}
