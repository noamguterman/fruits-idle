using System;
using UnityEngine;

namespace Utilities
{
	[CreateAssetMenu(menuName = "Events/New Event")]
	public class ScriptableEvent : ScriptableObject
	{
		public event Action Event;

		public void Invoke()
		{
			Action @event = this.Event;
			if (@event == null)
			{
				return;
			}
			@event();
		}
	}
}
