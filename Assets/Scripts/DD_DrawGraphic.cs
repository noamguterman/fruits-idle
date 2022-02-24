using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DD_DrawGraphic : MaskableGraphic
{
	private int IsPointHorizontalInRect(Vector2 p, Rect rect)
	{
		if (p.x < rect.x)
		{
			return -1;
		}
		if (p.x > rect.x + rect.width)
		{
			return 1;
		}
		return 0;
	}

	private int IsPointVerticalityInRect(Vector2 p, Rect rect)
	{
		if (p.y < rect.y)
		{
			return -1;
		}
		if (p.y > rect.y + rect.height)
		{
			return 1;
		}
		return 0;
	}

	private Vector2? CalcHorizontalCutPoint(Vector2 p1, Vector2 p2, float y)
	{
		if (p2.y == p1.y)
		{
			return new Vector2?(new Vector2(p2.x, p2.y));
		}
		float num = (y - p1.y) / (p2.y - p1.y) * (p2.x - p1.x);
		return new Vector2?(new Vector2(p1.x + num, y));
	}

	private int AddHorizontalCutPoints(List<Vector2> points, int sn, float y)
	{
		Vector2? vector = null;
		Vector2? vector2 = null;
		int num = 0;
		if (sn > 0)
		{
			Vector2? vector3;
			vector = (vector3 = this.CalcHorizontalCutPoint(points[sn], points[sn - 1], y));
			if (vector3 != null)
			{
				points.Insert(sn, vector.Value);
				sn++;
				num++;
			}
		}
		if (sn < points.Count - 1)
		{
			Vector2? vector3;
			vector2 = (vector3 = this.CalcHorizontalCutPoint(points[sn], points[sn + 1], y));
			if (vector3 != null)
			{
				points.Insert(sn + 1, vector2.Value);
				num++;
			}
		}
		return num;
	}

	private void HorizontalCut(List<Vector2> points, Rect range)
	{
		int i = 0;
		int num = 0;
		while (i < points.Count)
		{
			int num2 = this.IsPointVerticalityInRect(points[i], range);
			if (num2 > 0)
			{
				num = this.AddHorizontalCutPoints(points, i, range.y + range.height);
			}
			else if (num2 < 0)
			{
				num = this.AddHorizontalCutPoints(points, i, range.y);
			}
			num++;
			i += num;
			num = 0;
		}
	}

	protected bool IsPointInRect(Vector2 p, Rect rect)
	{
		return this.IsPointHorizontalInRect(p, rect) == 0 && this.IsPointVerticalityInRect(p, rect) == 0;
	}

	protected void DrawRectang(VertexHelper vh, Vector2 point1st, Vector2 point2nd, Vector2 point3rd, Vector2 point4th, Color color)
	{
		UIVertex[] array = new UIVertex[4];
		array[0].position = point1st;
		array[0].color = color;
		array[0].uv0 = Vector2.zero;
		array[1].position = point2nd;
		array[1].color = color;
		array[1].uv0 = Vector2.zero;
		array[2].position = point3rd;
		array[2].color = color;
		array[2].uv0 = Vector2.zero;
		array[3].position = point4th;
		array[3].color = color;
		array[3].uv0 = Vector2.zero;
		vh.AddUIVertexQuad(array);
	}

	protected void DrawPoint(VertexHelper vh, Vector2 point, Color color, float thickness, float scaleX = 1f, float scaleY = 1f)
	{
		Vector2 point1st = new Vector2((point.x - thickness / 2f) * scaleX, (point.y - thickness / 2f) * scaleY);
		Vector2 point2nd = new Vector2((point.x - thickness / 2f) * scaleX, (point.y + thickness / 2f) * scaleY);
		Vector2 point3rd = new Vector2((point.x + thickness / 2f) * scaleX, (point.y + thickness / 2f) * scaleY);
		Vector2 point4th = new Vector2((point.x + thickness / 2f) * scaleX, (point.y - thickness / 2f) * scaleY);
		this.DrawRectang(vh, point1st, point2nd, point3rd, point4th, color);
	}

	protected void DrawHorizontalSegmet(VertexHelper vh, Vector2 startPoint, Vector2 endPoint, Color color, float thickness, float scaleX = 1f, float scaleY = 1f)
	{
		Vector2 point1st = new Vector2(startPoint.x * scaleX, startPoint.y * scaleY - thickness / 2f);
		Vector2 point2nd = new Vector2(startPoint.x * scaleX, startPoint.y * scaleY + thickness / 2f);
		Vector2 point3rd = new Vector2(endPoint.x * scaleX, endPoint.y * scaleY + thickness / 2f);
		Vector2 point4th = new Vector2(endPoint.x * scaleX, endPoint.y * scaleY - thickness / 2f);
		this.DrawRectang(vh, point1st, point2nd, point3rd, point4th, color);
	}

	protected void DrawVerticalitySegmet(VertexHelper vh, Vector2 startPoint, Vector2 endPoint, Color color, float thickness, float scaleX = 1f, float scaleY = 1f)
	{
		Vector2 point1st = new Vector2(startPoint.x * scaleX - thickness / 2f, startPoint.y * scaleY);
		Vector2 point2nd = new Vector2(endPoint.x * scaleX - thickness / 2f, endPoint.y * scaleY);
		Vector2 point3rd = new Vector2(endPoint.x * scaleX + thickness / 2f, endPoint.y * scaleY);
		Vector2 point4th = new Vector2(startPoint.x * scaleX + thickness / 2f, startPoint.y * scaleY);
		this.DrawRectang(vh, point1st, point2nd, point3rd, point4th, color);
	}

	protected void DrawHorizontalLine(VertexHelper vh, List<Vector2> points, Color color, float thickness)
	{
		if (points.Count < 2)
		{
			return;
		}
		for (int i = 0; i < points.Count - 1; i++)
		{
			this.DrawHorizontalSegmet(vh, points[i], points[i + 1], color, thickness, 1f, 1f);
		}
	}

	protected void DrawHorizontalLine(VertexHelper vh, List<Vector2> points, Color color, float thickness, Rect range)
	{
		if (points.Count < 2)
		{
			return;
		}
		this.HorizontalCut(points, range);
		int i = 0;
		while (i < points.Count - 1)
		{
			if (!this.IsPointInRect(points[i], range))
			{
				points.RemoveAt(i);
			}
			else if (!this.IsPointInRect(points[i + 1], range))
			{
				points.RemoveAt(i + 1);
				i++;
			}
			else
			{
				this.DrawHorizontalSegmet(vh, points[i], points[i + 1], color, thickness, 1f, 1f);
				i++;
			}
		}
	}

	protected void DrawTriangle(VertexHelper vh, Vector2 points, Color color, float thickness, float rotate, float scaleX = 1f, float scaleY = 1f)
	{
		float num = thickness / 3f * 2f;
		Vector2 point2nd = new Vector2((points.x + Mathf.Sin(0.0174532924f * rotate) * num) * scaleX, (points.y + Mathf.Cos(0.0174532924f * rotate) * num) * scaleY);
		Vector2 point3rd = new Vector2((points.x + Mathf.Sin(0.0174532924f * (rotate + 120f)) * num) * scaleX, (points.y + Mathf.Cos(0.0174532924f * (rotate + 120f)) * num) * scaleY);
		Vector2 vector = new Vector2((points.x + Mathf.Sin(0.0174532924f * (rotate + 240f)) * num) * scaleX, (points.y + Mathf.Cos(0.0174532924f * (rotate + 240f)) * num) * scaleY);
		this.DrawRectang(vh, vector, point2nd, point3rd, vector, color);
	}

	protected void DrawRectFrame(VertexHelper vh, Vector2 point1st, Vector2 point2nd, Vector2 point3rd, Vector2 point4th, Color color, float thickness)
	{
		this.DrawVerticalitySegmet(vh, point1st, point2nd, color, thickness, 1f, 1f);
		this.DrawHorizontalSegmet(vh, point2nd, point3rd, color, thickness, 1f, 1f);
		this.DrawVerticalitySegmet(vh, point3rd, point4th, color, thickness, 1f, 1f);
		this.DrawHorizontalSegmet(vh, point4th, point1st, color, thickness, 1f, 1f);
	}

	public static float GetTriangleCentreDis(float thickness)
	{
		return thickness / 3f;
	}
}
