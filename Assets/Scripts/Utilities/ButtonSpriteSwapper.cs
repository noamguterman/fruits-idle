using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Utilities
{
	[RequireComponent(typeof(Toggle))]
	public class ButtonSpriteSwapper : MonoBehaviour
	{
		private void Awake()
		{
			this.toggle = base.GetComponent<Toggle>();
			this.toggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnValueChanged));
		}

		private void OnValueChanged(bool value)
		{
			this.toggle.image.sprite = (value ? this.buttonOn : this.buttonOff);
		}

		[SerializeField]
		private Sprite buttonOff;

		[SerializeField]
		private Sprite buttonOn;

		private Toggle toggle;
	}
}
