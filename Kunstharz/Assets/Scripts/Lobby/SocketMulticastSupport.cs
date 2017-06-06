using System;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using UnityEngine;

namespace Kunstharz
{
	public static class SocketMulticastSupport
	{
		public static void EnsureNetworkInterfaceSupportsMulticast (this Socket sock)
		{
			int supportingIdx = -1;

			NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface adapter in nics)
			{
				IPInterfaceProperties ip_properties = adapter.GetIPProperties();
				Debug.Log ("" + adapter + "   " + adapter.SupportsMulticast);
				if (adapter.GetIPProperties ().MulticastAddresses.Count > 0 &&
				    adapter.SupportsMulticast &&
					adapter.OperationalStatus == OperationalStatus.Up) {

					IPv4InterfaceProperties p = adapter.GetIPProperties().GetIPv4Properties();
					if (p != null) {
						Debug.Log ("Setting interface " + adapter.Name + " as multicast interface");
						supportingIdx = (int)IPAddress.HostToNetworkOrder (p.Index);
					}
				}
			}

			if (supportingIdx == -1) {
				Debug.Log ("No multicast supporting interface found, letting implementation choose interface");
			} else {
				sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, supportingIdx);
			}
		}
	}
}

