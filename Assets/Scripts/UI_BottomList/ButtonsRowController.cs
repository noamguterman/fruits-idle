using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Settings.UI.Tabs;
using Sirenix.OdinInspector;
using UI.BottomList.Buttons;
using UI.ListItems;
using UI.ListItems.Buttons;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI.BottomList
{
	public class ButtonsRowController : MonoBehaviour
    {
        [SerializeField]
        private float buttonsSpacing;

        [SerializeField]
        private float buttonsTabsSpacing;

        [SerializeField]
        private float buttonsAnimationMoveTime;

        [SerializeField]
        private float buttonChangeStateAnimationTime;

        [SerializeField]
        private AnimationCurve buttonsMoveAnimationCurve;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private ButtonBase upgradeButtonPrefab;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private TabController tabControllerPrefab;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private RectTransform viewport;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private ScrollRect scrollRect;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private VerticalLayoutGroup tabPanel;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private TabsSettings tabsSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private SoundSettings soundSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private PressVariationsSettings pressVariationsSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private ButtonsSettings buttonsSettings;

        private List<ButtonBase> allButtons;

        private List<ButtonBase> enabledButtons;

        private TabsManager tabsManager;

        private ButtonsStateManager buttonsStateManager;

        private Canvas canvas;

        private void OnDestroy()
		{
			TabsManager tabsManager = this.tabsManager;
			if (tabsManager != null)
			{
				tabsManager.Dispose();
			}
			ButtonsStateManager buttonsStateManager = this.buttonsStateManager;
			if (buttonsStateManager == null)
			{
				return;
			}
			buttonsStateManager.Dispose();
		}

		private IEnumerator Start()
		{
			allButtons = new List<ButtonBase>();
			enabledButtons = new List<ButtonBase>();
			canvas = GetComponentInParent<Canvas>();
			yield return null;
			Dictionary<int, TabContainer> dictionary = new Dictionary<int, TabContainer>();
			foreach (Tab tab in from t in tabsSettings.Tabs
			where t.IsEnabled
			select t)
			{
				if (dictionary.ContainsKey(tab.TabID))
				{
					dictionary[tab.TabID].AddRange(SetButtonsSettings(tab.GetTabItems()));
					dictionary[tab.TabID].Order();
				}
				else
				{
					dictionary.Add(tab.TabID, new TabContainer(SetButtonsSettings(tab.GetTabItems()).OrderBy((ListItem t) => t.Order).ToList<ListItem>(), tab.TabSprite, tab.SelectedTabSprite));
				}
			}
			int num = dictionary.SelectMany((KeyValuePair<int, TabContainer> l) => l.Value.ListItems).Count((ListItem b) => b.IsEnabled);
			RectTransform content = scrollRect.content;
			content.anchorMin = Vector2.zero;
			content.anchorMax = Vector2.right;
			int num2 = dictionary.Count - 1;
			content.sizeDelta = new Vector2
			{
				x = num * upgradeButtonPrefab.Size.x - viewport.rect.size.x + buttonsSpacing * (num - num2 + 1) + buttonsTabsSpacing * num2,
				y = content.sizeDelta.y
			};
			float x = content.offsetMax.x - content.offsetMin.x;
			content.offsetMin = content.offsetMin.SetX(0f);
			content.offsetMax = content.offsetMax.SetX(x);
			List<TabController> list = new List<TabController>();
			int i;
			foreach (KeyValuePair<int, TabContainer> keyValuePair in dictionary)
			{
				TabController tabController = tabControllerPrefab.PullOrCreate<TabController>();
				tabController.transform.SetParent(tabPanel.transform, false);
				tabController.Initialize(keyValuePair.Value.Tab, keyValuePair.Value.SelectedTab, keyValuePair.Key);
				list.Add(tabController);
				List<ListItem> list2 = (from u in keyValuePair.Value.ListItems
				orderby u.Order
				select u).ToList<ListItem>();
				for (i = 0; i < list2.Count; i++)
				{
					ListItem listItem = list2[i];
                    ButtonBase buttonBase = (listItem.UseCustomButtonPrefab && !listItem.ButtonPrefab.IsNull()) ? listItem.ButtonPrefab.PullOrCreate<ButtonBase>() : upgradeButtonPrefab.PullOrCreate<ButtonBase>();
					buttonBase.ButtonClick += OnButtonClick;
					buttonBase.transform.SetParent(scrollRect.content, false);
					buttonBase.AnchoredPosition = ((enabledButtons.Count == 0) ? (Vector2.right * (buttonsSpacing + buttonBase.Size.x / 2f)) : (enabledButtons.Last<ButtonBase>().AnchoredPosition + Vector2.right * (buttonBase.Size.x + ((i == 0) ? this.buttonsTabsSpacing : this.buttonsSpacing))));
					buttonBase.AnchoredPosition = buttonBase.AnchoredPosition.SetY(-buttonBase.Size.y / 2f);
					buttonBase.Initialize(listItem, tabController);
                    Debug.Log("------" + buttonBase.name + "   " + listItem.IsEnabled + "   " + listItem.Name + "    " + keyValuePair.Value.Tab.name);

                    if (listItem.IsEnabled)
					{
						enabledButtons.Add(buttonBase);
					}
					allButtons.Add(buttonBase);
				}
			}

			tabsManager = new TabsManager(canvas, list, scrollRect, enabledButtons, soundSettings.GetAudioSource(SoundType.UIElementClick), buttonsSpacing, tabsSettings);
			buttonsStateManager = new ButtonsStateManager(allButtons, canvas, scrollRect, buttonsSpacing, buttonsAnimationMoveTime, buttonChangeStateAnimationTime, buttonsMoveAnimationCurve);
			buttonsStateManager.ButtonStateChange += delegate(ButtonBase button, bool enabled)
			{
				if (enabled)
				{
					int num3 = allButtons.IndexOf(button);
					if (num3 > 0)
					{
						int index1 = num3;
						num3 = enabledButtons.IndexOf(allButtons.TakeWhile((ButtonBase b, int ii) => ii < index1).LastOrDefault((ButtonBase b) => b.enabled)) + 1;
					}
					enabledButtons.Insert(num3, button);
					return;
				}
				enabledButtons.Remove(button);
			};
			StartCoroutine(CoroutineUtils.ExecuteAfter(delegate()
			{
				allButtons.ForEach(delegate(ButtonBase b)
				{
					b.Refresh();
				});
			}, (YieldInstruction)null));
			yield break;
		}

		private void OnButtonClick()
		{
			allButtons.ForEach(delegate(ButtonBase b)
			{
				b.Refresh();
			});
		}

		private List<ListItem> SetButtonsSettings(IReadOnlyList<ListItem> list)
		{
			List<ListItem> list2 = list.ToList<ListItem>();
			list2.ForEach(delegate(ListItem l)
			{
				if (!l.ShowButtonOptions)
				{
					l.SetButtonSettings(buttonsSettings);
				}
			});
			return list2;
		}

	}
}
