using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UI.ListItems.Upgrades.Piece;
using UnityEngine;

namespace UI.ListItems
{
	[CreateAssetMenu(menuName = "Settings/Levels Settings", fileName = "Levels Settings")]
	public class LevelsSettings : ScriptableObject
	{
		//[FoldoutGroup("Settings", 0)]
		[Button("Generate Settings")]
		private void GenerateSettings()
		{
			this.levels.Clear();
			for (int i = 1; i <= this.maxLevel; i++)
			{
				float earnedMoneyToUnclock = (this.moneyToUnlockDeltaType == DeltaType.Additive) ? (this.defaultMoneyToUnlock * (1f + this.deltaMoneyToUnlock * (float)(i - 1))) : (this.defaultMoneyToUnlock * Mathf.Pow(this.deltaMoneyToUnlock, (float)(i - 1)));
				float rewardedMoney = (this.rewardedMoneyDeltaType == DeltaType.Additive) ? (this.defaultRewardedMoney * (1f + this.deltaRewardedMoney * (float)(i - 1))) : (this.defaultRewardedMoney * Mathf.Pow(this.deltaRewardedMoney, (float)(i - 1)));
				this.levels.Add(new Level(i, earnedMoneyToUnclock, rewardedMoney));
			}
		}

		private void OnEnable()
		{
		}

		public Level GetCurrentLevel()
		{
			int level = GameData.CurrentLevel;
			if (this.CurrentLevel != null && level == this.CurrentLevel.LevelNumber)
			{
				return this.CurrentLevel;
			}
			List<Level> list = this.levels.ToList<Level>();
			level = Mathf.Clamp(level, list.First<Level>().LevelNumber, list.Last<Level>().LevelNumber);
			list.Reverse();
			Level level3 = list.First((Level u) => u.LevelNumber <= level);
			list.Reverse();
			Level level2 = list.First((Level u) => u.LevelNumber >= level);
			if (level3.LevelNumber == level2.LevelNumber)
			{
				return this.CurrentLevel = level3;
			}
			float t = (float)(level - level3.LevelNumber) / (float)(level2.LevelNumber - level3.LevelNumber);
			return this.CurrentLevel = new Level(level, Mathf.Lerp(level3.EarnedMoneyToUnclock, level2.EarnedMoneyToUnclock, t), Mathf.Lerp(level3.RewardedMoney, level2.RewardedMoney, t));
		}

		//[FoldoutGroup("Settings", 0)]
		[SerializeField]
		private int maxLevel;

		//[FoldoutGroup("Settings/Money To Unlock", 0)]
		[SerializeField]
		[LabelText("Default")]
		private float defaultMoneyToUnlock;

		//[FoldoutGroup("Settings/Money To Unlock", 0)]
		[SerializeField]
		[LabelText("Delta")]
		private float deltaMoneyToUnlock;

		//[FoldoutGroup("Settings/Money To Unlock", 0)]
		[SerializeField]
		[LabelText("Delta Type")]
		private DeltaType moneyToUnlockDeltaType;

		//[FoldoutGroup("Settings/Rewarded Money", 0)]
		[SerializeField]
		[LabelText("Default")]
		private float defaultRewardedMoney;

		//[FoldoutGroup("Settings/Rewarded Money", 0)]
		[SerializeField]
		[LabelText("Delta")]
		private float deltaRewardedMoney;

		//[FoldoutGroup("Settings/Rewarded Money", 0)]
		[SerializeField]
		[LabelText("Delta Type")]
		private DeltaType rewardedMoneyDeltaType;

		private Level CurrentLevel;

		[SerializeField]
		private List<Level> levels;
	}
}
