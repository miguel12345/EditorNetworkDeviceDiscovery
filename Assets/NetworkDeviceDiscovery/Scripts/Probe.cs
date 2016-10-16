using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NetworkDeviceDiscovery {
	
	public class Probe {
		
		public int CheckFrequency = 2; //times per second
		public int BeaconMaxAge = 3; //how long should we wait for a beacon signal before considering dead
		public Logger.LogLevel Loglevel = Logger.LogLevel.Info; 

		List<BeaconDevice> BackgroundThreadDetectedBeacons = new List<BeaconDevice> ();
		List<BeaconDevice> MainThreadDetectedBeacons = new List<BeaconDevice> ();
		bool needListSync;
		private System.Object thisLock = new System.Object();

		public List<Device> ConnectedDevices {
			get {
				var connectedBeacons = ConnectedBeacons;
				return connectedBeacons.Select (beacon => beacon.Device).ToList();
			}
		}

		List<BeaconDevice> ConnectedBeacons {
			get {
				lock (thisLock) {
					if (needListSync) {
						MainThreadDetectedBeacons = new List<BeaconDevice> (BackgroundThreadDetectedBeacons);
						needListSync = false;
					}
				}

				return MainThreadDetectedBeacons;
			}
		}

		bool kill;

		// Use this for initialization
		public void Start () {
		
			Thread t = new Thread( this.Loop );
			t.Start();
		}

		public void Stop() {
			kill = true;
		}

		void Loop() {

			Logger.LogInfo ("Starting probe",Loglevel);

			UdpClient ProbeClient = null;

			try {

				ProbeClient = new UdpClient(8887);
				IPEndPoint RemoteBeaconEndpoint = new IPEndPoint(IPAddress.Any, 0);

				while (!kill) {

					var asyncResult = ProbeClient.BeginReceive (null, null);
	
					while (!asyncResult.IsCompleted && !kill) {

						Thread.Sleep (500);
						RemoveDeadBeacons ();
					}

					if(kill) break;

					byte[] receivedData = ProbeClient.EndReceive(asyncResult,ref RemoteBeaconEndpoint);

					var ServerResponse = Encoding.UTF8.GetString (receivedData);
					var beaconDevice = JsonUtility.FromJson<BeaconDevice> (ServerResponse);
					if (beaconDevice.IsValid) {
						HandleDeviceDectected (beaconDevice);
					} else {
						Logger.LogWarning ("Not a device -> Received " + ServerResponse + " from " + RemoteBeaconEndpoint.Address.ToString (), Loglevel);
					}
				}
			}
			catch(ThreadAbortException) {
				//Unity kills the thread when it needs to, no worries here
			}
			catch(Exception e) {
				Logger.LogError (e.GetType().ToString()+" "+e.Message,Loglevel);
			}
			finally {

				Logger.LogInfo ("Stopping probe",Loglevel);

				if(ProbeClient != null)
					ProbeClient.Close();
			}
		}

		void HandleDeviceDectected(BeaconDevice device) {
			lock (thisLock) {
				var existingDevice = BackgroundThreadDetectedBeacons.Find (d => d.Equals (device));

				if (existingDevice == null) {
					device.LastDetectionTime = TimeUtils.GetCurrentUnixTimestamp ();
					Logger.LogInfo ("New device detect " + device, Loglevel);
					BackgroundThreadDetectedBeacons.Add (device);
					needListSync = true;
				} else {
					existingDevice.LastDetectionTime = TimeUtils.GetCurrentUnixTimestamp ();
				}
			}
		}

		void RemoveDeadBeacons() {
			var currentTime = TimeUtils.GetCurrentUnixTimestamp ();
			lock (thisLock) {
				int removed = BackgroundThreadDetectedBeacons.RemoveAll (beacon => (currentTime - beacon.LastDetectionTime) > BeaconMaxAge);
				if (removed > 0) {

					Logger.LogInfo (removed+" dead beacons removed",Loglevel);

					needListSync = true;
				}
			}
		}
	}
}