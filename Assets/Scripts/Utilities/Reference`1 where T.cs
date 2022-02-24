using System;

namespace Utilities
{
	public class Reference<T> where T : struct
	{
		public T Value { get; set; }

		public Reference(T reference = default(T))
		{
			this.Value = reference;
		}

		public override string ToString()
		{
			T value = this.Value;
			return value.ToString();
		}
	}
}
