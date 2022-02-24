using System;
using System.Globalization;
using System.Linq;
using Sirenix.OdinInspector;
using UI.ListItems.Buttons;
using UnityEngine;
using Utilities;

namespace UI.ListItems.Upgrades.Piece.Smelter
{
	[Serializable]
	public class SmelterTypeListItem : UpgradeListItem
    {
        //[FoldoutGroup("$Name", false, 0)]
        [SerializeField]
        [HideInInspector]
        private SmelterUpgradeType upgradeType;

        private readonly string valueIdentifier = "[VALUE]";

        public override string Name
		{
			get
			{
				return upgradeType.ToString().AddSpaces();
			}
		}

		public override bool IsEnabled
		{
			get
			{
				return base.IsEnabled && GameData.GetUpgradeLevel(UI.ListItems.Upgrades.Piece.UpgradeType.SmeltersUnlock.ToString()) > 0;
			}
		}

		public override string ValueText
		{
			get
			{
				string valueType = this.valueType;
				if (!valueType.Contains(valueIdentifier))
				{
                    return currentValueText;
				}
                
                //return valueType.Replace(valueIdentifier, (CurrentLevelUpgrade.Value < 1f) ? Math.Round((double)CurrentLevelUpgrade.Value, 2).ToString(CultureInfo.InvariantCulture) : CurrentLevelUpgrade.Value.AbbreviateNumber(true));
                return valueType.Replace(valueIdentifier, Math.Round((double)CurrentLevelUpgrade.Value, 2).ToString(CultureInfo.InvariantCulture));
            }
		}

		public override string GetPrefsKey()
		{
			return upgradeType.ToString();
		}

		public SmelterUpgradeType UpgradeType
		{
			get
			{
				return upgradeType;
			}
		}

		public override void SetButtonSettings(ButtonsSettings buttonsSettings)
		{
			SetButtonSettings(buttonsSettings.SmeltersUpgrades.FirstOrDefault((ButtonItemSmelterUpgrade s) => s.UpgradeType == upgradeType));
		}

		public override void OnButtonClick()
		{
			base.OnButtonClick();
		}

	}
}
