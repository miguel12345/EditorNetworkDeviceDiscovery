using UnityEngine;
using System.Collections;
using System.Net;
using System;

namespace NetworkDeviceDiscovery {

	[Serializable]
	public class Device
	{
		public string Name;
		public string Model;
		public RuntimePlatform Platform;
		public string IPAddress;

		public string DisplayName {
			get {
				//SystemInfo.deviceName seems to always return <unknown> on Android
				//and SystemInfo.deviceModel seems to return a reasonable user-facing name
				if (Platform == RuntimePlatform.Android) {
					return Model;
				}

				return Name;
			}
		}

		public static Device CurrentDevice {
			get {
				Device device = new Device ();
				device.Name = SystemInfo.deviceName;
				device.Platform = Application.platform;
				device.IPAddress = NetworkUtils.NetworkLocalIpAdress;
				device.Model = SystemInfo.deviceModel;
				return device;
			}
		}

		public override string ToString ()
		{
			return string.Format ("{0} - {1} - {2}", DisplayName, Platform,IPAddress);
		}
	}

}