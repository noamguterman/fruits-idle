using System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace UI.ListItems
{
	[Serializable]
	public class Bonus
    {
        //[FoldoutGroup("$Name", 0)]
        [SerializeField]
        [Range(0f, 1f)]
        private float appearProbability;

        //[FoldoutGroup("$Name", 0)]
        [SerializeField]
        private float bonusDuration;

        //[FoldoutGroup("$Name", 0)]
        [SerializeField]
        private float bonusRewardedDuration;

        //[FoldoutGroup("$Name", 0)]
        [SerializeField]
        private Color bonusButtonColor;

        //[FoldoutGroup("$Name", 0)]
        [SerializeField]
        [TextArea]
        private string bonusText;

        //[FoldoutGroup("$Name", 0)]
        [SerializeField]
        [HideInInspector]
        private BonusType bonusType;

        //[FoldoutGroup("$Name", 0)]
        [SerializeField]
        [ShowIf("SpeedBoost", true)]
        private float pressPowerMultiplier = 1f;

        //[FoldoutGroup("$Name", 0)]
        [SerializeField]
        [ShowIf("SpeedBoost", true)]
        private float spawnAmountMultiplier = 1f;

        //[FoldoutGroup("$Name", 0)]
        [SerializeField]
        [ShowIf("Money", true)]
        private float moneyMultiplier = 1f;

        [UsedImplicitly]
		private string Name
		{
			get
			{
				return BonusType.ToString().AddSpaces();
			}
		}

		public float AppearProbability
		{
			get
			{
				return appearProbability;
			}
		}

		public float BonusDuration
		{
			get
			{
				return bonusDuration;
			}
		}

		public BonusType BonusType
		{
			get
			{
				return bonusType;
			}
		}

		public Color BonusButtonColor
		{
			get
			{
				return bonusButtonColor;
			}
		}

		public float BonusRewardedDuration
		{
			get
			{
				return bonusRewardedDuration;
			}
		}

		public string BonusText
		{
			get
			{
				return bonusText;
			}
		}

		[UsedImplicitly]
		private bool SpeedBoost
		{
			get
			{
				return bonusType == BonusType.SpeedBoost;
			}
		}

		[UsedImplicitly]
		private bool Money
		{
			get
			{
				return bonusType == BonusType.Money;
			}
		}

		public float PressPowerMultiplier
		{
			get
			{
				return pressPowerMultiplier;
			}
		}

		public float SpawnAmountMultiplier
		{
			get
			{
				return spawnAmountMultiplier;
			}
		}

		public float MoneyMultiplier
		{
			get
			{
				return moneyMultiplier;
			}
		}

	}
}
