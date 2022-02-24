using System;
using UnityEngine;

public class DD_CalcRectTransformHelper
{
	public static Vector2 CalcAnchorPointPosition(Vector2 anchorMin, Vector2 anchorMax, Vector2 parentSize, Vector2 pivot)
	{
		Vector2 vector = new Vector2(parentSize.x * anchorMin.x, parentSize.y * anchorMin.y);
		Vector2 vector2 = new Vector2(parentSize.x * anchorMax.x - vector.x, parentSize.y * anchorMax.y - vector.y);
		return vector + new Vector2(vector2.x * pivot.x, vector2.y * pivot.y);
	}

	public static Vector2 CalcAnchorPosition(Rect rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 parentSize, Vector2 pivot)
	{
		Vector2 b = DD_CalcRectTransformHelper.CalcAnchorPointPosition(anchorMin, anchorMax, parentSize, pivot);
		return new Vector2(rect.x + rect.width * pivot.x, rect.y + rect.height * pivot.y) - b;
	}

	public static Vector2 CalcOffsetMin(Rect rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 parentSize)
	{
		Vector2 b = new Vector2(parentSize.x * anchorMin.x, parentSize.y * anchorMin.y);
		return new Vector2(rect.x, rect.y) - b;
	}

	public static Vector2 CalcOffsetMax(Rect rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 parentSize)
	{
		Vector2 b = new Vector2(parentSize.x * anchorMax.x, parentSize.y * anchorMax.y);
		return new Vector2(rect.x + rect.width, rect.y + rect.height) - b;
	}

	public static Vector2 CalcSizeDelta(Rect rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 parentSize)
	{
		return DD_CalcRectTransformHelper.CalcOffsetMax(rect, anchorMin, anchorMax, parentSize) - DD_CalcRectTransformHelper.CalcOffsetMin(rect, anchorMin, anchorMax, parentSize);
	}

	public static Vector2 CalcRectSize(Vector2 sizeDelta, Vector2 anchorMin, Vector2 anchorMax, Vector2 parentSize)
	{
		Vector2 b = new Vector2(parentSize.x * anchorMin.x, parentSize.y * anchorMin.y);
		return new Vector2(parentSize.x * anchorMax.x, parentSize.y * anchorMax.y) - b + sizeDelta;
	}

	public static Rect CalcLocalRect(Vector2 anchorMin, Vector2 anchorMax, Vector2 parentSize, Vector2 pivot, Vector2 anchorPosition, Rect rectInRT)
	{
		return new Rect(DD_CalcRectTransformHelper.CalcAnchorPointPosition(anchorMin, anchorMax, parentSize, pivot) + anchorPosition + rectInRT.position, rectInRT.size);
	}
}
