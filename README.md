#Network Device Discovery

Network device discovery is a tool that allows you to easily get a list of devices that are currently in the same network.


## Why would I need this?

Let's say you creating your own Unity editor tool that needs to connect to a device running your game/app in order to communicate with it. To do that, you need to know the device's IP adress. Your default option is to ask the user for it but that is a time-consuming and frustating thing to do.

### Usage

With this tool, **you just need to perform four simple steps**:

* Install it by importing `NetworkDeviceDiscovery.unitypackage`
* Add `NetworkDeviceDiscovery/Prefabs/Beacon.preafb` to your first scene.
* Build and run it to a device (or just enter play mode)
* Inside your editor script, create a `Probe` instance and get the `ConnectedDevices` property

`ConnectedDevices` is a `List<Device>` with each `Device` being a simple class

```
public class Device
{
	public string Name;
	public string Model;
	public RuntimePlatform Platform;
	public string IPAddress;
}
```

A `Beacon` broadcasts a ping with a given frequency and a `Probe` listens for beacons and keeps an internal list of active ones.

As an example, here is a very simple Editor window that lists all connected devices

```
public class DeviceListEditorWindow : EditorWindow {

	Probe probe = new Probe();
	Device selectedDevice;
	DeviceList beaconList;
	
	void OnEnable() {
		beaconList = new DeviceList (probe);
		probe.Start();
	}

	void OnDisable() {
		probe.Stop ();
	}

	public void OnGUI ()
	{
		beaconList.Draw ();
		Repaint ();
	}
}
```

with `DeviceList` being a simple helper class to draw the actual popup list

```
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
```

## Compatibility

This tool was tested in OSX, Android and iOS but it should work on any platform that supports the `System.Net.Sockets` namespace.