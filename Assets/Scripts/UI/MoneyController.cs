using System;
using System.Globalization;
using Game.Audio;
using Sirenix.OdinInspector;
using UI.ListItems;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI
{
	public class MoneyController : MonoBehaviour
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private Image moneyImage;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private Text moneyText;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private SoundSettings soundSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private MoneyType moneyType;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private bool useMoneyAnimation;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [ShowIf("useMoneyAnimation", true)]
        private AnimationCurve animationCurve;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [ShowIf("useMoneyAnimation", true)]
        private AnimationCurve animationMoveCurve;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [ShowIf("useMoneyAnimation", true)]
        private float animationTime;

        private Canvas canvas;

        private float money;

        private void Start()
		{
            canvas = GetComponentInParent<Canvas>();
			Money = GameData.GetMoney(moneyType);
		}

		private void OnEnable()
		{
			GameData.MoneyChangeWithAnimation += OnMoneyChangeWithAnimation;
			GameData.MoneyChange += OnMoneyChange;
		}

		private void OnDisable()
		{
			GameData.MoneyChangeWithAnimation -= OnMoneyChangeWithAnimation;
			GameData.MoneyChange -= OnMoneyChange;
		}

		private float Money
		{
			set
			{
				if (value > money)
				{
					AudioSourceListener audioSource = soundSettings.GetAudioSource(SoundType.MoneyIncrease);
					if (audioSource != null)
					{
						audioSource.Play(false);
					}
				}
				money = value;
				string text = (value > 1000f) ? value.AbbreviateNumber(true) : Math.Round((double)value, 0).ToString(CultureInfo.InvariantCulture);
				moneyText.text = text;
			}
		}

		private void OnMoneyChangeWithAnimation(Vector3 position, bool worldPos, float cost, float initialScale, MoneyType moneyType)
		{
			if (this.moneyType != moneyType)
			{
				return;
			}
			if (!useMoneyAnimation || canvas.IsNull())
			{
				GameData.IncreaseMoney(cost, moneyType, true);
				return;
			}
			Vector3 v = worldPos ? new Vector3(position.PositionInCanvas(canvas).x, position.PositionInCanvas(canvas).y, 0) : position;
			Image moneyImageMoving = new GameObject("Money").AddComponent<Image>();
			moneyImageMoving.sprite = moneyImage.sprite;
			moneyImageMoving.transform.SetParent(canvas.transform, false);
			moneyImageMoving.SetNativeSize();
			Transform moneyRect = moneyImageMoving.transform;
			moneyRect.position = v;
			moneyRect.localScale = Vector3.one * initialScale;
			moneyRect.MoveScale(Vector3.one * 2f, animationTime * 0.25f, delegate()
			{
				moneyRect.MoveScale(Vector3.one, animationTime * 0.75f, null);
			});
			moneyRect.MovePosition(moneyImage.transform.position, animationTime, animationMoveCurve, delegate()
			{
				GameData.IncreaseMoney(cost, moneyType, true);
				Destroy(moneyImageMoving.gameObject);
			});
		}

		private void OnMoneyChange(float newMoneyAmount, MoneyType moneyType)
		{
			if (this.moneyType == moneyType)
			{
				Money = newMoneyAmount;
			}
		}

	}
}
