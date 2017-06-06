using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Text;
using UnityEngine;

namespace Kunstharz
{
	public class Publisher : MonoBehaviour
	{
		public String beacon = "THOMAS";
		public float beaconInterval = 0.5f;

		private Socket sock;

		private byte[] buf = new byte[1024];

		private float timeUntilBeacon;

		void OnEnable() {
			if (sock != null) {
				OnDisable ();
			}

			print ("Starting Publisher…");
			sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			sock.Blocking = false;
			NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface adapter in nics)
			{
				IPInterfaceProperties ip_properties = adapter.GetIPProperties();
				if (adapter.GetIPProperties().MulticastAddresses.Count == 0)
					continue; // most of VPN adapters will be skipped
				if (!adapter.SupportsMulticast)
					continue; // multicast is meaningless for this type of connection
				if (OperationalStatus.Up != adapter.OperationalStatus)
					continue; // this adapter is off or not connected
				IPv4InterfaceProperties p = adapter.GetIPProperties().GetIPv4Properties();
				if (null == p)
					continue; // IPv4 is not configured on this adapter

				// now we have adapter index as p.Index, let put it to socket option
				sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, (int)IPAddress.HostToNetworkOrder(p.Index));
			}
			sock.SetSocketOption (SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption (NetworkSpecs.PING_ADDRESS));
			// Traverse a maximum of one router to another network when sending multicast data
			sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);
			sock.Connect (new IPEndPoint(NetworkSpecs.PING_ADDRESS, NetworkSpecs.PING_PORT));
			timeUntilBeacon = 0;

			Application.runInBackground = true;
		}

		/**
		 * Automatically called when destroyed or disabled, cleans up
		 */
		void OnDisable() {
			print ("Shutting down Publisher…");
			sock.Close ();
			sock = null;
		}

		void SendBeacon() {
			byte[] beaconData = Encoding.UTF8.GetBytes ((beacon.Length > 0) ? beacon : "n/a");
			sock.Send (beaconData);
		}

		void SendBeaconIfIntervalExpired() {
			if (timeUntilBeacon <= 0) {
				SendBeacon ();
				timeUntilBeacon = beaconInterval;
			} else {
				timeUntilBeacon -= Time.deltaTime;
			}
		}

		void Update() {
			//if (beacon.Length > 0) {
				SendBeaconIfIntervalExpired ();
			//}
		}
	}
}

