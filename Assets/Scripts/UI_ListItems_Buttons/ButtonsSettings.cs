using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UI.ListItems.Booster;
using UI.ListItems.Upgrades.Piece;
using UI.ListItems.Upgrades.Piece.Press;
using UI.ListItems.Upgrades.Piece.Smelter;
using UnityEngine;
using Utilities;

namespace UI.ListItems.Buttons
{
	[CreateAssetMenu(menuName = "Settings/Buttons Settings", fileName = "Buttons Settings")]
	public class ButtonsSettings : ScriptableObject
	{
		public List<ButtonItemUpgrade> ButtonItemsUpgrades
		{
			get
			{
				return this.buttonItemsUpgrades;
			}
		}

		public List<ButtonItemPressUpgrade> ButtonItemsPressUpgrades
		{
			get
			{
				return this.buttonItemsPressUpgrades;
			}
		}

		public List<ButtonItemBooster> ButtonItemsBoosters
		{
			get
			{
				return this.buttonItemsBoosters;
			}
		}

		public List<ButtonItemSmelterUpgrade> SmeltersUpgrades
		{
			get
			{
				return this.smeltersUpgrades;
			}
		}

		[GenerateListFromEnum(EnumType = typeof(UpgradeType))]
		[SerializeField]
		[ListDrawerSettings(IsReadOnly = true, HideAddButton = true)]
		private List<ButtonItemUpgrade> buttonItemsUpgrades;

		[GenerateListFromEnum(EnumType = typeof(PressUpgradeType))]
		[SerializeField]
		[ListDrawerSettings(IsReadOnly = true, HideAddButton = true)]
		private List<ButtonItemPressUpgrade> buttonItemsPressUpgrades;

		[GenerateListFromEnum(EnumType = typeof(BoosterType))]
		[SerializeField]
		[ListDrawerSettings(IsReadOnly = true, HideAddButton = true)]
		private List<ButtonItemBooster> buttonItemsBoosters;

		[GenerateListFromEnum(EnumType = typeof(SmelterUpgradeType))]
		[SerializeField]
		[ListDrawerSettings(IsReadOnly = true, HideAddButton = true)]
		private List<ButtonItemSmelterUpgrade> smeltersUpgrades;
	}
}
