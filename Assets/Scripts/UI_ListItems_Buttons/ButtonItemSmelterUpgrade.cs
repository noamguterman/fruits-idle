using System;
using UI.ListItems.Upgrades.Piece.Smelter;
using UnityEngine;
using Utilities;

namespace UI.ListItems.Buttons
{
	[Serializable]
	public class ButtonItemSmelterUpgrade : ButtonItem
	{
		public override string Name
		{
			get
			{
				return this.upgradeType.ToString().AddSpaces();
			}
		}

		public SmelterUpgradeType UpgradeType
		{
			get
			{
				return this.upgradeType;
			}
		}

		[SerializeField]
		[HideInInspector]
		private SmelterUpgradeType upgradeType;
	}
}
