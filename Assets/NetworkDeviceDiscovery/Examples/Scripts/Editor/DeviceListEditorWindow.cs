using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;

namespace NetworkDeviceDiscovery {
	
	public class DeviceListEditorWindow : EditorWindow {

		Probe probe = new Probe();
		Device selectedDevice;
		DeviceList beaconList;

		[MenuItem ("Window/Device Discovery Example")]
		static void Init ()
		{
			DeviceListEditorWindow window = (DeviceListEditorWindow)GetWindow (typeof(DeviceListEditorWindow));
			window.Show ();
		}

		void OnEnable() {
			beaconList = new DeviceList (probe);
			probe.Start();
		}

		void OnDisable() {
			probe.Stop ();
		}

		public void OnGUI ()
		{
			EditorGUILayout.LabelField ("Devices in the same network", GUI.skin.FindStyle ("HeaderLabel"));
			EditorGUILayout.Space ();

			beaconList.Draw ();

			Repaint ();
		}
	}
}