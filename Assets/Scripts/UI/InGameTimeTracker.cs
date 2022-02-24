using System;
using Settings.UI.Tabs;
using Sirenix.OdinInspector;
using TMPro;
using UI.ListItems.Upgrades.Piece;
using UnityEngine;
using Utilities;

namespace UI
{
	public class InGameTimeTracker : MonoBehaviour
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private TMP_Text timeText;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private UpgradeSettings upgradeSettings;

        private DateTime startTime = DateTime.Now;

        private string CurrentTime
		{
			get
			{
				return TimeSpan.FromSeconds((double)((int)Time.time)).ToString();
			}
		}

		private string RealTime
		{
			get
			{
				return (DateTime.Now - startTime).ToString("hh\\:mm\\:ss");
			}
		}

		private void Awake()
		{
			upgradeSettings.GetUpgradeListItem(UpgradeType.SmeltersUnlock).LevelIncreased += delegate()
			{
				print(CurrentTime + " - Second Screen Unlock");
				StartCoroutine(CoroutineUtils.ExecuteAfter(delegate()
				{
					PlayerPrefs.Save();
				}, (YieldInstruction)null));
			};
		}

		private void Update()
		{
			timeText.SetText("In Game Time: " + CurrentTime + "\nReal time: " + RealTime);
		}

	}
}
