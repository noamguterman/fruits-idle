using System;
using UI.ListItems.Upgrades.Piece;
using UnityEngine;
using Utilities;

namespace UI.ListItems.Buttons
{
	[Serializable]
	public class ButtonItemUpgrade : ButtonItem
	{
		public override string Name
		{
			get
			{
				return this.upgradeType.ToString().AddSpaces();
			}
		}

		public UpgradeType UpgradeType
		{
			get
			{
				return this.upgradeType;
			}
		}

		[SerializeField]
		[HideInInspector]
		private UpgradeType upgradeType;
	}
}
