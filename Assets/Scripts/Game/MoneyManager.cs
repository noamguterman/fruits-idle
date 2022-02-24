using System;
using Settings.UI.Tabs;
using Sirenix.OdinInspector;
using UI.ListItems;
using UI.ListItems.Booster;
using UIControllers;
using UnityEngine;

namespace Game
{
	public class MoneyManager : MonoBehaviour
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private BoostersSettings boostersSettings;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private BonusView bonusView;

        private float boosterIncomeMultiplier;

        private float bonusIncomeMultiplier;

        public float GetCost(Costable costable)
		{
			return costable.Cost * ((costable.MoneyType == MoneyType.Money) ? (boosterIncomeMultiplier * bonusIncomeMultiplier) : 1f);
		}

		public void Subscribe(Costable costable)
		{
			costable.AddMoney += delegate(Costable c)
			{
				GameData.IncreaseMoney(GetCost(c), c.MoneyType, c.Position, 0f, true);
			};
		}

		private void OnEnable()
		{
			boosterIncomeMultiplier = (bonusIncomeMultiplier = 1f);
			boostersSettings.ChangeBoosterTypeMutiplier += OnBoosterMutiplierChanged;
			bonusView.BonusStateChange += OnBonusStateChange;
		}

		private void OnDisable()
		{
			boostersSettings.ChangeBoosterTypeMutiplier -= OnBoosterMutiplierChanged;
			bonusView.BonusStateChange -= OnBonusStateChange;
		}

		private void OnBoosterMutiplierChanged(BoosterType boosterType, float multiplier, float time, bool isEnabled)
		{
			if (boosterType != BoosterType.MultiplierIncome)
			{
				return;
			}
			boosterIncomeMultiplier = multiplier;
		}

		private void OnBonusStateChange(Bonus b, bool isEnabled)
		{
			if (b.BonusType != BonusType.Money)
			{
				return;
			}
			bonusIncomeMultiplier = (isEnabled ? b.MoneyMultiplier : 1f);
		}

	}
}
