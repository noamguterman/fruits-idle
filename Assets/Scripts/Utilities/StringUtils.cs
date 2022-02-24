using System;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Utilities
{
	public static class StringUtils
	{
		public static string AddSpaces([NotNull] this string inputString)
		{
			return Regex.Replace(inputString, "((?<=\\p{Ll})\\p{Lu})|((?!\\A)\\p{Lu}(?>\\p{Ll}))", " $0");
		}
	}
}
