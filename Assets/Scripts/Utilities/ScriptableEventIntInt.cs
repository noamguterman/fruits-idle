using System;
using UnityEngine;

namespace Utilities
{
	[CreateAssetMenu(menuName = "Events/New Event<int,int>")]
	public class ScriptableEventIntInt : ScriptableObject
	{
		public event Action<int, int> Event;

		public void Invoke(int arg1, int arg2)
		{
			Action<int, int> @event = this.Event;
			if (@event == null)
			{
				return;
			}
			@event(arg1, arg2);
		}
	}
}
