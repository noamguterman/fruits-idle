using System;
using UnityEngine;

public class iOSHapticFeedbackExample : MonoBehaviour
{
	private void OnGUI()
	{
		for (int i = 0; i < 7; i++)
		{
			if (GUI.Button(new Rect(0f, (float)(i * 60), 300f, 50f), "Trigger " + (iOSHapticFeedback.iOSFeedbackType)i))
			{
				iOSHapticFeedback.Instance.Trigger((iOSHapticFeedback.iOSFeedbackType)i);
			}
		}
	}
}
