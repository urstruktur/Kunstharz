using System.Net;

namespace Kunstharz
{
	public class NetworkSpecs
	{
		/** Multicast address where pings are sent to */
		public static IPAddress PING_ADDRESS = IPAddress.Parse("239.10.4.20");

		public const int PING_PORT = 51244;

		/** Port that the host uses for the networkmanager */
		public const int GAME_HOST_PORT = 51255;
	}
}

