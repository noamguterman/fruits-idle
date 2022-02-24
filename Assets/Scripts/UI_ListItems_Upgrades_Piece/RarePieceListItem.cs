using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.ListItems.Upgrades.Piece
{
	[Serializable]
	public sealed class RarePieceListItem : PieceListItem
	{
		public float PieceHealthMultiplier
		{
			get
			{
				return this.pieceHealthMultiplier;
			}
		}

		public float PieceCost
		{
			get
			{
				return this.pieceCost;
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

		public override string GetPrefsKey()
		{
			return string.Format("rarePiecesUpgrades{0}", this.prefsKeyIndex);
		}

		//[FoldoutGroup("$Name", false, 0)]
		[SerializeField]
		private float pieceCost;

		//[FoldoutGroup("$Name", false, 0)]
		[SerializeField]
		private float pieceHealthMultiplier;
	}
}
