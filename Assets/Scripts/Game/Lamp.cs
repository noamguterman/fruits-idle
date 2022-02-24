using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;
using Utilities.AlphaAnimation;

namespace Game
{
	public class Lamp : MonoBehaviour
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private SpriteRenderer lampOn;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private SpriteRenderer lampOff;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private float changeLampStateTime = 0.1f;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private float lampStateTime = 0.5f;

        private Coroutine lampFlickerRoutine;

        public bool IsFlickering
		{
			get
			{
				return lampFlickerRoutine != null;
			}
		}

		private void Awake()
		{
			lampOn.AnimateAlpha(0f, null);
			lampOn.gameObject.SetActive(false);
		}

		public void SetActive(bool isEnabled, float time = 0f)
		{
			if (isEnabled)
			{
				gameObject.SetActive(true);
				this.AnimateAlpha(0f, null);
				lampOff.AnimateAlpha(1f, time, false, 0f, null);
				return;
			}
			this.AnimateAlpha(0f, time, delegate(Lamp t)
			{
				Stop();
				gameObject.SetActive(false);
			});
		}

		public void StartFlashing()
		{
			SetActive(true, 0f);
			lampOn.gameObject.SetActive(true);
			lampOn.AnimateAlpha(0f, null);
			bool isOn = false;
			lampFlickerRoutine = StartCoroutine(CoroutineUtils.LoopExecute(delegate()
			{
				lampOn.AnimateAlpha((float)(isOn ? 0 : 1), changeLampStateTime, false, 0f, null);
				isOn = !isOn;
			}, new WaitForSeconds(changeLampStateTime + lampStateTime)));
		}

		public void Stop()
		{
			if (lampFlickerRoutine != null)
			{
				StopCoroutine(lampFlickerRoutine);
			}
			lampFlickerRoutine = null;
			lampOn.gameObject.SetActive(false);
		}

	}
}
