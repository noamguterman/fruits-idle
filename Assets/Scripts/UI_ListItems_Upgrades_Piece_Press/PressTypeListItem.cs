using System;
using System.Linq;
using Sirenix.OdinInspector;
using UI.ListItems.Buttons;
using UnityEngine;
using Utilities;

namespace UI.ListItems.Upgrades.Piece.Press
{
	[Serializable]
	public class PressTypeListItem : UpgradeListItem
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

		public override string GetPrefsKey()
		{
			return this.upgradeType.ToString();
		}

		public override void SetButtonSettings(ButtonsSettings buttonsSettings)
		{
			base.SetButtonSettings(buttonsSettings.ButtonItemsPressUpgrades.FirstOrDefault((ButtonItemPressUpgrade b) => b.UpgradeType == this.UpgradeType));
		}

		//[FoldoutGroup("$Name", false, 0)]
		[SerializeField]
		private PressUpgradeType upgradeType;
	}
}
