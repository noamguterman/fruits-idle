using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DD_Lines : DD_DrawGraphic
{
	public float Thickness
	{
		get
		{
			return this.m_Thickness;
		}
		set
		{
			this.m_Thickness = value;
		}
	}

	public bool IsShow
	{
		get
		{
			return this.m_IsShow;
		}
		set
		{
			if (value != this.m_IsShow)
			{
				this.UpdateGeometry();
			}
			this.m_IsShow = value;
		}
	}

	protected override void Awake()
	{
		this.m_DataDiagram = base.GetComponentInParent<DD_DataDiagram>();
		if (null == this.m_DataDiagram)
		{
			UnityEngine.Debug.Log(this + "null == m_DataDiagram");
		}
		this.m_Coordinate = base.GetComponentInParent<DD_CoordinateAxis>();
		if (null == this.m_Coordinate)
		{
			UnityEngine.Debug.Log(this + "null == m_Coordinate");
		}
		GameObject gameObject = base.gameObject.transform.parent.gameObject;
		if (null == gameObject)
		{
			UnityEngine.Debug.Log(this + "null == parent");
		}
		RectTransform component = gameObject.GetComponent<RectTransform>();
		RectTransform component2 = base.gameObject.GetComponent<RectTransform>();
		if (null == component2 || null == component)
		{
			UnityEngine.Debug.Log(this + "null == localrt || parentrt");
		}
		component2.anchorMin = Vector2.zero;
		component2.anchorMax = new Vector2(1f, 1f);
		component2.pivot = Vector2.zero;
		component2.anchoredPosition = Vector2.zero;
		component2.sizeDelta = Vector2.zero;
		if (null != this.m_Coordinate)
		{
			this.m_Coordinate.CoordinateRectChangeEvent += this.OnCoordinateRectChange;
			this.m_Coordinate.CoordinateScaleChangeEvent += this.OnCoordinateScaleChange;
			this.m_Coordinate.CoordinateeZeroPointChangeEvent += this.OnCoordinateZeroPointChange;
		}
	}

	private void Update()
	{
		if (this.m_CurIsShow == this.m_IsShow)
		{
			return;
		}
		this.m_CurIsShow = this.m_IsShow;
		this.UpdateGeometry();
	}

	private float ScaleX(float x)
	{
		if (null == this.m_Coordinate)
		{
			UnityEngine.Debug.Log(this + "null == m_Coordinate");
			return x;
		}
		return x / this.m_Coordinate.coordinateAxisViewRangeInPixel.width;
	}

	private float ScaleY(float y)
	{
		if (null == this.m_Coordinate)
		{
			UnityEngine.Debug.Log(this + "null == m_Coordinate");
			return y;
		}
		return y / this.m_Coordinate.coordinateAxisViewRangeInPixel.height;
	}

	private int GetStartPointSN(List<Vector2> points, float startX)
	{
		int num = 0;
		float num2 = 0f;
		foreach (Vector2 vector in points)
		{
			if (num2 > startX)
			{
				return points.IndexOf(vector);
			}
			num2 += vector.x;
			num++;
		}
		return num;
	}

	private void OnCoordinateRectChange(object sender, DD_CoordinateRectChangeEventArgs e)
	{
		this.UpdateGeometry();
	}

	private void OnCoordinateScaleChange(object sender, DD_CoordinateScaleChangeEventArgs e)
	{
		this.UpdateGeometry();
	}

	private void OnCoordinateZeroPointChange(object sender, DD_CoordinateZeroPointChangeEventArgs e)
	{
		this.CurStartPointSN = this.GetStartPointSN(this.PointList, this.m_Coordinate.coordinateAxisViewRangeInPixel.x);
		this.UpdateGeometry();
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
		if (!this.m_IsShow)
		{
			return;
		}
		float num = 0f;
		List<Vector2> list = new List<Vector2>();
		for (int i = this.CurStartPointSN; i < this.PointList.Count; i++)
		{
			list.Add(new Vector2(this.ScaleX(num), this.ScaleY(this.PointList[i].y - this.m_Coordinate.coordinateAxisViewRangeInPixel.y)));
			num += this.PointList[i].x;
			if (num >= this.m_Coordinate.coordinateAxisViewRangeInPixel.width * base.rectTransform.rect.width)
			{
				break;
			}
		}
		base.DrawHorizontalLine(vh, list, this.color, this.m_Thickness, new Rect(0f, 0f, base.rectTransform.rect.width, base.rectTransform.rect.height));
	}

	public void AddPoint(Vector2 point)
	{
		this.PointList.Insert(0, new Vector2(point.x, point.y));
		while (this.PointList.Count > this.m_DataDiagram.m_MaxPointNum)
		{
			this.PointList.RemoveAt(this.PointList.Count - 1);
			MonoBehaviour.print(this.PointList.Count);
		}
		this.UpdateGeometry();
	}

	public bool Clear()
	{
		if (null == this.m_Coordinate)
		{
			UnityEngine.Debug.LogWarning(this + "null == m_Coordinate");
		}
		try
		{
			this.m_Coordinate.CoordinateRectChangeEvent -= this.OnCoordinateRectChange;
			this.m_Coordinate.CoordinateScaleChangeEvent -= this.OnCoordinateScaleChange;
			this.m_Coordinate.CoordinateeZeroPointChangeEvent -= this.OnCoordinateZeroPointChange;
			this.PointList.Clear();
		}
		catch (NullReferenceException arg)
		{
			MonoBehaviour.print(this + " : " + arg);
			return false;
		}
		return true;
	}

	private float m_Thickness = 5f;

	private bool m_IsShow = true;

	private bool m_CurIsShow = true;

	private List<Vector2> PointList = new List<Vector2>();

	private int CurStartPointSN;

	private DD_DataDiagram m_DataDiagram;

	private DD_CoordinateAxis m_Coordinate;

	[NonSerialized]
	public string lineName = "";
}
