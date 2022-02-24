using System;
using UI.ListItems.Upgrades.Piece.Press;
using UnityEngine;
using Utilities;

namespace UI.ListItems.Buttons
{
	[Serializable]
	public class ButtonItemPressUpgrade : ButtonItem
	{
		public override string Name
		{
			get
			{
				return this.upgradeType.ToString().AddSpaces();
			}
		}

		public PressUpgradeType UpgradeType
		{
			get
			{
				return this.upgradeType;
			}
		}

		[SerializeField]
		[HideInInspector]
		private PressUpgradeType upgradeType;
	}
}
