using System;
using UnityEngine;
using UnityEngine.UI;

namespace Utilities.AlphaAnimation
{
	[RequireComponent(typeof(Text))]
	public class AlphaAnimatedText : Renderable
	{
        private Text txt;

        public override Color Color
		{
			get
			{
				return Txt.color;
			}
			set
			{
				Txt.color = value;
			}
		}

		private Text Txt
		{
			get
			{
				Text result;
				if ((result = txt) == null)
				{
					result = (txt = GetComponent<Text>());
				}
				return result;
			}
		}
	}
}
