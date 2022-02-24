using System;
using System.Globalization;
using Settings.UI.Tabs;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace UI.ListItems.Booster
{
	[Serializable]
	public class BoosterListItemMoney : BoosterListItem
    {
        //[FoldoutGroup("$Name", false, 0)]
        [SerializeField]
        private float cost;

        //[FoldoutGroup("$Name", false, 0)]
        [SerializeField]
        private MoneyType moneyType;

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

		public override bool IsPurchasable
		{
			get
			{
				return !isActive && GameData.GetMoney(moneyType) >= cost;
			}
		}

		public override string PriceText
		{
			get
			{
				return cost.ToString(CultureInfo.InvariantCulture);
			}
		}

		public override void OnButtonClick()
		{
			if (GameData.GetMoney(moneyType) < cost || isActive)
			{
				return;
			}
			GameData.IncreaseMoney(-cost, moneyType, true);
			CoroutineUtils.StartCoroutine(CoroutineUtils.ExecuteAfter(new Action(InvokeUIRefresh),  (YieldInstruction)null));
			InvokeChangeBoosterTypeMutiplier(true);
			InvokeUIRefresh();
			Debug.Log(duration);
			InvokeUIStartTimer(duration).Elapsed += delegate()
			{
				InvokeChangeBoosterTypeMutiplier(false);
				InvokeUIRefresh();
			};
		}

		public override void Initialize(BoostersSettings boostersSettings)
		{
			base.Initialize(boostersSettings);
			GameData.MoneyChange += delegate(float f, MoneyType type)
			{
				if (type == moneyType)
				{
					InvokeUIRefresh();
				}
			};
		}

	}
}
