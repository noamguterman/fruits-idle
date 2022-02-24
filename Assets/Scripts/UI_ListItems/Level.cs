using System;
using UnityEngine;

namespace UI.ListItems
{
	[Serializable]
	public class Level
	{
		public float RewardedMoney
		{
			get
			{
				return this.rewardedMoney;
			}
		}

		public float EarnedMoneyToUnclock
		{
			get
			{
				return this.earnedMoneyToUnclock;
			}
		}

		public int LevelNumber
		{
			get
			{
				return this.levelNumber;
			}
		}

		public Level(int levelNumber, float earnedMoneyToUnclock, float rewardedMoney)
		{
			this.levelNumber = levelNumber;
			this.earnedMoneyToUnclock = earnedMoneyToUnclock;
			this.rewardedMoney = rewardedMoney;
		}

		[SerializeField]
		private int levelNumber;

		[SerializeField]
		private float earnedMoneyToUnclock;

		[SerializeField]
		private float rewardedMoney;
	}
}
