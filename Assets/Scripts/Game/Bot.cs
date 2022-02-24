using System;
using System.Collections.Generic;
using System.Linq;
using Settings.UI.Tabs;
using Sirenix.OdinInspector;
using UI;
using UI.ListItems;
using UIControllers;
using UnityEngine;
using Utilities;

namespace Game
{
	public class Bot : MonoBehaviour
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private List<Tab> botClickableTabs;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private TouchArea touchArea;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private BonusView bonusView;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private LevelRow levelRow;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private EarningsDialog earningsDialog;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private DD_DataDiagram dataDiagram;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Range(0f, 1f)]
        private float getMoneyRewardedChance;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private float clickRate;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private float timeScale;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private bool clickOnBoost;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private int rangeAvg;

        private GameObject line;

        private GameObject levelUpLine;

        private GameObject avgLine;

        private float lastUpgradedTime;

        private float lastTimeLevelUp;

        private int upgradesBeforeLevelUp;

        private List<float> values = new List<float>();

        private bool isUpgradedInCurrentFrame;

        private void Start()
		{
			line = dataDiagram.AddLine("New Line");
			levelUpLine = dataDiagram.AddLine("New Line", Color.red);
			avgLine = dataDiagram.AddLine("New Line", Color.blue);
			foreach (Tab tab in botClickableTabs)
			{
				using (IEnumerator<ListItem> enumerator2 = tab.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						ListItem item = enumerator2.Current;
						item.Refresh += delegate()
						{
							Action executeAction;
							executeAction = new Action(delegate()
							{
								OnUIRefresh(item);
							});

                            StartCoroutine(CoroutineUtils.ExecuteAfter(executeAction, new WaitForFixedUpdate()));
						};
						OnUIRefresh(item);
					}
				}
			}
			bonusView.BonusButtonAvailable += delegate()
			{
				if (clickOnBoost)
				{
					StartCoroutine(CoroutineUtils.ExecuteAfter(new Action(bonusView.SimulateClick), new WaitForFixedUpdate()));
				}
			};
			levelRow.LevelUpAvailable += delegate()
			{
				StartCoroutine(CoroutineUtils.ExecuteAfter(delegate()
				{
					dataDiagram.InputPoint(levelUpLine, new Vector2((float)upgradesBeforeLevelUp, Time.time - lastTimeLevelUp));
					upgradesBeforeLevelUp = 0;
					lastTimeLevelUp = Time.time;
					levelRow.SimulateClick();
				}, new WaitForFixedUpdate()));
			};
			earningsDialog.EarningsDialogVisibilityChange += delegate(bool isEnabled)
			{
				if (isEnabled)
				{
					StartCoroutine(CoroutineUtils.Delay(2f, delegate()
					{
						earningsDialog.SimulateClick(UnityEngine.Random.value < getMoneyRewardedChance);
					}));
				}
			};
			StartCoroutine(CoroutineUtils.LoopExecute(new Action(touchArea.SimulateClick), new WaitForSeconds(clickRate)));
			Time.timeScale = timeScale;
		}

		private void OnUIRefresh(ListItem item)
		{
			if (isUpgradedInCurrentFrame)
			{
				return;
			}
			if (item.IsEnabled && item.IsAvailable && item.IsPurchasable && !item.IsLevelLocked)
			{
				isUpgradedInCurrentFrame = true;
				StartCoroutine(CoroutineUtils.ExecuteAfter(delegate()
				{
					if (lastUpgradedTime.Equals(0f))
					{
						lastUpgradedTime = Time.time;
					}
					float num = Time.time - lastUpgradedTime;
					if (num > 0.5f)
					{
						values.Add(num);
						dataDiagram.InputPoint(line, new Vector2(1f, Time.time - lastUpgradedTime));
						upgradesBeforeLevelUp++;
						if (values.Count == rangeAvg)
						{
							dataDiagram.InputPoint(avgLine, new Vector2((float)rangeAvg, values.Average()));
							values.Clear();
						}
					}
					lastUpgradedTime = Time.time;
					item.OnButtonClick();
					isUpgradedInCurrentFrame = false;
				}, new WaitForFixedUpdate()));
			}
		}

	}
}
