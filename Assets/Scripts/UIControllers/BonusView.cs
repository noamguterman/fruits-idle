using System;
using System.Linq;
using Game;
using Game.Audio;
using Sirenix.OdinInspector;
using TMPro;
using UI.Boosters;
using UI.ListItems;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utilities;

namespace UIControllers
{
	public class BonusView : MonoBehaviour
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private Button bonusButton;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private Text buttonText;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private Timer timer;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private BoosterDialog boosterDialog;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private BonusSettings bonusSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private SoundSettings soundSettings;

        private Bonus currentBonus;

        private MultipleCoroutine shakeButtonRoutine;

        private AudioSourceListener currentBonusAudio;

        public event Action<Bonus, bool> BonusStateChange;

		public event Action BonusButtonAvailable;

		private void Awake()
		{
			bonusButton.interactable = false;
			bonusButton.gameObject.SetActive(false);
			Preloader.LogoHided += delegate()
			{
				StartCoroutine(CoroutineUtils.Delay(bonusSettings.BonusAppearInterval, new Action(OnLogoHided)));
			};
		}

		private void OnLogoHided()
		{
			StartCoroutine(CoroutineUtils.LoopExecute(delegate()
			{
				foreach (Bonus bonus in from b in bonusSettings.Bonuses
				orderby b.AppearProbability descending
				select b)
				{
					if (currentBonus == null && UnityEngine.Random.value < bonus.AppearProbability)
					{
						SetButtonState(true);
						timer.StartTimer(bonusSettings.ButtonStayTime, Timer.OutputType.Seconds);
						timer.Elapsed += delegate()
						{
							SetButtonState(false);
							currentBonus = null;
						};
						bonusButton.image.color = bonus.BonusButtonColor;
						currentBonus = bonus;
						break;
					}
				}
			}, new WaitForSeconds(bonusSettings.BonusAppearInterval)));
		}

		private void SetButtonState(bool isEnabled)
		{
			if (isEnabled)
			{
				bonusButton.gameObject.SetActive(true);
				bonusButton.enabled = true;
				buttonText.gameObject.SetActive(true);
				buttonText.text = "Boost!";
				bonusButton.transform.localScale = Vector3.zero;
				bonusButton.transform.MoveScale(Vector3.one * 1.15f, 0.35f, delegate()
				{
					bonusButton.interactable = true;
					Action bonusButtonAvailable = BonusButtonAvailable;
                    if (bonusButtonAvailable != null)
                    {
                        bonusButtonAvailable();
                    }

                    MultipleCoroutine multipleCoroutine2 = new MultipleCoroutine(this);
					multipleCoroutine2.Add(() => TransformUtils.LerpScale(bonusButton.transform, Vector3.one, 0.35f, null, null));
					multipleCoroutine2.Add(() => TransformUtils.LerpScale(bonusButton.transform, Vector3.one * 1.15f, 0.35f, null, null));
					MultipleCoroutine multipleCoroutine3 = multipleCoroutine2;
					shakeButtonRoutine = multipleCoroutine2;
					multipleCoroutine3.StartCoroutines(true);
				});
				return;
			}
			MultipleCoroutine multipleCoroutine = shakeButtonRoutine;
			if (multipleCoroutine != null)
			{
				multipleCoroutine.Dispose();
			}
			bonusButton.interactable = false;
			bonusButton.transform.MoveScale(Vector3.zero, 0.1f, delegate()
			{
				bonusButton.gameObject.SetActive(false);
			});
		}

		private void OnEnable()
		{
			bonusButton.onClick.AddListener(new UnityAction(OnBonusButtonClick));
		}

		private void OnDisable()
		{
			bonusButton.onClick.RemoveListener(new UnityAction(OnBonusButtonClick));
		}

		public void SimulateClick()
		{
			if (bonusButton.interactable && bonusButton.gameObject.activeSelf)
			{
				OnBonusButtonClick();
			}
		}

		private void OnBonusButtonClick()
		{
			if (SoundSettings.IsVibrationOn)
			{
				iOSHapticFeedback.Instance.Trigger(iOSHapticFeedback.iOSFeedbackType.ImpactMedium);
			}
			BonusType bonusType = currentBonus.BonusType;
			if (bonusType != BonusType.SpeedBoost)
			{
				if (bonusType != BonusType.Money)
				{
					throw new ArgumentOutOfRangeException();
				}
				soundSettings.GetAudioSource(SoundType.UIBoostMoneyClick).Play(false);
			}
			else
			{
				soundSettings.GetAudioSource(SoundType.UIBoostSpeedClick).Play(false);
			}
			timer.StopTimer();
			MultipleCoroutine multipleCoroutine = shakeButtonRoutine;
			if (multipleCoroutine != null)
			{
				multipleCoroutine.Dispose();
			}
			bonusButton.transform.MoveScale(Vector3.one, 0.35f, null);
			bonusButton.enabled = false;
			boosterDialog.Show(currentBonus, delegate(bool isRewarded)
			{
				BonusType bonusType2 = currentBonus.BonusType;
				if (bonusType2 != BonusType.SpeedBoost)
				{
					if (bonusType2 != BonusType.Money)
					{
						throw new ArgumentOutOfRangeException();
					}
					(currentBonusAudio = soundSettings.GetAudioSource(SoundType.BoostMoney)).Play(0.75f, 1f, true);
				}
				else
				{
					currentBonusAudio = soundSettings.GetAudioSource(SoundType.BoostSpeed);
				}
				Action<Bonus, bool> bonusStateChange = BonusStateChange;
				bonusStateChange(currentBonus, true);

                timer.StartTimer(isRewarded ? currentBonus.BonusRewardedDuration : currentBonus.BonusDuration, buttonText, Timer.OutputType.All);
				timer.Elapsed += delegate()
				{
					SetButtonState(false);
					currentBonusAudio.Stop(1f, null);
					Action<Bonus, bool> bonusStateChange2 = BonusStateChange;
					bonusStateChange2(currentBonus, false);

                    currentBonus = null;
				};
			});
		}

	}
}
