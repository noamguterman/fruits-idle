using System;
using UnityEngine;

namespace Crystal
{
	public class SafeArea : MonoBehaviour
    {
        [SerializeField]
        private bool conformX = true;

        [SerializeField]
        private bool conformY = true;

        private RectTransform panel;

        private Rect lastSafeArea = new Rect(0f, 0f, 0f, 0f);

        public static bool UseDebugLog;

        public enum SimDevice
        {
            None,
            IPhoneX,
            IPhoneXsMax
        }

        private void Awake()
		{
			panel = GetComponent<RectTransform>();
			if (panel == null)
			{
				Debug.LogError("Cannot apply safe area - no RectTransform found on " + base.name);
				Destroy(gameObject);
			}
			Refresh();
		}

		private void Update()
		{
			Refresh();
		}

		private void Refresh()
		{
			Rect safeArea = GetSafeArea();
			if (safeArea != lastSafeArea)
			{
				ApplySafeArea(safeArea);
			}
		}

		private Rect GetSafeArea()
		{
			Rect safeArea = Screen.safeArea;
			if (Application.isEditor && SafeArea.Sim != SafeArea.SimDevice.None)
			{
				Rect rect = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
				switch (SafeArea.Sim)
				{
				case SafeArea.SimDevice.None:
					break;
				case SafeArea.SimDevice.IPhoneX:
					rect = ((Screen.height > Screen.width) ? nsaIPhoneX[0] : nsaIPhoneX[1]);
					break;
				case SafeArea.SimDevice.IPhoneXsMax:
					rect = ((Screen.height > Screen.width) ? nsaIPhoneXsMax[0] : nsaIPhoneXsMax[1]);
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
				safeArea = new Rect((float)Screen.width * rect.x, (float)Screen.height * rect.y, (float)Screen.width * rect.width, (float)Screen.height * rect.height);
			}
			return safeArea;
		}

		private void ApplySafeArea(Rect r)
		{
			lastSafeArea = r;
			if (!conformX)
			{
				r.x = 0f;
				r.width = (float)Screen.width;
			}
			if (!conformY)
			{
				r.y = 0f;
				r.height = (float)Screen.height;
			}
			Vector2 position = r.position;
			Vector2 anchorMax = r.position + r.size;
			position.x /= (float)Screen.width;
			position.y /= (float)Screen.height;
			anchorMax.x /= (float)Screen.width;
			anchorMax.y /= (float)Screen.height;
			panel.anchorMin = position;
			panel.anchorMax = anchorMax;
			if (SafeArea.UseDebugLog)
			{
				Debug.LogFormat("New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}", new object[]
				{
					name,
					r.x,
					r.y,
					r.width,
					r.height,
					Screen.width,
					Screen.height
				});
			}
		}

		public static SafeArea.SimDevice Sim;

		private readonly Rect[] nsaIPhoneX = new Rect[]
		{
			new Rect(0f, 0.04187192f, 1f, 0.9039409f),
			new Rect(0.0541871935f, 0.056f, 0.891625643f, 0.944f)
		};

		private readonly Rect[] nsaIPhoneXsMax = new Rect[]
		{
			new Rect(0f, 0.03794643f, 1f, 0.9129464f),
			new Rect(0.04910714f, 0.0507246368f, 0.901785731f, 0.9492754f)
		};

	}
}
