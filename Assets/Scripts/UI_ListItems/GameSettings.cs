using System;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

namespace UI.ListItems
{
	[CreateAssetMenu(menuName = "Settings/Game Settings")]
	public class GameSettings : ScriptableObject
    {
        [SerializeField]
        private bool isLoggingEnabled;

        [SerializeField]
        private float ballSpawnInterval;

        [SerializeField]
        private float logoShowTime;

        [SerializeField]
        private float logoAnimationTime;

        [TitleGroup("Earnings Dialog Settings", null, TitleAlignments.Left, true, true, false, 0)]
        [SerializeField]
        [TextArea]
        private string offlineEarningsText;

        [SerializeField]
        [TextArea]
        private string onLevelIncreaseReardText;

        [Space(35f)]
        [SerializeField]
        [InlineButton("SetLevel", "Set Level")]
        private int level;

        [SerializeField]
        [InlineButton("AddMoney", "Add Money")]
        private float money;

        [UsedImplicitly]
		private void AddMoney()
		{
			GameData.IncreaseMoney(money, MoneyType.Money, true);
		}

		[UsedImplicitly]
		private void SetLevel()
		{
			GameData.SetLevel(level);
		}

		[Button("Delete Player Prefs", ButtonSizes.Large)]
		private void DeletePlayerPrefs()
		{
			PlayerPrefs.DeleteAll();
		}

		public float BallSpawnInterval
		{
			get
			{
				return ballSpawnInterval;
			}
		}

		public bool IsLoggingEnabled
		{
			get
			{
				return isLoggingEnabled;
			}
		}

		public string OfflineEarningsText
		{
			get
			{
				return offlineEarningsText;
			}
		}

		public string OnLevelIncreaseReardText
		{
			get
			{
				return onLevelIncreaseReardText;
			}
		}

		public float LogoShowTime
		{
			get
			{
				return logoShowTime;
			}
		}

		public float LogoAnimationTime
		{
			get
			{
				return logoAnimationTime;
			}
		}

	}
}
