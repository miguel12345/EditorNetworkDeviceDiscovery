using UnityEngine;
using System.Collections;
using System;

namespace NetworkDeviceDiscovery {

	public class TimeUtils : MonoBehaviour
	{

		private static readonly DateTime UnixEpoch =
			new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static int GetCurrentUnixTimestamp()
		{
			return (int)(DateTime.UtcNow - UnixEpoch).TotalSeconds;
		}

	}

}