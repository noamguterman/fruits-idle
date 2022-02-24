using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace UI.ListItems
{
	[CreateAssetMenu(menuName = "Settings/Bonus Settings")]
	public class BonusSettings : ScriptableObject
    {
        [SerializeField]
        private float bonusAppearInterval;

        [SerializeField]
        private float buttonStayTime;

        [SerializeField]
        [GenerateListFromEnum(EnumType = typeof(BonusType))]
        [ListDrawerSettings(IsReadOnly = true, HideAddButton = true)]
        private List<Bonus> bonuses;

        public float BonusAppearInterval
		{
			get
			{
				return bonusAppearInterval;
			}
		}

		public float ButtonStayTime
		{
			get
			{
				return buttonStayTime;
			}
		}

		public List<Bonus> Bonuses
		{
			get
			{
				return bonuses;
			}
		}

	}
}
