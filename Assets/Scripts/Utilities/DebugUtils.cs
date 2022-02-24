using System;
using UnityEngine;

namespace Utilities
{
	public static class DebugUtils
	{
		public static void Log(object message, bool withDate = true)
		{
			if (!withDate)
			{
				UnityEngine.Debug.Log(message);
				return;
			}
			string text = DateTime.Now.ToLongTimeString() + ":" + DateTime.Now.Millisecond;
			int num = DebugUtils.MaxLogMsgLen - message.ToString().Length - text.Length;
			for (int i = 0; i < num; i++)
			{
				message += "-";
			}
			UnityEngine.Debug.Log(message + text);
		}

		private static readonly int MaxLogMsgLen = 100;
	}
}
