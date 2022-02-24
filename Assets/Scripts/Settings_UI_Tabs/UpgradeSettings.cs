using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UI.ListItems;
using UI.ListItems.Upgrades.Piece;
using UnityEngine;
using Utilities;

namespace Settings.UI.Tabs
{
	[CreateAssetMenu(menuName = "Settings/Upgrade Settings")]
	public class UpgradeSettings : Tab
    {
        //[FoldoutGroup("Upgrades", 0)]
        [SerializeField]
        [ListDrawerSettings(IsReadOnly = true, HideAddButton = true)]
        [GenerateListFromEnum(EnumType = typeof(UpgradeType))]
        private List<UpgradeTypeListItem> allUpgrades;

        //[FoldoutGroup("Upgrades/Pieces/Settings", 0)]
        [SerializeField]
        private float defaultValue;

        //[FoldoutGroup("Upgrades/Pieces/Settings", 0)]
        [SerializeField]
        private int levelNextDefaultValue;

        //[FoldoutGroup("Upgrades/Pieces/Settings", 0)]
        [SerializeField]
        private float multiplierNextDefaultValue;

        //[FoldoutGroup("Upgrades/Pieces/Settings", 0)]
        [SerializeField]
        private float deltaValue;

        //[FoldoutGroup("Upgrades/Pieces/Settings", 0)]
        [SerializeField]
        private float defaultPrice;

        //[FoldoutGroup("Upgrades/Pieces/Settings", 0)]
        [SerializeField]
        private float deltaPrice;

        //[FoldoutGroup("Upgrades/Pieces/Settings", 0)]
        [SerializeField]
        private int maxLevel;

        //[FoldoutGroup("Upgrades/Pieces/Settings", 0)]
        [SerializeField]
        private DeltaType deltaType;

        //[FoldoutGroup("Upgrades/Pieces", false, 0)]
        [SerializeField]
        [ListDrawerSettings(Expanded = false)]
        [OnValueChanged("OnPiecesUpgradesChanged", false)]
        private List<PieceListItem> piecesUpgrades;

        //[FoldoutGroup("Upgrades/Pieces", false, 0)]
        [SerializeField]
        [ListDrawerSettings(Expanded = false)]
        private List<RarePieceListItem> rarePieceUpgrades;

        [SerializeField]
        private float maxOfflineDirationHours;

        [SerializeField]
        private float minOfflineEarnings;

        private int piecesUpgradesCount;

        //[FoldoutGroup("Upgrades/Pieces/Settings", 0)]
		[Button("Generate Settings")]
		private void GenerateSettings()
		{
			for (int i = 0; i < piecesUpgrades.Count; i++)
			{
				if (multiplierNextDefaultValue == 0f)
				{
                    Upgrade upgrade;
                    switch (i)
                    {
                        case 0:
                            upgrade = new Upgrade(0, defaultPrice, defaultValue);
                            break;
                        case 1:
                            upgrade = piecesUpgrades[i - 1].GetUpgrade(levelNextDefaultValue);
                            break;
                        default:
                            upgrade = piecesUpgrades[i - 1].GetUpgrade(levelNextDefaultValue-1);
                            break;
                    }
					//Upgrade upgrade = (i == 0) ? new Upgrade(0, defaultPrice, defaultValue) : piecesUpgrades[i - 1].GetUpgrade(levelNextDefaultValue);
					piecesUpgrades[i].SetSettings(i == 0, upgrade.Value, deltaValue, upgrade.PriceOfLevelUp, deltaPrice, maxLevel, deltaType);
				}
				else
				{
					Upgrade upgrade2 = (i == 0) ? new Upgrade(0, defaultPrice, defaultValue) : piecesUpgrades[i - 1].GetUpgrade((i == 1) ? 1 : 0);
					float num = upgrade2.Value;
					float num2 = upgrade2.PriceOfLevelUp;
					if (i > 0)
					{
						num *= multiplierNextDefaultValue;
						num2 *= multiplierNextDefaultValue;
					}
					piecesUpgrades[i].SetSettings(i == 0, num, deltaValue, num2, deltaPrice, maxLevel, deltaType);
				}
			}
		}

		public float MaxOfflineDirationHours
		{
			get
			{
				return maxOfflineDirationHours;
			}
		}

		public float MinOfflineEarnings
		{
			get
			{
				return minOfflineEarnings;
			}
		}

		public IReadOnlyList<UpgradeTypeListItem> AllUpgrades
		{
			get
			{
				return allUpgrades;
			}
		}

		public IReadOnlyList<PieceListItem> PiecesUpgrades
		{
			get
			{
				return piecesUpgrades;
			}
		}

		public IReadOnlyList<RarePieceListItem> RarePieceUpgrades
		{
			get
			{
				return rarePieceUpgrades;
			}
		}

		public UpgradeListItem GetUpgradeListItem(UpgradeType upgradeType)
		{
			return AllUpgrades.First((UpgradeTypeListItem u) => u.UpgradeType == upgradeType);
		}

		public Upgrade GetCurrentUpgrade(UpgradeType upgradeType)
		{
			return AllUpgrades.First((UpgradeTypeListItem upgradeListItem) => upgradeListItem.UpgradeType == upgradeType).CurrentLevelUpgrade;
		}

		[UsedImplicitly]
		private void OnPiecesUpgradesChanged()
		{
			for (int i = 0; i < piecesUpgrades.Count; i++)
			{
				piecesUpgrades[i].Initialize(this);
				piecesUpgrades[i].prefsKeyIndex = i;
			}
			rarePieceUpgrades.ForEach(delegate(RarePieceListItem p)
			{
				p.Initialize(this);
			});
		}

		private void OnEnable()
		{
			OnPiecesUpgradesChanged();
		}

		public override IReadOnlyList<ListItem> GetTabItems()
		{
			List<ListItem> list = new List<ListItem>();
			list.AddRange(new List<ListItem>(AllUpgrades));
			list.AddRange(new List<ListItem>(PiecesUpgrades));
			list.AddRange(new List<ListItem>(RarePieceUpgrades));
			return list;
		}

	}
}
