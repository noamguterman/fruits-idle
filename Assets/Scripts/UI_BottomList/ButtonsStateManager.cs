using System;
using System.Collections.Generic;
using System.Linq;
using UI.BottomList.Buttons;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using Utilities.AlphaAnimation;

namespace UI.BottomList
{
	public class ButtonsStateManager : IDisposable
    {
        private readonly List<ButtonBase> allButtons;

        private readonly Canvas canvas;

        private readonly ScrollRect scrollRect;

        private readonly float buttonsSpacing;

        private readonly float buttonsAnimationMoveTime;

        private readonly float buttonChangeStateAnimationTime;

        private readonly AnimationCurve buttonsMoveAnimationCurve;

        private MultipleCoroutine currentLastAnimationRoutine;

        public event Action<ButtonBase, bool> ButtonStateChange;

		public ButtonsStateManager(List<ButtonBase> allButtons, Canvas canvas, ScrollRect scrollRect, float buttonsSpacing, float buttonsAnimationMoveTime, float buttonChangeStateAnimationTime, AnimationCurve buttonsMoveAnimationCurve)
		{
			this.allButtons = allButtons;
			this.canvas = canvas;
			this.scrollRect = scrollRect;
			this.buttonsSpacing = buttonsSpacing;
			this.buttonsAnimationMoveTime = buttonsAnimationMoveTime;
			this.buttonChangeStateAnimationTime = buttonChangeStateAnimationTime;
			this.buttonsMoveAnimationCurve = buttonsMoveAnimationCurve;
			allButtons.ForEach(delegate(ButtonBase b)
			{
				b.ButtonStateChange += this.OnButtonStateChange;
			});
		}

		private void OnButtonStateChange(ButtonBase button, bool isEnabled)
		{
            ChainRoutine(isEnabled ? EnableButton(button) : DisableButton(button));
        }

		private MultipleCoroutine EnableButton(ButtonBase button)
		{
			List<ButtonBase> buttonsToRightSide = new List<ButtonBase>();
			float canvasScale = 0f;
			float d = 0f;
            Action<ButtonBase> action_7 = null;
            Action action_8 = null;

			return new MultipleCoroutine(null)
			{
				delegate()
				{
					int buttonIndex = allButtons.IndexOf(button);
					buttonsToRightSide = (from b in allButtons.SkipWhile((ButtonBase b, int i) => i <= buttonIndex)
					where b.gameObject.activeSelf select b).ToList<ButtonBase>();
					ButtonBase buttonBase = this.allButtons.TakeWhile((ButtonBase b, int i) => i <= buttonIndex).Last((ButtonBase b) => b.gameObject.activeSelf);
					canvasScale = canvas.transform.localScale.x;
					d = button.Size.x + buttonsSpacing;
					RectTransform content = scrollRect.content;
					scrollRect.velocity = Vector2.zero;
					content.sizeDelta -= Vector2.left * d;
					content.transform.position -= Vector3.left * d * canvasScale / 2f;
					Action<ButtonBase, bool> buttonStateChange = this.ButtonStateChange;
					if (buttonStateChange != null)
					{
						buttonStateChange(button, true);
					}
					button.Position = buttonBase.Position + Vector3.right * d * canvasScale;
					return null;
				},
				delegate()
				{
					scrollRect.enabled = false;
					Action<ButtonBase> action;

                    if((action = action_7) == null)
                    {
                        action = (action_7 = delegate(ButtonBase rb)
                        {
                            rb.transform.MovePositionX(rb.transform.position.x + d * canvasScale, buttonsAnimationMoveTime, buttonsMoveAnimationCurve);
                        });
                    }
                    buttonsToRightSide.ForEach(action);
					return CoroutineUtils.Delay(buttonsAnimationMoveTime);
				},
				delegate()
				{
                    button.AnimateAlpha(0f, null);
					button.gameObject.SetActive(true);
					button.AnimateAlpha(1f, buttonChangeStateAnimationTime, false, 0f, null);
					float time = buttonChangeStateAnimationTime;
					Action postExecute;
                    if ((postExecute = action_8) == null)
                    {
                        postExecute = (action_8 = delegate()
                        {
                            scrollRect.enabled = true;
                        });
                    }
                    return CoroutineUtils.Delay(time, postExecute);
				}
			};
		}

		private MultipleCoroutine DisableButton(ButtonBase button)
		{
			List<ButtonBase> buttonsToRightSide = new List<ButtonBase>();
			float canvasScale = 0f;
			float d = 0f;
            Action<ButtonBase> action_6 = null;
            Action action_7 = null;

            return new MultipleCoroutine(null)
			{
				delegate()
				{
                    int buttonIndex = allButtons.IndexOf(button);
					buttonsToRightSide = (from b in allButtons.SkipWhile((ButtonBase b, int i) => i <= buttonIndex)
					where b.gameObject.activeSelf select b).ToList<ButtonBase>();
					canvasScale = canvas.transform.localScale.x;
					d = button.Size.x + buttonsSpacing;
					RectTransform content = scrollRect.content;
					scrollRect.velocity = Vector2.zero;
					content.sizeDelta += Vector2.left * d;
					content.transform.position += Vector3.left * d * canvasScale / 2f;
					Action<ButtonBase, bool> buttonStateChange = this.ButtonStateChange;
					if (buttonStateChange != null)
					{
						buttonStateChange(button, false);
					}
					return null;
				},
				delegate()
				{
                    scrollRect.enabled = false;
					button.AnimateAlpha(0f, buttonChangeStateAnimationTime, delegate(ButtonBase b)
					{
						b.gameObject.SetActive(false);
					});
					return CoroutineUtils.Delay(buttonChangeStateAnimationTime);
				},
				delegate()
				{
                    Action<ButtonBase> action;
                    if ((action = action_6) == null)
                    {
                        action = (action_6 = delegate(ButtonBase rb)
                        {
                            rb.transform.MovePositionX(rb.transform.position.x - d * canvasScale, this.buttonsAnimationMoveTime, this.buttonsMoveAnimationCurve);
                        });
                    }

					buttonsToRightSide.ForEach(action);
					float time = buttonsAnimationMoveTime;
					Action postExecute;
                    if ((postExecute = action_7) == null)
                    {
                        postExecute = (action_7 = delegate()
                        {
                            this.scrollRect.enabled = true;
                        });
                    }

					return CoroutineUtils.Delay(time, postExecute);
				}
			};
		}

		private void ChainRoutine(MultipleCoroutine routine)
		{
            MultipleCoroutine multipleCoroutine = this.currentLastAnimationRoutine;
            this.currentLastAnimationRoutine = routine;
            this.currentLastAnimationRoutine.IsRunning = true;
            if (multipleCoroutine != null && multipleCoroutine.IsRunning)
            {
                CoroutineUtils.StartCoroutine(CoroutineUtils.ExecuteAfter(() => {
                    this.currentLastAnimationRoutine.StartCoroutines();
                }, multipleCoroutine));
                return;
            }

            this.currentLastAnimationRoutine.StartCoroutines();

        }

		public void Dispose()
		{
			ButtonStateChange = null;
		}

	}
}
