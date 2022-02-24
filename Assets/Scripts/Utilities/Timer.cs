using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities
{
	public class Timer : MonoBehaviour
	{
		public float CurrentTime { get; set; }

		public event Action Elapsed;

		public bool IsActivated { get; private set; }

		private static ulong Now
		{
			get
			{
				return (ulong)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
			}
		}

		private void Awake()
		{
			this.timerImage = base.GetComponentsInChildren<Image>(true).FirstOrDefault((Image img) => img.transform.parent == base.transform);
			this.timerText = base.GetComponentsInChildren<Text>(true).FirstOrDefault((Text img) => img.transform == base.transform || img.transform.parent == base.transform);
			Text tmp_Text = this.timerText;
			if (tmp_Text != null)
			{
				tmp_Text.gameObject.SetActive(false);
			}
			this.timerData = Timer.GetTimerData(this.id);
		}

		public static void StartTimer(ulong milliseconds, string timerID)
		{
			Timer.TimerData timerData = Timer.GetTimerData(timerID);
			timerData.SetTimerData(milliseconds, true);
			PlayerPrefs.SetString("Timer" + timerID, JsonUtility.ToJson(timerData));
		}

		public void StopTimer()
		{
			this.IsActivated = false;
			this.Elapsed = null;
		}

		public void StartTimer(ulong milliseconds, Timer.OutputType t = Timer.OutputType.Seconds)
		{
			this.SetTimer(milliseconds, t);
		}

		public void StartTimer(float seconds, Timer.OutputType t = Timer.OutputType.Seconds)
		{
			this.SetTimer(seconds, t);
		}

		public void StartTimer(float seconds, string additionalOutputString, Timer.OutputType t = Timer.OutputType.Seconds)
		{
			this.additionalOutputString = additionalOutputString;
			this.SetTimer(seconds, t);
		}

		public void StartTimer(ulong milliseconds, Image tImage, Timer.OutputType t = Timer.OutputType.Seconds)
		{
			this.timerImage = tImage;
			this.SetTimer(milliseconds, t);
		}

		public void SetMultiplier(float multiplier)
		{
			if (multiplier < 0f)
			{
				return;
			}
			this.currentMultiplier = multiplier;
		}

		public void IncreaseMultiplier(float value)
		{
			if (this.currentMultiplier + value < 0f)
			{
				return;
			}
			this.currentMultiplier += value;
		}

		private void SetTimer(float seconds, Timer.OutputType t)
		{
			this.Elapsed = null;
			this.currentMultiplier = 0.83f;
			this.outputType = t;
			this.CurrentTime = seconds;
			this.IsActivated = true;
			this.timerData.SetTimerData((ulong)this.CurrentTime, true);
			PlayerPrefs.SetString("Timer" + this.id, JsonUtility.ToJson(this.timerData));
			Text tmp_Text = this.timerText;
			if (tmp_Text == null)
			{
				return;
			}
			tmp_Text.gameObject.SetActive(true);
		}

		public void StartTimer(float seconds, Text textMeshPro, Timer.OutputType t = Timer.OutputType.Seconds)
		{
			this.timerText = textMeshPro;
			this.timerTextAdddedFromFunc = true;
			this.SetTimer(seconds, t);
		}

		private void Update()
		{
			if (!this.IsActivated)
			{
				return;
			}
			if (this.CurrentTime <= Time.deltaTime * this.currentMultiplier)
			{
				this.CurrentTime = 0f;
				this.IsActivated = false;
				PlayerPrefs.DeleteKey("Timer" + this.id);
			}
			else
			{
				this.CurrentTime -= Time.deltaTime * this.currentMultiplier;
			}
			if (this.timerText)
			{
				if (!this.timerText.gameObject.activeSelf)
				{
					this.timerText.gameObject.SetActive(true);
				}
				this.timerText.text = new Timer.TimeContainer(this.CurrentTime, this.outputType).ToString() + this.additionalOutputString;
			}
			if (this.timerImage)
			{
				float num = this.CurrentTime / (this.timerData.ElapseDate - this.timerData.StartDate);
				if (this.reversedFill)
				{
					num = 1f - num;
				}
				this.timerImage.fillAmount = num;
			}
			if (!this.IsActivated)
			{
				if (this.timerImage)
				{
					this.timerImage.fillAmount = 0f;
				}
				if (this.timerTextAdddedFromFunc)
				{
					this.timerText = null;
					this.timerTextAdddedFromFunc = false;
				}
				else
				{
					Text tmp_Text = this.timerText;
					if (tmp_Text != null)
					{
						tmp_Text.gameObject.SetActive(false);
					}
				}
				Action elapsed = this.Elapsed;
				if (elapsed == null)
				{
					return;
				}
				elapsed();
			}
		}

		private static Timer.TimerData GetTimerData(string timerID)
		{
			string @string = PlayerPrefs.GetString("Timer" + timerID, string.Empty);
			if (!(@string == string.Empty))
			{
				return JsonUtility.FromJson<Timer.TimerData>(@string);
			}
			return new Timer.TimerData(timerID);
		}

		[SerializeField]
		private string id;

		[SerializeField]
		public bool reversedFill;

		private string additionalOutputString;

		private Text timerText;

		private Image timerImage;

		private Timer.OutputType outputType;

		private float currentMultiplier;

		private Timer.TimerData timerData;

		private bool timerTextAdddedFromFunc;

		[Serializable]
		public class TimerData
		{
			public TimerData(string id)
			{
				this.Id = id;
			}

			public void SetTimerData(ulong milliseconds, bool start = true)
			{
				this.StartDate = Timer.Now;
				this.ElapseDate = this.StartDate + milliseconds;
				this.IsStarted = start;
			}

			public readonly string Id;

			public ulong StartDate;

			public ulong ElapseDate;

			public bool IsStarted;
		}

		[Serializable]
		public struct TimeContainer
		{
			public TimeContainer(float seconds, Timer.OutputType t)
			{
				this.outputType = t;
				this.allSeconds = Mathf.CeilToInt(seconds);
				this.Seconds = this.allSeconds % 60;
				float num = (float)this.allSeconds / 60f;
				this.Minutes = (int)num % 60;
				float num2 = num / 60f;
				this.Hours = (int)num2;
			}

			public string ToString()
			{
				switch (this.outputType)
				{
				case Timer.OutputType.All:
				{
					string text = (this.Seconds < 10) ? ("0" + this.Seconds) : this.Seconds.ToString();
					text = ((this.Minutes < 10) ? string.Concat(new object[]
					{
						"0",
						this.Minutes,
						":",
						text
					}) : (this.Minutes + ":" + text));
					if (this.Hours <= 0)
					{
						return text;
					}
					if (this.Hours >= 10)
					{
						return this.Hours + ":" + text;
					}
					return string.Concat(new object[]
					{
						"0",
						this.Hours,
						":",
						text
					});
				}
				case Timer.OutputType.Seconds:
					return this.ToSeconds().ToString();
				case Timer.OutputType.Adaptive:
					if (this.Hours > 0)
					{
						return string.Format("{0}h", this.Hours);
					}
					if (this.Minutes > 0)
					{
						return string.Format("{0}m", this.Minutes);
					}
					return string.Format("{0}s", this.Seconds);
				default:
					throw new ArgumentOutOfRangeException();
				}
			}

			private int ToSeconds()
			{
				return this.allSeconds;
			}

			public ulong ToMilliseconds()
			{
				return (ulong)((long)this.ToSeconds() * 1000L);
			}

			public int Seconds;

			public int Minutes;

			public int Hours;

			private int allSeconds;

			private readonly Timer.OutputType outputType;
		}

		public enum OutputType
		{
			All,
			Seconds,
			Adaptive
		}
	}
}
