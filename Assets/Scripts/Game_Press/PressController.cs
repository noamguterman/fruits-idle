using System;
using System.Collections;
using System.Collections.Generic;
using Game.Audio;
using Settings.UI.Tabs;
using Sirenix.OdinInspector;
using UI.ListItems;
using UI.ListItems.Booster;
using UI.ListItems.Upgrades.Piece.Press;
using UIControllers;
using UnityEngine;
using Utilities;

namespace Game.Press
{
	public class PressController : MonoBehaviour
    {
        [SerializeField]
        private List<PressPart> pressParts;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private BoostersSettings boostersSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private SoundSettings soundSettings;

        private float currentT;

        private float pressPowerMultiplier;

        private float boosterSpeedMultiplier;

        private bool isMovingDown;

        private bool isBonusEnabled;

        private bool beginToCrush;

        private List<Coroutine> currentMovingRoutines;

        private PressVariation pressSettings;

        private BonusView bonusView;

        private BgFlashMaker bgFlashMaker;

        public event Action<float> PressCrush;

		public float SpeedMultiplier { get; set; }

		public Reference<float> Multiplier { get; private set; }

		private void Awake()
		{
			Multiplier = new Reference<float>(1f);
		}

		private void Start()
		{
			currentMovingRoutines = new List<Coroutine>();
			pressParts.ForEach(delegate(PressPart p)
			{
				p.Initialize();
			});
			MovePress(pressSettings.GetCurrentUpgrade(PressUpgradeType.PressSpeedUpDown, false).Value, !isMovingDown, 1f);
		}

		public void Resume()
		{
			MovePress(pressSettings.GetCurrentUpgrade(PressUpgradeType.PressSpeedUpDown, false).Value, !isMovingDown, 1f);
		}

		private void OnEnable()
		{
			pressParts.ForEach(delegate(PressPart p)
			{
				p.PressPartCollidedWithOre += OnPressPartCollidedWithOre;
			});
			boostersSettings.ChangeBoosterTypeMutiplier += OnBoosterMutiplierChanged;
		}

		private void OnDisable()
		{
			pressParts.ForEach(delegate(PressPart p)
			{
				p.PressPartCollidedWithOre -= this.OnPressPartCollidedWithOre;
			});
			currentMovingRoutines.ForEach(delegate(Coroutine r)
			{
				this.StopCoroutineIfRunning(ref r);
			});
			currentMovingRoutines.Clear();
			bonusView.BonusStateChange -= OnBonusStateChange;
			boostersSettings.ChangeBoosterTypeMutiplier -= OnBoosterMutiplierChanged;
		}

		public void Initialize(BonusView bonusView, BgFlashMaker bgFlashMaker, PressVariation pressSettings)
		{
			this.bonusView = bonusView;
			bonusView.BonusStateChange += OnBonusStateChange;
			this.bgFlashMaker = bgFlashMaker;
			this.pressSettings = pressSettings;
			pressPowerMultiplier = 1f;
			boosterSpeedMultiplier = 1f;
		}

		private void PressStopMoving()
		{
			beginToCrush = false;
			AnimationCurve animationCurve = isMovingDown ? pressSettings.PressMoveDownCurve : pressSettings.PressMoveUpCurve;
			AnimationCurve initialCurve = isMovingDown ? pressSettings.PressMoveUpCurve : pressSettings.PressMoveDownCurve;
			currentT = initialCurve.EvaluateTime(1f - animationCurve.Evaluate(currentT));
			MovePress(pressSettings.GetCurrentUpgrade(PressUpgradeType.PressSpeedUpDown, false).Value, !isMovingDown, 1f);
		}

		private void MovePress(float time, bool isMoveDown, float destinationT)
		{
			isMovingDown = isMoveDown;
			currentMovingRoutines.ForEach(delegate(Coroutine r)
			{
				this.StopCoroutineIfRunning(ref r);
			});
			currentMovingRoutines.Clear();
			pressParts.ForEach(delegate(PressPart p)
			{
				if (p.IsMovable)
				{
					currentMovingRoutines.Add(StartCoroutine(LerpPressPosition(p, isMoveDown, time, destinationT)));
				}
			});
		}

		private IEnumerator LerpPressPosition(PressPart pressPart, bool moveDown, float time, float destinationT)
		{
			time = Mathf.Max(time, pressSettings.MinPressMoveTime);
			Vector3 initialPosition;
			Vector3 newPosition;
			AnimationCurve curve;
			if (moveDown)
			{
				initialPosition = pressPart.InitialPosition;
				newPosition = pressPart.TargetPosition;
				curve = pressSettings.PressMoveDownCurve;
			}
			else
			{
				initialPosition = pressPart.TargetPosition;
				newPosition = pressPart.InitialPosition;
				curve = pressSettings.PressMoveUpCurve;
			}
			float maxMultiplier = time / pressSettings.MinPressMoveTime;
			while (Mathf.Abs(currentT - destinationT) > 0.001f)
			{
				currentT = Mathf.Min(destinationT, currentT + Time.deltaTime / (time / Mathf.Min(maxMultiplier, SpeedMultiplier * boosterSpeedMultiplier * Multiplier.Value)));
				Vector3 a = initialPosition;
				Vector3 b = newPosition;
				AnimationCurve animationCurve = curve;
				pressPart.Position = Vector3.Lerp(a, b, (animationCurve != null) ? animationCurve.Evaluate(currentT) : currentT);
				yield return null;
			}
			if (moveDown)
			{
				if (isBonusEnabled)
				{
					AudioSourceListener audioSource = soundSettings.GetAudioSource(SoundType.BoostSpeed);
					if (audioSource != null)
					{
						audioSource.PlayOneShot();
					}
				}
				Action<float> pressCrush = this.PressCrush;
				if (pressCrush != null)
				{
					pressCrush(this.pressSettings.GetCurrentUpgrade(PressUpgradeType.PressStrength, false).Value * this.pressPowerMultiplier);
				}
				if (isBonusEnabled)
				{
					bgFlashMaker.MakeFlash();
				}
			}
			PressStopMoving();
			yield break;
		}

		private void OnBonusStateChange(Bonus b, bool isEnabled)
		{
			if (b.BonusType != BonusType.SpeedBoost)
			{
				return;
			}
			isBonusEnabled = isEnabled;
			pressPowerMultiplier = (isEnabled ? b.PressPowerMultiplier : 1f);
		}

		private void OnBoosterMutiplierChanged(BoosterType boosterType, float multiplier, float time, bool isEnabled)
		{
			if (boosterType != BoosterType.GameSpeed)
			{
				return;
			}
			this.boosterSpeedMultiplier = multiplier;
		}

		private void OnPressPartCollidedWithOre(PressPart pressPart)
		{
			if (!isMovingDown || beginToCrush)
			{
				return;
			}
			beginToCrush = true;
			Vector3 vector = pressPart.Position + (pressPart.TargetPosition - pressPart.Position) * pressSettings.PressPushYOffsetPercent;
			if (vector.y < pressPart.TargetPosition.y + pressSettings.MinUpperPressDistFromLowerPart)
			{
				vector.y = pressPart.TargetPosition.y + pressSettings.MinUpperPressDistFromLowerPart;
			}
			float value = pressSettings.GetCurrentUpgrade(PressUpgradeType.PressSpeedUpDown, false).Value;
			MovePress(value * pressSettings.PressCrushSlowDownSpeedMultiplier, true, pressSettings.PressMoveDownCurve.EvaluateTime(pressPart.GetT(vector)));
		}

	}
}
