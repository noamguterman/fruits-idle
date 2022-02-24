using System;
using System.Collections.Generic;
using Game;
using UI.ListItems;
using UI.ListItems.Booster;
using UnityEngine;

namespace Settings.UI.Tabs
{
	[CreateAssetMenu(menuName = "Settings/Boosters Settings", fileName = "Boosters Settings")]
	public class BoostersSettings : Tab
    {
        [SerializeField]
        private int rewardedVideoPerDayAmount;

        [SerializeField]
        private List<BoosterListItemRewarded> boostersRewarded;

        [SerializeField]
        private List<BoosterListItemMoney> boostersMoney;

        private List<BoosterListItem> allBoosters;

        public override IReadOnlyList<ListItem> GetTabItems()
		{
			return allBoosters;
		}

		public event Action<BoosterType, float, float, bool> ChangeBoosterTypeMutiplier;

		private void OnEnable()
		{
			if (Preloader.IsFirstApplicationLaunch || (DateTime.Now - LastTimeRewardedVideoAmountReseted).Days >= 1)
			{
				ResetRewardedVideoAmount();
			}
			allBoosters = new List<BoosterListItem>(boostersRewarded);
			allBoosters.AddRange(boostersMoney);
			foreach (BoosterListItem boosterListItem in allBoosters)
			{
				boosterListItem.Initialize(this);
				boosterListItem.ChangeBoosterTypeMutiplier += delegate(BoosterType type, float m, float time, bool isActive)
				{
					Action<BoosterType, float, float, bool> changeBoosterTypeMutiplier = ChangeBoosterTypeMutiplier;
					if (changeBoosterTypeMutiplier == null)
					{
						return;
					}
					changeBoosterTypeMutiplier(type, m, time, isActive);
				};
			}
		}

		public static int CurrentRewardedVideoAmount
		{
			get
			{
				return PlayerPrefs.GetInt("CurrentRewardedVideoAmount");
			}
			set
			{
				PlayerPrefs.SetInt("CurrentRewardedVideoAmount", value);
			}
		}

		private DateTime LastTimeRewardedVideoAmountReseted
		{
			get
			{
				return new DateTime(long.Parse(PlayerPrefs.GetString("LastTimeRewardedVideoAmountReseted", DateTime.Now.Ticks.ToString())));
			}
			set
			{
				PlayerPrefs.SetString("LastTimeRewardedVideoAmountReseted", value.Ticks.ToString());
			}
		}

		private void ResetRewardedVideoAmount()
		{
			CurrentRewardedVideoAmount = rewardedVideoPerDayAmount;
			LastTimeRewardedVideoAmountReseted = DateTime.Now;
		}

	}
}
