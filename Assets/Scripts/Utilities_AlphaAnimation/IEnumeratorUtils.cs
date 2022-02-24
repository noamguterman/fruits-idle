using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Utilities.AlphaAnimation
{
	public static class IEnumeratorUtils
	{
		public static bool MoveNextCycled<T>([NotNull] this IEnumerator<T> ieEnumerator)
		{
			if (!ieEnumerator.MoveNext())
			{
				ieEnumerator.Reset();
				return ieEnumerator.MoveNext();
			}
			return true;
		}
	}
}
