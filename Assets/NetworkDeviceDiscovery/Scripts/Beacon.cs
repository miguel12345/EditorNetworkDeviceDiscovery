using UnityEngine;
using System.Collections;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using System.Net;
using System;

namespace NetworkDeviceDiscovery {
	
	public class Beacon : MonoBehaviour {

		public int Port = 8888;
		public Logger.LogLevel Loglevel = Logger.LogLevel.Warning; 
		public int BroadcastSignalFrequency = 1; 

		bool kill = false;
		BeaconDevice currentBeaconDevice;
		byte[] currentBeaconDeviceJSONData;

		static Beacon currentBeacon;

		// Use this for initialization
		void Awake () {

			if (currentBeacon != null) {
				GameObject.Destroy (this.gameObject);
				return;
			} else {
				currentBeacon = this;
				DontDestroyOnLoad (gameObject);
			}

			currentBeaconDevice = new BeaconDevice (Device.CurrentDevice, Port);
			currentBeaconDeviceJSONData = Encoding.UTF8.GetBytes(JsonUtility.ToJson (currentBeaconDevice));

		}

		void OnEnable() {
			Thread t = new Thread( this.Loop );
			t.Start();
		}

		void OnDisable() {
			kill = true;
		}

		void Loop() {

			Logger.LogInfo ("Starting beacon",Loglevel);

			var beaconUDP = new UdpClient(Port);
			beaconUDP.EnableBroadcast = true; 

			try {

				while (!kill)
				{
					Logger.LogDebug ("Sending beacon signal",Loglevel);

					var BroadcastEntpoint = new IPEndPoint(IPAddress.Broadcast, 8887);
					try {
						beaconUDP.Send(currentBeaconDeviceJSONData, currentBeaconDeviceJSONData.Length, BroadcastEntpoint);
					}
					catch(SocketException) {
						//perhaps the internet is unreachable, let's wait and try again
					}

					Thread.Sleep ( (int)((1.0f / (float)BroadcastSignalFrequency)*1000.0f));
				}

				Logger.LogInfo ("Stopping beacon",Loglevel);
			}
			catch(ThreadAbortException) {
				//Unity kills the thread when it needs to, no worries here
			}
			catch(Exception e) {
				Logger.LogError (e.GetType().ToString()+" "+e.Message,Loglevel);
			}
			finally {
				beaconUDP.Close();
			}
		}
	}
}