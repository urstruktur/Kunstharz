﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Text;
using UnityEngine;

namespace Kunstharz
{
	public struct FinderEntry {
		public string hostname;
		public float lastBeaconTime;
		public Challenge challenge;

		public override string ToString ()
		{
			return challenge + "@" + hostname;
		}
	}

	public class Finder : MonoBehaviour
	{
		public float beaconExpireTime = 1.5f;
		public List<FinderEntry> entries = new List<FinderEntry> ();

		private Socket sock;
		private byte[] buf = new byte[1024];

		public delegate void FinderDelegateChallengeExpried(int i);
		public static FinderDelegateChallengeExpried ChallengeExpired;

		public delegate void FinderDelegateChallengeDiscovered(FinderEntry entry);
		public static FinderDelegateChallengeDiscovered ChallengeDiscovered;

		public delegate void FinderDelegateFinderEntriesChanged(List<FinderEntry> entries);
		public static FinderDelegateFinderEntriesChanged FinderEntriesChanged;

		void OnEnable() {
			if (sock != null) {
				OnDisable ();
			}
				
			sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			sock.Blocking = false;
			sock.SetSocketOption (SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption (NetworkSpecs.PING_ADDRESS, IPAddress.Any));
			sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, NetworkSpecs.MULTICAST_TTL);

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
			int removed = 0;

			for (int i = entries.Count - 1; i >= 0; --i) {
				if ((Time.time - entries [i].lastBeaconTime) > beaconExpireTime) {
					++removed;
					entries.RemoveAt (i);
					print ("Discovered game expired at index: " + i);
					ChallengeExpired(i);
				}
			}

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
				ChallengeDiscovered(entry);
			}

			SendFinderEntriesChanged ();
		}

		void SendFinderEntriesChanged() {
			SendMessageUpwards ("FinderEntriesChanged", entries, SendMessageOptions.DontRequireReceiver);
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
				entry.challenge = JsonUtility.FromJson<Challenge>(beacon);
				entry.lastBeaconTime = Time.time;

				AddEntry (entry);
			}
		}
	}
}

