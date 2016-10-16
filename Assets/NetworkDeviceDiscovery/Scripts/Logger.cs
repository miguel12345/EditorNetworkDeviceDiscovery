using UnityEngine;
using System.Collections;

namespace NetworkDeviceDiscovery {

	public class Logger
	{
		public enum LogLevel {
			Debug,
			Info,
			Warning,
			Error
		}

		public static void LogDebug(string message, LogLevel level) {
			if (LogLevel.Debug.CompareTo(level) >= 0) {
				Debug.Log(message);
			}
		}

		public static void LogInfo(string message, LogLevel level) {
			if (LogLevel.Info.CompareTo(level) >= 0) {
				Debug.Log(message);
			}
		}

		public static void LogWarning(string message, LogLevel level) {
			if (LogLevel.Warning.CompareTo(level) >= 0) {
				Debug.LogWarning(message);
			}
		}

		public static void LogError(string message, LogLevel level) {
			if (LogLevel.Error.CompareTo(level) >= 0) {
				Debug.LogError(message);
			}
		}

	}

}