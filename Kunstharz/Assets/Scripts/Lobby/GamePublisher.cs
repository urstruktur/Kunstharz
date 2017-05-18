using UnityEngine;
using System;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Kunstharz
{

    public class GamePublisher : MonoBehaviour
    {

        // CONNECTION PORTS
        private enum PORTS
        {
            PING_REQUEST = 11001,
            PING_RESPONSE = 11002
        }

        // CONNECTION VERSIONS
        private enum VERSION
        {
            PING = 1
        }

        private UdpClient pingSocket = new UdpClient();

        // Use this for initialization
        void Start()
        {
            pingSocket = SocketHelper.CreateUDPServer((int)PORTS.PING_REQUEST, (endPoint, receivedBytes) =>
            {
                if (SocketHelper.CheckVersion(ref receivedBytes, (int)VERSION.PING))
                {
                    HandlePingConnection(endPoint);
                }
            });
        }

        void OnEnable()
        {

        }

        void OnDisable()
        {

        }

        // ---------------- private methods ------------------------

        private void HandlePingConnection(IPEndPoint endPoint)
        {
            string gameName = "Kunstharz Testname";

            byte[] gameNameData = UTF8Encoding.UTF8.GetBytes(gameName);
            byte[] gameNameLengthData;

            try
            {
                gameNameLengthData = BitConverter.GetBytes(Convert.ToUInt16(gameNameData.Length));
            }
            catch (OverflowException e)
            {
                gameName = gameName.Substring(0, 8);
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
    }
}
