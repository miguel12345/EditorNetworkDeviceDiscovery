using UnityEngine;
using System.Collections;
using System;

namespace NetworkDeviceDiscovery {

	[Serializable]
	public class BeaconDevice
	{
		public Device Device;
		public int Port;
		public int LastDetectionTime;

		[SerializeField]
		bool IsValidDevice;

		public bool IsValid {
			get {
				return IsValidDevice;
			}
		}

		public BeaconDevice(Device device,int Port) {
			this.Device = device;
			this.Port = Port;
			this.IsValidDevice = true;
		}

		public override string ToString ()
		{
			return string.Format ("[NetworkBeaconDevice: Device={0}, Port={1}]", Device,Port);
		}

		public override bool Equals (object obj)
		{
			if (obj is BeaconDevice) {
				BeaconDevice other = (BeaconDevice)obj;
				return other.Device.IPAddress.Equals (this.Device.IPAddress) && this.Port == other.Port;
			}

			return false;
		}

		public override int GetHashCode ()
		{
			return Device.IPAddress.GetHashCode () ^ Port;
		}
	}

}