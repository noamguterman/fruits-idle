using System;
using System.Collections.Generic;
using System.Linq;
using Game.Press;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UI.ListItems.Upgrades.Piece;
using UI.ListItems.Upgrades.Piece.Press;
using UnityEngine;

namespace Settings.UI.Tabs
{
	[Serializable]
	public class PressVariation
    {
        //[FoldoutGroup("$Name", 0)]
        [SerializeField]
        [Required]
        [AssetsOnly]
        private PressController press;

        //[FoldoutGroup("$Name", 0)]
        [SerializeField]
        private List<PressTypeListItem> pressUpgrades;

        //[FoldoutGroup("$Name/Settings", 0)]
        [SerializeField]
        [Range(0f, 1f)]
        private float pressPushYOffsetPercent;

        //[FoldoutGroup("$Name/Settings", 0)]
        [SerializeField]
        private float pressCrushSlowDownSpeedMultiplier;

        //[FoldoutGroup("$Name/Settings", 0)]
        [SerializeField]
        private float minUpperPressDistFromLowerPart;

        //[FoldoutGroup("$Name/Settings", 0)]
        [SerializeField]
        private float minPressMoveTime;

        //[FoldoutGroup("$Name/Settings/Animation Curves", 0)]
        [SerializeField]
        private AnimationCurve pressMoveDownCurve;

        //[FoldoutGroup("$Name/Settings/Animation Curves", 0)]
        [SerializeField]
        private AnimationCurve pressMoveUpCurve;

        public PressController Press
		{
			get
			{
				return press;
			}
		}

		public List<PressTypeListItem> PressUpgrades
		{
			get
			{
				return pressUpgrades;
			}
		}

		public float PressPushYOffsetPercent
		{
			get
			{
				return pressPushYOffsetPercent;
			}
		}

		public float PressCrushSlowDownSpeedMultiplier
		{
			get
			{
				return pressCrushSlowDownSpeedMultiplier;
			}
		}

		public float MinUpperPressDistFromLowerPart
		{
			get
			{
				return minUpperPressDistFromLowerPart;
			}
		}

		public float MinPressMoveTime
		{
			get
			{
				return minPressMoveTime;
			}
		}

		public AnimationCurve PressMoveDownCurve
		{
			get
			{
				return pressMoveDownCurve;
			}
		}

		public AnimationCurve PressMoveUpCurve
		{
			get
			{
				return pressMoveUpCurve;
			}
		}

		public Upgrade GetCurrentUpgrade(PressUpgradeType upgradeType, bool useDebug = false)
		{
			PressTypeListItem pressTypeListItem = pressUpgrades.FirstOrDefault((PressTypeListItem upgradeListItem) => upgradeListItem.UpgradeType == upgradeType);
			if (pressTypeListItem == null)
			{
				return null;
			}
			return pressTypeListItem.CurrentLevelUpgrade;
		}

		[UsedImplicitly]
		private string Name
		{
			get
			{
				PressController pressController = press;
				if (pressController == null)
				{
					return null;
				}
				return pressController.name;
			}
		}

	}
}
