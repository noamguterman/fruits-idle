using System;
using UI.ListItems.Booster;
using UnityEngine;
using Utilities;

namespace UI.ListItems.Buttons
{
	[Serializable]
	public class ButtonItemBooster : ButtonItem
	{
		public override string Name
		{
			get
			{
				return this.boosterType.ToString().AddSpaces();
			}
		}

		public BoosterType BoosterType
		{
			get
			{
				return this.boosterType;
			}
		}

		[SerializeField]
		[HideInInspector]
		private BoosterType boosterType;
	}
}
