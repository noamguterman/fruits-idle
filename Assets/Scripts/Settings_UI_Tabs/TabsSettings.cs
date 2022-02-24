using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Settings.UI.Tabs
{
	[CreateAssetMenu(menuName = "Settings/Tabs Settings", fileName = "Tabs Settings")]
	public class TabsSettings : ScriptableObject
    {
        [TitleGroup("On Tab Click Scroll Settings", null, TitleAlignments.Left, true, true, false, 0)]
        [SerializeField]
        private float onTabClickScrollTime;

        [SerializeField]
        private AnimationCurve onTabClickScrollCurve;

        [SerializeField]
        private Tab defaultTab;

        [Space(25f)]
        [SerializeField]
        private List<Tab> tabs;

        public Tab DefaultTab
		{
			get
			{
				return defaultTab;
			}
		}

		public List<Tab> Tabs
		{
			get
			{
				return tabs;
			}
		}

		public float OnTabClickScrollTime
		{
			get
			{
				return onTabClickScrollTime;
			}
		}

		public AnimationCurve OnTabClickScrollCurve
		{
			get
			{
				return onTabClickScrollCurve;
			}
		}

	}
}
