using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Kunstharz
{
	public struct FinderEntry {
		public string hostname;
		public string beacon;
		public float lastBeaconTime;

		public override string ToString ()
		{
			return beacon + "[ " + hostname + " ]";
		}
	}

	public class Finder : MonoBehaviour
	{
		public float beaconExpireTime = 1.5f;

		private Socket sock;
		private byte[] buf = new byte[1024];
		private List<FinderEntry> entries = new List<FinderEntry> ();

		public ICollection<FinderEntry> found {
			get {
				return entries.AsReadOnly ();
			}
		}

		void OnEnable() {
			if (sock != null) {
				OnDisable ();
			}
				
			sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			sock.Blocking = false;
			sock.SetSocketOption (SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption (NetworkSpecs.PING_ADDRESS, IPAddress.Any));
			sock.Bind (new IPEndPoint (IPAddress.Any, NetworkSpecs.PING_PORT));

			Application.runInBackground = true;
		}

		/**
		 * Automatically called when destroyed or disabled, cleans up
		 */
		void OnDisable() {
			sock.Close ();
			sock = null;
			entries.Clear ();
		}

		void CleanExpiredEntries() {
			int removed = entries.RemoveAll (e => (Time.time - e.lastBeaconTime) > beaconExpireTime);
			if (removed > 0) {
				SendFinderEntriesChanged ();
			}
		}

		void AddEntry(FinderEntry entry) {
			// Remove expired and old entries of same host
			int identicalHostnameCount = entries.RemoveAll (e => e.hostname == entry.hostname);
			entries.Add (entry);

			if (identicalHostnameCount == 0) {
				print ("Discovered game: " + entry);
			}

			SendFinderEntriesChanged ();
		}

		void SendFinderEntriesChanged() {
			SendMessageUpwards ("FinderEntriesChanged", found, SendMessageOptions.DontRequireReceiver);
		}

		void Update() {
			CleanExpiredEntries ();

			if (sock.Available > 0) {
				EndPoint sender = new IPEndPoint (IPAddress.Any, 0);
				int receivedBytes = sock.ReceiveFrom (buf, ref sender);
				// Just the address without beacon port
				string hostname = ((IPEndPoint)sender).Address.ToString ();
				string beacon = Encoding.UTF8.GetString (buf, 0, receivedBytes);

				FinderEntry entry = new FinderEntry ();
				entry.hostname = hostname;
				entry.beacon = beacon;
				entry.lastBeaconTime = Time.time;

				AddEntry (entry);
			}
		}
	}
}

