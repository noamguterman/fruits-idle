using System;
using System.Collections.Generic;
using UI.ListItems;
using UnityEngine;

namespace Settings.UI.Tabs
{
	[CreateAssetMenu(menuName = "Settings/Press Variations Settings", fileName = "Press Variations Settings")]
	public class PressVariationsSettings : Tab
    {
        [SerializeField]
        private List<PressVariation> pressVariations;

        private int CurrentPressIndex
		{
			get
			{
				return PlayerPrefs.GetInt("CurrentPressIndex");
			}
			set
			{
				PlayerPrefs.SetInt("CurrentPressIndex", value);
			}
		}

		public PressVariation CurrentPressVariation
		{
			get
			{
				return pressVariations[CurrentPressIndex];
			}
		}

		public override IReadOnlyList<ListItem> GetTabItems()
		{
			return CurrentPressVariation.PressUpgrades;
		}

	}
}
