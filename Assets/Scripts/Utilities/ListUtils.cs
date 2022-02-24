using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace Utilities
{
	public static class ListUtils
	{
		public static T Random<T>([NotNull] this IList<T> list)
		{
			return list[UnityEngine.Random.Range(0, list.Count)];
		}

		public static void GenerateListItemsFromEnum<T>([NotNull] this IList<T> list, Type enumType)
		{
			if (!enumType.IsEnum)
			{
				return;
			}
			Type typeFromHandle = typeof(T);
			FieldInfo enumField = typeFromHandle.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).FirstOrDefault((FieldInfo e) => e.FieldType == enumType);
			PropertyInfo enumProp = null;
			if (enumField == null)
			{
				enumProp = typeFromHandle.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty).FirstOrDefault((PropertyInfo e) => e.PropertyType == enumType);
				if (enumProp == null)
				{
					return;
				}
			}
			int[] array = (int[])Enum.GetValues(enumType);
			if (array.Length == list.Count)
			{
				return;
			}
			string[] names = Enum.GetNames(enumType);
			Dictionary<int, string> enumDict = new Dictionary<int, string>();
			for (int k = 0; k < array.Length; k++)
			{
				enumDict.Add(array[k], names[k]);
			}
			List<T> source = new List<T>(list);
			list.Clear();
			int[] array2 = array;
			for (int j = 0; j < array2.Length; j++)
			{
				int i = array2[j];
				T t = source.FirstOrDefault(delegate(T s)
				{
					string a;
					if (!(enumField != null))
					{
						a = ((enumProp != null) ? enumProp.GetValue(s).ToString() : null);
					}
					else
					{
						a = enumField.GetValue(s).ToString();
					}
					return a == enumDict[i];
				});
				T t2 = (t != null && !t.Equals(default(T))) ? t : ((T)((object)Activator.CreateInstance(typeFromHandle, new object[0])));
				object value = Enum.ToObject(enumType, i);
				if (enumField != null)
				{
					enumField.SetValue(t2, value);
				}
				else
				{
					enumProp.SetValue(t2, value);
				}
				list.Add(t2);
			}
		}
	}
}
