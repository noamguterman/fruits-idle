using System;
using UI;
using UnityEngine;
using Utilities;

namespace Game
{
	public static class ReviewGameController
    {
        private static readonly float DelayTime = 15f;

        private static RateWindow rateWindow;

        public static bool IsReviewGameWindowShowed
		{
			get
			{
				return PlayerPrefs.GetInt("IsReviewGameWindowShowed", 0) == 1;
			}
			private set
			{
				PlayerPrefs.SetInt("IsReviewGameWindowShowed", value ? 1 : 0);
			}
		}

		public static void Initialize(RateWindow rateWindow)
		{
			ReviewGameController.rateWindow = rateWindow;
			GameData.LevelChanged += OnLevelIncreased;
		}

		private static void OnLevelIncreased(int level)
		{
			if (level == 5 || level == 10)
			{
                GameData.LevelChanged -= OnLevelIncreased;
			    IsReviewGameWindowShowed = true;
			    CoroutineUtils.StartCoroutine(CoroutineUtils.Delay(DelayTime, new Action(RequestReview)));
            }
        }

        public static void RequestReview()
		{
			rateWindow.Show();
		}

	}
}
