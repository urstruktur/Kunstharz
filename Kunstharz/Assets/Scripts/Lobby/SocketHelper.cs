using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;

public static class SocketHelper {
	
	public static UdpClient CreateUDPServer(int port, Action<IPEndPoint, byte[]> messageReceived){

		UdpClient listener = new UdpClient(port);
		IPEndPoint groupEndPoint = new IPEndPoint(IPAddress.Any, port);

		UnityThreadHelper.CreateThread(() => {
			try{
				while(true){
					byte[] receivedBytes = listener.Receive(ref groupEndPoint);
					messageReceived(groupEndPoint, receivedBytes);
				}
			} catch (Exception e) {
				UnityThreadHelper.Dispatcher.Dispatch(() => {
					Debug.Log(e);
				});
			}
			finally {
				try{
					listener.Close();
				} catch (Exception e){
					UnityThreadHelper.Dispatcher.Dispatch(() => {
						Debug.Log(e);
					});
				}
			}

		});

		return listener;
	}

	public static bool CheckVersion(Socket handler, int version) {
        byte[] versionBuffer = new byte[1];
        handler.Receive(versionBuffer);
        return CheckVersion(ref versionBuffer, version);
    }

    public static bool CheckVersion(ref byte[] message, int version) {
        if (message.Length < 1) {
            return false;
        }

        byte versionByte = message[0];

        byte[] newMessage = new byte[message.Length - 1];
        Array.Copy(message, 1, newMessage, 0, message.Length - 1);
        message = newMessage;

        return (versionByte == version);
    }

}
