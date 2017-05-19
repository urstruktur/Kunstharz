using System;
using System.Net;
using System.Net.Sockets;
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
			sock.SetSocketOption (SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption (NetworkSpecs.PING_ADDRESS));
			// Traverse a maximum of one router to another network when sending multicast data
			sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 1);
			sock.Connect (new IPEndPoint(NetworkSpecs.PING_ADDRESS, NetworkSpecs.PING_PORT));
			timeUntilBeacon = 0;
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
			byte[] beaconData = Encoding.UTF8.GetBytes (beacon);
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
			if (beacon.Length > 0) {
				SendBeaconIfIntervalExpired ();
			}
		}
	}
}

