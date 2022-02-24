using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Utilities
{
	public static class CanvasUtils
	{
		public static Vector2 CanvasSize([NotNull] this Canvas c)
		{
			Transform transform = c.transform;
			Rect rect = ((RectTransform)transform).rect;
			Vector3 localScale = transform.localScale;
			return new Vector2(rect.width * localScale.x, rect.height * localScale.y);
		}

		public static Vector2 PositionInCanvas([NotNull] this Transform transform, Canvas canvas)
		{
			return transform.position.PositionInCanvas(canvas);
		}

		public static Vector2 PositionInCanvas(this Vector3 position, Canvas canvas)
		{
			if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
			{
				throw new Exception("Canvas must be Overlay.");
			}
			return canvas.CanvasSize() * CameraUtils.MainCamera.WorldToViewportPoint(position);
		}
	}
}
