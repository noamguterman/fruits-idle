using System;
using System.Linq;
using Game.Pieces;
using Settings.UI.Tabs;
using Sirenix.OdinInspector;
using UI.ListItems.Buttons;
using UnityEngine;

namespace UI.ListItems.Upgrades.Piece
{
	[Serializable]
	public class PieceListItem : UpgradeListItem
    {
        //[FoldoutGroup("$Name", false, 0)]
        [SerializeField]
        private Game.Pieces.Piece piece;

        //[FoldoutGroup("$Name", false, 0)]
        [SerializeField]
        private Material pieceMaterial;

        //[FoldoutGroup("$Name", false, 0)]
        [SerializeField]
        private MoneyType moneyType;

        [SerializeField]
        [HideInInspector]
        public int prefsKeyIndex = DefaultPrefsKeyIndex;

        [SerializeField]
        [HideInInspector]
        public int Index;

        private UpgradeSettings upgradeSettings;

        public static int DefaultPrefsKeyIndex
		{
			get
			{
				return -1;
			}
		}

		public bool IsActiveInGame { get; set; }

		public override bool IsEnabled
		{
			get
			{
				if (IsActiveInGame)
				{
					return true;
				}
				int num = upgradeSettings.PiecesUpgrades.ToList().IndexOf(this);
				return base.IsEnabled && (CurrentLevelUpgrade.Level > 0 || num == 0 || upgradeSettings.PiecesUpgrades[num - 1].CurrentLevelUpgrade.Level > 0);
			}
		}

		public override bool ShowButtonOptions
		{
			get
			{
				return true;
			}
		}

		public override bool ShowOptionsButton
		{
			get
			{
				return false;
			}
		}

		public Game.Pieces.Piece Piece
		{
			get
			{
				return piece;
			}
		}

		public Material PieceMaterial
		{
			get
			{
				return pieceMaterial;
			}
		}

		public MoneyType MoneyType
		{
			get
			{
				return moneyType;
			}
		}

		public override void OnButtonClick()
		{
			base.OnButtonClick();
			if (GetCurrentUpgrade().Level == 1)
			{
			}
		}

		public int TypeID
		{
			get
			{
				return Index;
			}
		}

		public void Initialize(UpgradeSettings upgradeSettings)
		{
			this.upgradeSettings = upgradeSettings;
			IsActiveInGame = false;
		}

		public override void SetButtonSettings(ButtonsSettings buttonsSettings)
		{
		}

		public override string GetPrefsKey()
		{
			return string.Format("piecesUpgrades{0}", prefsKeyIndex);
		}

	}
}
