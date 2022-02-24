using System;

namespace Utilities
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class GenerateListFromEnumAttribute : Attribute
	{
		public Type EnumType { get; set; }
	}
}
