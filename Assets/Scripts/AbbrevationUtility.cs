using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public static class AbbrevationUtility
{
	public static string AbbreviateNumber(this float number, bool capital = true)
	{
		foreach (KeyValuePair<float, string> keyValuePair in AbbrevationUtility.Abbrevations.Reverse<KeyValuePair<float, string>>())
		{
			if (Mathf.Abs(number) >= keyValuePair.Key)
			{
				string text = (number / keyValuePair.Key).ToString(CultureInfo.InvariantCulture);
				int b = text.Contains(".") ? ((text.IndexOf(".", StringComparison.Ordinal) < 3) ? 4 : 5) : (text.Contains(",") ? ((text.IndexOf(",", StringComparison.Ordinal) < 3) ? 4 : 5) : 4);
				text = text.Substring(0, Mathf.Min(text.Length, b)) + (capital ? keyValuePair.Value : keyValuePair.Value.ToLower());
				return text.Replace(',', '.');
			}
		}
		if (number.ToString().Contains(','))
		{
			int num = number.ToString().IndexOf(',') + 1;
			return number.ToString(CultureInfo.InvariantCulture).Substring(0, num + 1);
		}
		if (number.ToString().Contains('.'))
		{
			int num2 = number.ToString().IndexOf('.') + 1;
			return number.ToString(CultureInfo.InvariantCulture).Substring(0, num2 + 1);
		}
		return number.ToString(CultureInfo.InvariantCulture);
	}

	private static readonly SortedDictionary<float, string> Abbrevations = new SortedDictionary<float, string>
	{
		{
			1000f,
			"K"
		},
		{
			1000000f,
			"M"
		},
		{
			1E+09f,
			"B"
		},
		{
			1E+12f,
			"T"
		},
		{
			1E+15f,
			"Q"
		},
		{
			1E+18f,
			"A"
		},
		{
			1E+21f,
			"C"
		},
		{
			1E+24f,
			"D"
		},
		{
			1E+27f,
			"E"
		},
        {
            1E+30f,
            "F"
        },
        {
            1E+33f,
            "G"
        },
        {
            1E+36f,
            "H"
        }
    };
}
