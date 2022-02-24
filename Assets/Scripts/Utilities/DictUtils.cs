using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities
{
	public static class DictUtils
	{
		public static KeyValuePair<TKey, TValue> Random<TKey, TValue>(this IDictionary<TKey, TValue> dict)
		{
			int num = UnityEngine.Random.Range(0, dict.Count);
			int num2 = 0;
			foreach (KeyValuePair<TKey, TValue> result in dict)
			{
				if (num == num2)
				{
					return result;
				}
				num2++;
			}
			return dict.First<KeyValuePair<TKey, TValue>>();
		}
	}
}
