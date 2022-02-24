using System;
using Settings.UI.Tabs;
using Sirenix.OdinInspector;
using UI;
using UIControllers;
using UnityEngine;
using Utilities;

namespace Game.Press
{
	public class PressManager : MonoBehaviour, ISuspendable
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private Transform world;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private BgFlashMaker bgFlashMaker;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private BonusView bonusView;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private Boostmeter boostmeter;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private PressVariationsSettings pressVariationsSettings;

        private PressController currentPress;

        private Coroutine changeSpeedMultiplierRoutine;

        public event Action<float> PressCrush;

		private void Awake()
		{
			this.SetCurrentPress();
		}

		private void OnEnable()
		{
			this.boostmeter.MultiplierChange += this.OnMultiplierChange;
		}

		private void OnDisable()
		{
			this.boostmeter.MultiplierChange -= this.OnMultiplierChange;
		}

		private void SetCurrentPress()
		{
			if (this.currentPress)
			{
				this.currentPress.PressCrush -= this.OnPressCrush;
				this.currentPress.Push<PressController>();
			}
			PressVariation currentPressVariation = this.pressVariationsSettings.CurrentPressVariation;
			PressController press = currentPressVariation.Press;
			this.currentPress = ((press != null) ? press.PullOrCreate<PressController>() : null);
			if (currentPressVariation.Press == null || this.currentPress == null)
			{
				return;
			}
			Transform transform = this.currentPress.transform;
			transform.SetParent(this.world);
			transform.localPosition = currentPressVariation.Press.transform.position;
			this.currentPress.PressCrush += this.OnPressCrush;
			this.currentPress.Initialize(this.bonusView, this.bgFlashMaker, currentPressVariation);
		}

		private void OnPressCrush(float force)
		{
			Action<float> pressCrush = this.PressCrush;
			if (pressCrush == null)
			{
				return;
			}
			pressCrush(force);
		}

		public void OnMultiplierChange(float multiplier)
		{
			if (this.currentPress == null)
			{
				return;
			}
			this.currentPress.SpeedMultiplier = multiplier;
		}

		public void Suspend()
		{
            //Debug.Log("Suspended");
			if (this.changeSpeedMultiplierRoutine != null)
			{
				base.StopCoroutine(this.changeSpeedMultiplierRoutine);
			}
			this.changeSpeedMultiplierRoutine = base.StartCoroutine(CoroutineUtils.LerpFloat(this.currentPress.Multiplier, 0f, 0.5f));
		}

		public void Resume()
		{
			if (this.changeSpeedMultiplierRoutine != null)
			{
				base.StopCoroutine(this.changeSpeedMultiplierRoutine);
			}
			base.StartCoroutine(CoroutineUtils.LerpFloat(this.currentPress.Multiplier, 1f, 0.3f));
		}

	}
}
