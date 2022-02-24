using System;
using System.Collections.Generic;
using Settings.UI.Tabs;
using UnityEngine;
using Utilities;

namespace UI.ListItems.Booster
{
	[Serializable]
	public class BoosterListItemRewarded : BoosterListItem
    {
        private bool wasClicked;
        public override bool ShowButtonOptions
		{
			get
			{
				return false;
			}
		}

		public override bool ShowOptionsButton
		{
			get
			{
				return false;
			}
		}

		public override string PriceText
		{
			get
			{
				return string.Empty;
			}
		}

		public override bool IsPurchasable
		{
			get
			{
                //Debug.LogError("return !this.isActive && (Application.isEditor || (BoostersSettings.CurrentRewardedVideoAmount > 0 && Ad.IsRewardedLoaded));");
                return !isActive;// && (Application.isEditor || (BoostersSettings.CurrentRewardedVideoAmount > 0));
			}
		}

		public override void Initialize(BoostersSettings boostersSettings)
		{
			base.Initialize(boostersSettings);
            //Ad.RewardedLoaded += base.InvokeUIRefresh;
            AdsManager.RewardedClosed += this.OnRewardedClosed;
        }

		public override void OnButtonClick()
		{
			wasClicked = true;
			BoosterType boosterType = this.boosterType;
			if (boosterType != BoosterType.GameSpeed)
			{
				if (boosterType != BoosterType.MultiplierIncome)
				{
					throw new ArgumentOutOfRangeException();
				}
			}
			if (Application.isEditor)
			{
				OnRewardedClosed(true);
				return;
			}

            AdsManager.Instance.ShowRewardedVideo();
        }

		private void OnRewardedClosed(bool isReward)
		{
            CoroutineUtils.StartCoroutine(CoroutineUtils.ExecuteAfter(new Action(base.InvokeUIRefresh), new WaitForEndOfFrame()));

            if (!wasClicked || !isReward)
			{
				return;
			}
			wasClicked = false;

            BoostersSettings.CurrentRewardedVideoAmount--;
			InvokeChangeBoosterTypeMutiplier(true);
			InvokeUIRefresh();
			Debug.Log(duration);
			InvokeUIStartTimer(duration).Elapsed += delegate()
			{
				InvokeChangeBoosterTypeMutiplier(false);
				InvokeUIRefresh();
			};
		}

	}
}
