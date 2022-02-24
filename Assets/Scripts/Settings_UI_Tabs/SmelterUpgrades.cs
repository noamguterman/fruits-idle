using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UI.ListItems;
using UI.ListItems.Upgrades.Piece;
using UI.ListItems.Upgrades.Piece.Smelter;
using UnityEngine;
using Utilities;

namespace Settings.UI.Tabs
{
	[CreateAssetMenu(menuName = "Settings/Smelter Upgrades", fileName = "Smelter Upgrades")]
	public class SmelterUpgrades : Tab
	{
		public Upgrade GetUpgrade(SmelterUpgradeType upgradeType)
		{
			return this.smelterUpgrades.First((SmelterTypeListItem u) => u.UpgradeType == upgradeType).CurrentLevelUpgrade;
		}

		public override IReadOnlyList<ListItem> GetTabItems()
		{
			return this.smelterUpgrades;
		}

		[SerializeField]
		[GenerateListFromEnum(EnumType = typeof(SmelterUpgradeType))]
		[ListDrawerSettings(IsReadOnly = true, HideAddButton = true)]
		private List<SmelterTypeListItem> smelterUpgrades;
	}
}
