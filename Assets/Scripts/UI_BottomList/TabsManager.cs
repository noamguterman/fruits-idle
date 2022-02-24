using System;
using System.Collections.Generic;
using System.Linq;
using Game.Audio;
using Settings.UI.Tabs;
using UI.BottomList.Buttons;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI.BottomList
{
	public class TabsManager : IDisposable
    {
        private readonly TabsSettings tabsSettings;

        private readonly List<TabController> tabs;

        private readonly List<ButtonBase> enabledButtons;

        private readonly ScrollRect scrollRect;

        private readonly Canvas canvas;

        private readonly float buttonsSpacing;

        private readonly Coroutine updateTabsRoutine;

        private TabController currentSelectedTabController;

        //Action _003C_003E9__2;

        public TabsManager(Canvas canvas, List<TabController> tabs, ScrollRect scrollRect, List<ButtonBase> enabledButtons, AudioSourceListener tabClickSound, float buttonsSpacing, TabsSettings tabsSettings)
		{
            //TabsManager _003C_003E4__this = this;

            //Debug.LogError("TabsManager");
			this.canvas = canvas;
			this.tabs = tabs;
			this.tabsSettings = tabsSettings;
			this.scrollRect = scrollRect;
			this.enabledButtons = enabledButtons;
			this.buttonsSpacing = buttonsSpacing;
			TabController tabController = tabs.First((TabController t) => t.Index == tabsSettings.DefaultTab.TabID);
			SetCurrentTab(tabController);
			MoveToTab(tabController, 0f);
			updateTabsRoutine = CoroutineUtils.StartCoroutine(CoroutineUtils.LoopExecute(new Action(Update), null));

            //tabs.ForEach(delegate (TabController tab)
            //{
            //    TabController tab2 = tab;
            //    Action value;
            //    if ((value = _003C_003E9__2) == null)
            //    {
            //        value = (_003C_003E9__2 = delegate ()
            //        {
            //            tabClickSound.Play(false);
            //        });
            //    }
            //    tab2.ButtonClick += value;
            //    tab.ButtonClick += delegate ()
            //    {
            //        _003C_003E4__this.MoveToTab(tab, tabsSettings.OnTabClickScrollTime);
            //    };
            //});

            tabs.ForEach(delegate (TabController tab)
            {
                TabController tab2 = tab;
                Action value;
                value = new Action(delegate ()
                {
                    tabClickSound.Play(false);
                });

                tab2.ButtonClick += value;
                tab.ButtonClick += delegate ()
                {
                    MoveToTab(tab, tabsSettings.OnTabClickScrollTime);
                };
            });
        }

		private void Update()
		{
			if (enabledButtons.Count == 0 || !scrollRect.enabled)
			{
				return;
			}
			TabController tabController = null;
			float num = float.PositiveInfinity;
			if (enabledButtons.Last<ButtonBase>().Position.x < canvas.CanvasSize().x)
			{
				tabController = enabledButtons.Last<ButtonBase>().TabController;
			}
			else
			{
				for (int i = 0; i < enabledButtons.Count; i++)
				{
					float x = enabledButtons[i].transform.position.x;
					if (x > 0f && x < num)
					{
						num = x;
						tabController = enabledButtons[i].TabController;
					}
				}
			}
			if (currentSelectedTabController == tabController)
			{
				return;
			}
			SetCurrentTab(tabController);
		}

		private void SetCurrentTab(TabController tabController)
		{
			TabController tabController2 = currentSelectedTabController;
			if (tabController2 != null)
			{
				tabController2.SetSelectState(false);
			}
			currentSelectedTabController = tabController;
			tabController.SetSelectState(true);
		}

		private void MoveToTab(TabController tab, float time = 0f)
		{
			if (!scrollRect.enabled)
			{
				return;
			}
			scrollRect.velocity = Vector2.zero;
			ButtonBase buttonBase = enabledButtons.First((ButtonBase b) => b.TabController == tab);
			RectTransform content = scrollRect.content;
			Transform transform = canvas.transform;
			Vector3 localScale = transform.localScale;
			Vector3 position = content.transform.position;
			AnimationCurve onTabClickScrollCurve = tabsSettings.OnTabClickScrollCurve;
			RectTransform rectTransform = (RectTransform)scrollRect.transform.parent;
			Vector2 offsetMin = rectTransform.offsetMin;
			float newX = Mathf.Max(position.x - buttonBase.Position.x + (buttonBase.Size.x / 2f + buttonsSpacing + offsetMin.x) * localScale.x, (-content.rect.size.x + rectTransform.rect.size.x + ((RectTransform)transform).rect.size.x + offsetMin.x) * localScale.x / 2f);
			content.transform.MovePositionX(newX, time, onTabClickScrollCurve);
		}

		public void Dispose()
		{
			CoroutineUtils.StopCoroutine(updateTabsRoutine);
		}

	}
}
