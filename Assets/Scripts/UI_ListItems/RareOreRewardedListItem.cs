using System;
using Sirenix.OdinInspector;
using UI.ListItems.Buttons;
using UnityEngine;

namespace UI.ListItems
{
	[Serializable]
	public class RareOreRewardedListItem : ListItem
    {
        //[FoldoutGroup("$Name", 0)]
        [SerializeField]
        private int requiredRewardedAmount;

        //[FoldoutGroup("$Name", 0)]
        [SerializeField]
        private int gemAmount;

        //[FoldoutGroup("$Name", 0)]
        [SerializeField]
        private float waitTime;

        private bool startsRewardedVideo;

        private int CurrentRewardedAmount
		{
			get
			{
				return PlayerPrefs.GetInt("CurrentRewardedAmount");
			}
			set
			{
				PlayerPrefs.SetInt("CurrentRewardedAmount", value);
			}
		}

		private bool IsActive
		{
			get
			{
				return bool.Parse(PlayerPrefs.GetString("IsActive", "false"));
			}
			set
			{
				PlayerPrefs.SetString("IsActive", value.ToString());
			}
		}

		private DateTime TimerEndDate
		{
			get
			{
				return DateTime.Parse(PlayerPrefs.GetString("TimerEndDate"));
			}
			set
			{
				PlayerPrefs.SetString("TimerEndDate", value.ToString());
			}
		}

		private bool IsReady
		{
			get
			{
				return CurrentRewardedAmount >= requiredRewardedAmount;
			}
		}

		public override string LevelText
		{
			get
			{
				return string.Empty;
			}
		}

		public override string ValueText
		{
			get
			{
				return string.Format("{0} gems", gemAmount);
			}
		}

		public override bool IsAvailable
		{
			get
			{
				return true;
			}
		}

		public override bool IsLocked
		{
			get
			{
				return false;
			}
		}

		public override int CurrentLockLevel
		{
			get
			{
				return -1;
			}
		}

		public override bool UseTimer
		{
			get
			{
				return true;
			}
		}

		public override bool IsPurchasable
		{
			get
			{
				return !IsActive && (Application.isEditor);
			}
		}

		public override Sprite PriceImage
		{
			get
			{
				if (!IsReady)
				{
					return priceImage;
				}
				return null;
			}
		}

		public override string PriceText
		{
			get
			{
				if (!IsReady)
				{
					return string.Format("{0}/{1}", CurrentRewardedAmount, requiredRewardedAmount);
				}
				return "Get";
			}
		}

		public override string GetPrefsKey()
		{
			return string.Empty;
		}

		public void Initialize()
		{
			startsRewardedVideo = false;
			if (IsActive)
			{
				SetTimer((float)(TimerEndDate - DateTime.UtcNow).TotalSeconds, false);
			}

            {
				if (!startsRewardedVideo)
				{
					return;
				}
				//if (isReward)
				//{
				//	int currentRewardedAmount = this.CurrentRewardedAmount;
				//	this.CurrentRewardedAmount = currentRewardedAmount + 1;
				//}
				startsRewardedVideo = false;
				InvokeUIRefresh();
			};
		}

		private void SetTimer(float time, bool giveReward)
		{
			if (giveReward)
			{
				GameData.IncreaseMoney((float)gemAmount, MoneyType.Gem, Button.UpgradeImage.transform.position, Button.UpgradeImage.rectTransform.rect.size.y / Button.UpgradeImage.sprite.rect.size.y, false);
			}
			IsActive = true;
			TimerEndDate = DateTime.UtcNow.AddSeconds((double)time);
			InvokeUIStartTimer(waitTime, time).Elapsed += delegate()
			{
				IsActive = false;
				InvokeUIRefresh();
			};
		}

		public override void OnButtonClick()
		{
			if (IsReady)
			{
				CurrentRewardedAmount = 0;
				SetTimer(waitTime, true);
			}
			else if (Application.isEditor)
			{
				int currentRewardedAmount = CurrentRewardedAmount;
				CurrentRewardedAmount = currentRewardedAmount + 1;
			}
			else
			{
				startsRewardedVideo = true;
			}
			InvokeUIRefresh();
		}

		public override void SetButtonSettings(ButtonsSettings buttonsSettings)
		{
		}

	}
}
