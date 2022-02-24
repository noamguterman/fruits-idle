using System;
using System.Globalization;
using UnityEngine;

namespace Utilities
{
	public class OfflineTimeTracker : MonoBehaviour
	{
		private DateTime ApplicationCloseTime
		{
			get
			{
				if (!PlayerPrefs.HasKey("ApplicationCloseTime"))
				{
					MonoBehaviour.print("ApplicationCloseTime hasn't key");
					return DateTime.UtcNow;
				}
				DateTime dateTime = DateTime.Parse(PlayerPrefs.GetString("ApplicationCloseTime"));
				MonoBehaviour.print(string.Format("close time (sec) = {0}", dateTime));
				if (!(dateTime > DateTime.UtcNow))
				{
					return dateTime;
				}
				return DateTime.UtcNow;
			}
			set
			{
				PlayerPrefs.SetString("ApplicationCloseTime", value.ToString(CultureInfo.InvariantCulture));
				PlayerPrefs.Save();
			}
		}

		public float ApplicationOfflineTimeHours { get; private set; }

		private void Awake()
		{
			this.ApplicationOfflineTimeHours = Convert.ToSingle(DateTime.UtcNow.Subtract(this.ApplicationCloseTime).TotalSeconds) / 3600f;
		}

		private void OnApplicationPause(bool pauseStatus)
		{
			if (pauseStatus)
			{
				MonoBehaviour.print("App Close");
				this.ApplicationCloseTime = DateTime.UtcNow;
			}
		}
	}
}
