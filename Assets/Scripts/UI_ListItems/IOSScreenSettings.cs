using System;
using UnityEngine;

namespace UI.ListItems
{
	[CreateAssetMenu(menuName = "Settings/IOS Screen Settings", fileName = "IOS Screen Settings")]
	public class IOSScreenSettings : ScriptableObject
	{
		public Vector2 GetResolution()
		{
			return new Vector2((float)Screen.width, (float)Screen.height);
		}
	}
}
