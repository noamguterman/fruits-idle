using System;
using System.Linq;
using Sirenix.OdinInspector;
using UI.ListItems.Buttons;
using UnityEngine;
using Utilities;

namespace UI.ListItems.Upgrades.Piece
{
	[Serializable]
	public class UpgradeTypeListItem : UpgradeListItem
    {
        //[FoldoutGroup("$Name", 0)]
        [SerializeField]
        [HideInInspector]
        private UpgradeType upgradeType;

        public UpgradeType UpgradeType
		{
			get
			{
				return this.upgradeType;
			}
		}

		public override string Name
		{
			get
			{
				return this.upgradeType.ToString().AddSpaces();
			}
		}

		public override bool ShowButtonOptions
		{
			get
			{
				return false;
			}
		}

		public override bool ShowOptionsButton
		{
			get
			{
				return false;
			}
		}

		public override string GetPrefsKey()
		{
			return this.upgradeType.ToString();
		}

		public override void SetButtonSettings(ButtonsSettings buttonsSettings)
		{
			base.SetButtonSettings(buttonsSettings.ButtonItemsUpgrades.FirstOrDefault((ButtonItemUpgrade b) => b.UpgradeType == this.UpgradeType));
		}

	}
}
