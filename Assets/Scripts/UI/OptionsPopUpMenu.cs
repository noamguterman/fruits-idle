using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities;
using Utilities.AlphaAnimation;

namespace UI
{
	[RequireComponent(typeof(Toggle))]
	public class OptionsPopUpMenu : MonoBehaviour
    {
        //[FoldoutGroup("Animator", 0)]
        [SerializeField]
        private Animator animator;

        //[FoldoutGroup("Animator", 0)]
        [SerializeField]
        [ShowIf("IsAnimatorAdded", true)]
        private string buttonClickTriggerName = "ButtonClick";

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private float elementsPadding;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private float animationTimeOn;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private float animationTimeOff;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private OptionsPopUpMenu.AnimationType animationType;

        private List<Selectable> allSelectables;

        private Toggle optionsButton;

        private bool isExpanded;

        private enum AnimationType
        {
            Alpha,
            Scale
        }

        private bool IsAnimatorAdded
		{
			get
			{
				return animator != null;
			}
		}

		private void Awake()
		{
			optionsButton = GetComponent<Toggle>();
		}

		private void Start()
		{
			allSelectables = new List<Selectable>();
			allSelectables = new List<Selectable>((from t in GetComponentsInChildren<Selectable>(false)
			where t != optionsButton
			select t).ToList<Selectable>());
			allSelectables.ForEach(delegate(Selectable t)
			{
				t.transform.localPosition = Vector3.zero;
				t.transform.SetParent(transform.parent, false);
				t.image.rectTransform.sizeDelta = optionsButton.image.rectTransform.rect.size;
				t.gameObject.SetActive(false);
			});
		}

		private void OnEnable()
		{
			optionsButton.onValueChanged.AddListener(new UnityAction<bool>(OnOptionsToggleClick));
		}

		private void OnDisable()
		{
			optionsButton.onValueChanged.RemoveListener(new UnityAction<bool>(OnOptionsToggleClick));
		}

		public void Close()
		{
			optionsButton.isOn = false;
		}

		private void OnOptionsToggleClick(bool isEnabled)
		{
			if (isEnabled)
			{
				Rect rect = optionsButton.image.rectTransform.rect;
				float num = rect.size.y + elementsPadding + base.transform.localPosition.y;
				float height = rect.height;
				AnimationType animationType = this.animationType;
				if (animationType != AnimationType.Alpha)
				{
					if (animationType != AnimationType.Scale)
					{
						throw new ArgumentOutOfRangeException();
					}
					for (int i = 0; i < allSelectables.Count; i++)
					{
						Selectable t = allSelectables[i];
						t.gameObject.SetActive(true);
						t.AnimateAlpha(1f, null);
						Transform transform = t.transform;
						transform.localPosition = new Vector3(base.transform.localPosition.x, -num - (float)i * (height + elementsPadding), -num - (float)i * (height + elementsPadding));
						transform.localScale = Vector3.zero;
						StartCoroutine(CoroutineUtils.Delay((float)i * animationTimeOn * 0.35f, delegate()
						{
							t.transform.MoveScale(Vector3.one, animationTimeOn, null);
						}));
					}
				}
				else
				{
					for (int j = 0; j < allSelectables.Count; j++)
					{
						Selectable selectable = allSelectables[j];
						selectable.gameObject.SetActive(true);
						selectable.AnimateAlpha(0f, null);
						selectable.AnimateAlpha(1f, animationTimeOn, false, 0f, null);
						selectable.transform.MoveLocalPosition(new Vector3(transform.localPosition.x, -num - (float)j * (height + elementsPadding), -num - (float)j * (height + elementsPadding)), animationTimeOn);
					}
				}
			}
			else
			{
				AnimationType animationType = this.animationType;
				if (animationType != AnimationType.Alpha)
				{
					if (animationType != AnimationType.Scale)
					{
						throw new ArgumentOutOfRangeException();
					}
					for (int k = 0; k < allSelectables.Count; k++)
					{
						allSelectables[k].transform.MoveScale(Vector3.zero, animationTimeOff, null);
					}
				}
				else
				{
					for (int l = 0; l < allSelectables.Count; l++)
					{
						Selectable selectable2 = allSelectables[l];
						selectable2.AnimateAlpha(0f, animationTimeOff, delegate(Selectable optT)
						{
							optT.gameObject.SetActive(false);
						});
						selectable2.transform.MoveLocalPosition(transform.localPosition, animationTimeOff);
					}
				}
			}
			if (IsAnimatorAdded)
			{
				animator.SetTrigger(buttonClickTriggerName);
			}
		}

	}
}
