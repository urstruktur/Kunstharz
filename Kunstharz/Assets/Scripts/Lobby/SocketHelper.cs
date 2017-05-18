using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public static class SocketHelper
{

    public static UdpClient CreateUDPServer(int port, Action<IPEndPoint, byte[]> messageReceived)
    {
        UdpClient listener = new UdpClient(port);
        IPEndPoint groupEndPoint = new IPEndPoint(IPAddress.Any, port);

        Thread listenThread = new Thread(() =>
		{
			while (true)
			{
				try
				{
					byte[] receivedBytes = listener.Receive(ref groupEndPoint);
                	messageReceived(groupEndPoint, receivedBytes);
				}
				catch (ThreadInterruptedException)
				{
					break; 
				}
				catch (SocketException)
				{
					break; 
				}
				// TODO: is this possible?
				finally
				{
					try
					{
						listener.Close();
					}
					catch (Exception e)
					{
						Debug.Log(e);
					}
				}
			}
		});

        listenThread.IsBackground = true;
        listenThread.Start();

        return listener;
    }

    public static bool CheckVersion(Socket handler, int version)
    {
        byte[] versionBuffer = new byte[1];
        handler.Receive(versionBuffer);
        return CheckVersion(ref versionBuffer, version);
    }

    public static bool CheckVersion(ref byte[] message, int version)
    {
        if (message.Length < 1)
        {
            return false;
        }

        byte versionByte = message[0];

        byte[] newMessage = new byte[message.Length - 1];
        Array.Copy(message, 1, newMessage, 0, message.Length - 1);
        message = newMessage;

        return (versionByte == version);
    }

}
