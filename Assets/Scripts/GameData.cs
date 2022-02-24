using System;
using UnityEngine;

public static class GameData
{
    private static float money;

    private static float moneyGem;

    private static bool isInitialized;

    public static void SetMoney(float value, MoneyType moneyType, bool increaseXP = true)
	{
		float num = GetMoney(moneyType);
		if (increaseXP && moneyType == MoneyType.Money && value > num)
		{
			CurrentLevelXp += value - num;
		}
		PlayerPrefs.SetFloat(moneyType.ToString(), value);
		if (moneyType != MoneyType.Money)
		{
			if (moneyType != MoneyType.Gem)
			{
				throw new ArgumentOutOfRangeException("moneyType", moneyType, null);
			}
			moneyGem = value;
		}
		else
		{
			money = value;
		}
		Action<float, MoneyType> moneyChange = MoneyChange;
		if (moneyChange == null)
		{
			return;
		}
		moneyChange(value, moneyType);
	}

	public static void IncreaseMoney(float value, MoneyType moneyType, bool increaseXP = true)
	{
		SetMoney(GetMoney(moneyType) + value, moneyType, increaseXP);
	}

	public static void IncreaseMoney(float value, MoneyType moneyType, Vector3 position, float scale, bool worldPos = true)
	{
		Action<Vector3, bool, float, float, MoneyType> moneyChangeWithAnimation = MoneyChangeWithAnimation;
		if (moneyChangeWithAnimation == null)
		{
			return;
		}
		moneyChangeWithAnimation(position, worldPos, value, scale, moneyType);
	}

	public static float GetMoney(MoneyType moneyType)
	{
		if (!isInitialized)
		{
			isInitialized = true;
			money = PlayerPrefs.GetFloat(MoneyType.Money.ToString(), 0f);
			moneyGem = PlayerPrefs.GetFloat(MoneyType.Gem.ToString(), 5f);
		}
		if (moneyType == MoneyType.Money)
		{
			return money;
		}
		if (moneyType != MoneyType.Gem)
		{
			throw new ArgumentOutOfRangeException("moneyType", moneyType, null);
		}
		return moneyGem;
	}

	public static float CurrentLevelXp
	{
		get
		{
			return PlayerPrefs.GetFloat("CurrentLevelXp", 0f);
		}
		private set
		{
			PlayerPrefs.SetFloat("CurrentLevelXp", value);
		}
	}

	public static int CurrentLevel
	{
		get
		{
			return PlayerPrefs.GetInt("CurrentLevel", 1);
		}
		private set
		{
			PlayerPrefs.SetInt("CurrentLevel", value);
			Action<int> levelChanged = LevelChanged;
			if (levelChanged == null)
			{
				return;
			}
			levelChanged(value);
		}
	}

	public static event Action<float, MoneyType> MoneyChange;

	public static event Action<Vector3, bool, float, float, MoneyType> MoneyChangeWithAnimation;

	public static event Action<int> LevelChanged;

	public static int GetUpgradeLevel(string type)
	{
        //Debug.Log("~~~~type = " + type + "   " + PlayerPrefs.GetInt(type, 0));
		return PlayerPrefs.GetInt(type, 0);
	}

	public static void SetUpgradeLevel(string type, int level)
	{
		PlayerPrefs.SetInt(type, level);
	}

	public static void IncreaseUpgradeLevel(string type)
	{
		PlayerPrefs.SetInt(type, GetUpgradeLevel(type) + 1);
	}

	public static void IncreaseLevel()
	{
		CurrentLevel++;
		CurrentLevelXp = 0f;
	}

	public static void SetLevel(int level)
	{
		CurrentLevel = level;
		CurrentLevelXp = 0f;
	}

}
