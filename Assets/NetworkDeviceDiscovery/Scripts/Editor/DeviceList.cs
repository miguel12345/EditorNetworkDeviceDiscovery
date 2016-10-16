using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEditor;

namespace NetworkDeviceDiscovery {

	public class DeviceList {

		public Device SelectedDevice {get; private set;}

		Probe probe;

		public DeviceList(Probe probe) {
			this.probe = probe;
		}

		public void Draw() {
			
			var connectedDevices = probe.ConnectedDevices;
			var selectedDeviceIndex = 0;
			if (SelectedDevice != null)
				selectedDeviceIndex = connectedDevices.IndexOf (SelectedDevice);
			if (selectedDeviceIndex < 0)
				selectedDeviceIndex = 0;

			selectedDeviceIndex = EditorGUILayout.Popup (selectedDeviceIndex, connectedDevices.Select (device => device.ToString()).ToArray());

			if (selectedDeviceIndex >= 0 && selectedDeviceIndex < connectedDevices.Count)
				SelectedDevice = connectedDevices [selectedDeviceIndex];
		}
	}
}