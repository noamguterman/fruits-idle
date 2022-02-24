using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.ListItems.Upgrades.Piece
{
	public abstract class UpgradeListItem : ListItem
    {
        [SerializeField]
        [ShowIf("ShowOptionsButton", true)]
        private bool overriveButtonOptions;

        [SerializeField]
        private bool removeButtonOnMaxLevel;

        [SerializeField]
        [ListDrawerSettings(Expanded = false)]
        private List<Upgrade> upgrades;

        [SerializeField]
        [ListDrawerSettings(Expanded = false)]
        private List<UpgradeListItem.LevelLockSetting> levelLockSettings;

        [SerializeField]
        private bool useDeltaType;

        [ShowIf("useDeltaType", true)]
        [SerializeField]
        private bool defaultUpgradeOpened;

        [ShowIf("useDeltaType", true)]
        [SerializeField]
        private float defaultValue;

        [ShowIf("useDeltaType", true)]
        [SerializeField]
        private float deltaValue;

        [ShowIf("useDeltaType", true)]
        [SerializeField]
        private float defaultPrice;

        [ShowIf("useDeltaType", true)]
        [SerializeField]
        private float deltaPrice;

        [ShowIf("useDeltaType", true)]
        [SerializeField]
        private int maxLevel;

        [ShowIf("useDeltaType", true)]
        [SerializeField]
        private DeltaType deltaType;

        private Upgrade currentLevelUpgrade;

        protected string currentPriceText;

        protected string currentValueText;

        private readonly string valueIdentifier = "[VALUE]";

        [Serializable]
        private class LevelLockSetting
        {
            public int UpgradeLevel
            {
                get
                {
                    return upgradeLevel;
                }
            }

            public int RequiredLevel
            {
                get
                {
                    return requiredLevel;
                }
            }

            [SerializeField]
            private int upgradeLevel;

            [SerializeField]
            private int requiredLevel;
        }

		[ShowIf("useDeltaType", true)]
		[Button("Generate Upgrades")]
		private void GenerateUpgrades()
		{
			SetSettings(defaultUpgradeOpened, defaultValue, deltaValue, defaultPrice, deltaPrice, maxLevel, deltaType);
		}

		public Upgrade CurrentLevelUpgrade
		{
			get
			{
				if (currentLevelUpgrade == null || currentLevelUpgrade.Equals(new Upgrade()))
				{
					currentLevelUpgrade = GetCurrentUpgrade();
					currentPriceText = GetPriceText(currentLevelUpgrade);
					currentValueText = GetValueText(currentLevelUpgrade);
				}
				return currentLevelUpgrade;
			}
			private set
			{
				currentLevelUpgrade = value;
				currentPriceText = GetPriceText(currentLevelUpgrade);
				currentValueText = GetValueText(currentLevelUpgrade);
			}
		}

		public void SetSettings(bool defaultUpgradeOpened, float defaultValue, float deltaValue, float defaultPrice, float deltaPrice, int maxLevel, DeltaType deltaType)
		{
			this.defaultUpgradeOpened = defaultUpgradeOpened;
			this.defaultValue = defaultValue;
			this.deltaValue = deltaValue;
			this.defaultPrice = defaultPrice;
			this.deltaPrice = deltaPrice;
			this.maxLevel = maxLevel;
			this.deltaType = deltaType;
			upgrades.Clear();
			for (int i = defaultUpgradeOpened ? 1 : 0; i <= maxLevel; i++)
			{
				int num = defaultUpgradeOpened ? (i - 1) : i;
				float value = (deltaType == DeltaType.Additive) ? (defaultValue * (1f + deltaValue * num)) : (defaultValue * Mathf.Pow(deltaValue, num));
				upgrades.Add(new Upgrade(i, defaultPrice * Mathf.Pow(deltaPrice, num), value));
			}
		}

		public Upgrade GetUpgrade(int level)
		{
			return upgrades.First((Upgrade u) => u.Level == level);
		}

		public event Action LevelIncreased;

		[UsedImplicitly]
		public override bool ShowButtonOptions
		{
			get
			{
				return overriveButtonOptions;
			}
		}

		[UsedImplicitly]
		public override bool ShowOptionsButton
		{
			get
			{
				return true;
			}
		}

		public override string LevelText
		{
			get
			{
				Upgrade upgrade = CurrentLevelUpgrade;
				if (upgrade.Level != 0)
				{
					return "LVL " + upgrade.Level.ToString();
				}
				return string.Empty;
			}
		}

		public override int CurrentLockLevel
		{
			get
			{
				LevelLockSetting levelLockSetting = levelLockSettings.FirstOrDefault((LevelLockSetting l) => l.UpgradeLevel == CurrentLevelUpgrade.Level);
				if (levelLockSetting == null)
				{
					return 0;
				}
				return levelLockSetting.RequiredLevel;
			}
		}

		public override bool IsEnabled
		{
			get
			{
				return !removeButtonOnMaxLevel || CurrentLevelUpgrade.Level < upgrades.Last<Upgrade>().Level;
			}
		}

		public override string PriceText
		{
			get
			{
				return currentPriceText;
			}
		}

		public override string ValueText
		{
			get
			{
				return currentValueText;
			}
		}

		public override Sprite PriceImage
		{
			get
			{
				if (CurrentLevelUpgrade.PriceOfLevelUp > 0f)
				{
					return priceImage;
				}
				return null;
			}
		}

		public override Sprite ButtonImage
		{
			get
			{
				if (CurrentLevelUpgrade.PriceOfLevelUp > 0f)
				{
					return buttonImage;
				}
				return null;
			}
		}

		public override bool IsLocked
		{
			get
			{
				return CurrentLevelUpgrade.Level == 0;
			}
		}

		public override bool IsAvailable
		{
			get
			{
				return CurrentLevelUpgrade.Level < upgrades.Last<Upgrade>().Level;
			}
		}

		public override bool IsPurchasable
		{
			get
			{
				return GameData.GetMoney(MoneyType.Money) >= CurrentLevelUpgrade.PriceOfLevelUp;
			}
		}

		protected UpgradeListItem()
		{
			GameData.MoneyChange += delegate(float newMoney, MoneyType moneyType)
			{
				if (moneyType == MoneyType.Money)
				{
					InvokeUIRefresh();
				}
			};
			GameData.LevelChanged += delegate(int newLevel)
			{
				InvokeUIRefresh();
			};
		}

		private void IncreaseUpgradeLevel()
		{
            Debug.LogError("IncreaseUpgradeLevel = " + GetPrefsKey());
			if (GameData.GetUpgradeLevel(GetPrefsKey()) == 0)
			{
				GameData.SetUpgradeLevel(GetPrefsKey(), upgrades.First<Upgrade>().Level + 1);
			}
			else
			{
				GameData.IncreaseUpgradeLevel(GetPrefsKey());
			}
			CurrentLevelUpgrade = GetCurrentUpgrade();
			Action levelIncreased = this.LevelIncreased;
			if (levelIncreased == null)
			{
				return;
			}
			levelIncreased();
		}

		private string GetValueText(Upgrade upgrade)
		{
			if (upgrade.Value <= 0f && string.IsNullOrWhiteSpace(ValueType))
			{
				return string.Empty;
			}

            if (!ValueType.Contains(valueIdentifier))
                return ((upgrade.Value < 1f) ? Math.Round(upgrade.Value, 2).ToString(CultureInfo.InvariantCulture) : upgrade.Value.AbbreviateNumber(true)) + " " + ValueType;


            return valueType.Replace(valueIdentifier, Math.Round((double)CurrentLevelUpgrade.Value, 2).ToString(CultureInfo.InvariantCulture));

        }

        private string GetPriceText(Upgrade upgrade)
		{
			float priceOfLevelUp = upgrade.PriceOfLevelUp;
			if (currentLevelUpgrade.Level >= upgrades.Last<Upgrade>().Level)
			{
				return "max";
			}
			if (priceOfLevelUp > 0f)
			{
				return priceOfLevelUp.AbbreviateNumber(true);
			}
			return "Get";
		}

		public void SetMaxLevel()
		{
            Debug.LogError("SetMaxLevel");
			if (currentLevelUpgrade.Level == upgrades.Last<Upgrade>().Level)
			{
				return;
			}
			GameData.SetUpgradeLevel(GetPrefsKey(), upgrades.Last<Upgrade>().Level);
			CurrentLevelUpgrade = GetCurrentUpgrade();
			InvokeUIRefresh();
		}

		protected Upgrade GetCurrentUpgrade()
		{
			int level = GameData.GetUpgradeLevel(GetPrefsKey());
			List<Upgrade> list = new List<Upgrade>(upgrades);
			level = Mathf.Clamp(level, list.First<Upgrade>().Level, list.Last<Upgrade>().Level);
			list.Reverse();
			Upgrade upgrade = list.First((Upgrade u) => u.Level <= level);
			list.Reverse();
			Upgrade upgrade2 = list.First((Upgrade u) => u.Level >= level);
			if (upgrade.Level == upgrade2.Level)
			{
				return upgrade;
			}
			float t = (level - upgrade.Level) / (float)(upgrade2.Level - upgrade.Level);
			return new Upgrade(level, Mathf.Lerp(upgrade.PriceOfLevelUp, upgrade2.PriceOfLevelUp, t), Mathf.Lerp(upgrade.Value, upgrade2.Value, t));
		}

		public override void OnButtonClick()
		{
			Upgrade upgrade = CurrentLevelUpgrade;
			float priceOfLevelUp = upgrade.PriceOfLevelUp;
			if (GameData.GetMoney(MoneyType.Money) < priceOfLevelUp)
			{
				return;
			}
			GameData.IncreaseMoney(priceOfLevelUp * -1f, MoneyType.Money, true);
			IncreaseUpgradeLevel();
		}

	}
}
