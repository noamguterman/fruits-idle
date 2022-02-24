using System;
using UnityEngine;

namespace Utilities
{
	[CreateAssetMenu(menuName = "Events/New Event<int>")]
	public class ScriptableEventInt : ScriptableObject
	{
		public event Action<int> Event;

		public void Invoke(int value)
		{
			Action<int> @event = this.Event;
			if (@event == null)
			{
				return;
			}
			@event(value);
		}
	}
}
