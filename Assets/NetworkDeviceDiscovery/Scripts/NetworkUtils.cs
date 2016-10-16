using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace NetworkDeviceDiscovery {
	public class NetworkUtils
	{
		//I was having trouble getting the correct IP address
		//through System.Net methods so I had to go through
		//the native route
		#if UNITY_IOS && !UNITY_EDITOR

		[DllImport ("__Internal")]
		private static extern string NDDGetIPAddress();

		static string GetLocalIPAddress() {
			return NDDGetIPAddress();
		}

		public static string NetworkLocalIpAdress { get { return GetLocalIPAddress(); } }

		#else

		public static string NetworkLocalIpAdress {
			get {
				IPHostEntry host;
				string localIP = "";
				host = Dns.GetHostEntry(Dns.GetHostName());
				foreach (IPAddress ip in host.AddressList)
				{
					if (ip.AddressFamily == AddressFamily.InterNetwork)
					{
						localIP = ip.ToString();
						break;
					}
				}
				return localIP;
			}
		}

		#endif
	}

}