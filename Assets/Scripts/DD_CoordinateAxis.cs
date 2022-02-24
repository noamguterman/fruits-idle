using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class DD_CoordinateAxis : DD_DrawGraphic
{
    private static readonly string MARK_TEXT_BASE_NAME = "MarkText";

    private static readonly string LINES_BASE_NAME = "Line";

    private static readonly string COORDINATE_RECT = "CoordinateRect";

    private const float INCH_PER_CENTIMETER = 0.3937008f;

    private readonly float[] MarkIntervalTab = new float[]
    {
        1f,
        2f,
        5f
    };

    private DD_DataDiagram m_DataDiagram;

    private RectTransform m_CoordinateRectT;

    private GameObject m_LinesPreb;

    private GameObject m_MarkTextPreb;

    private List<GameObject> m_LineList = new List<GameObject>();

    private Vector2 m_ZoomSpeed = new Vector2(1f, 1f);

    private Vector2 m_MoveSpeed = new Vector2(1f, 1f);

    private float m_CoordinateAxisMaxWidth = 100f;

    private float m_CoordinateAxisMinWidth = 0.1f;

    private float m_RectThickness = 2f;

    private Color m_BackgroundColor = new Color(0f, 0f, 0f, 0.5f);

    private Color m_MarkColor = new Color(0.8f, 0.8f, 0.8f, 1f);

    private List<GameObject> m_MarkHorizontalTexts = new List<GameObject>();

    private float m_MinMarkTextHeight = 20f;

    private float screenDpi;

    private Rect m_CoordinateAxisViewRange = new Rect(1f, 1f, 1f, 1f);

    public delegate void CoordinateRectChangeHandler(object sender, DD_CoordinateRectChangeEventArgs e);

    public delegate void CoordinateScaleChangeHandler(object sender, DD_CoordinateScaleChangeEventArgs e);

    public delegate void CoordinateZeroPointChangeHandler(object sender, DD_CoordinateZeroPointChangeEventArgs e);

    private float m_PixelPerMark
	{
		get
		{
			return 0.3937008f * m_DataDiagram.m_CentimeterPerMark * screenDpi;
		}
	}

	private Rect m_CoordinateAxisRange
	{
		get
		{
			try
			{
				Vector2 size = m_CoordinateRectT.rect.size;
				return new Rect(0f, 0f, size.x / (m_DataDiagram.m_CentimeterPerCoordUnitX * 0.3937008f * screenDpi), size.y / (m_DataDiagram.m_CentimeterPerCoordUnitY * 0.3937008f * screenDpi));
			}
			catch (NullReferenceException arg)
			{
				Debug.Log(this + " : " + arg);
			}
			return new Rect(Vector2.zero, GetComponent<RectTransform>().rect.size);
		}
	}

	private float m_CoordinateAxisViewSizeX
	{
		get
		{
			try
			{
				return m_CoordinateAxisRange.width * m_CoordinateAxisViewRange.width;
			}
			catch (NullReferenceException arg)
			{
				Debug.Log(this + " : " + arg);
			}
			return m_CoordinateAxisRange.width;
		}
	}

	private float m_CoordinateAxisViewSizeY
	{
		get
		{
			try
			{
				return m_CoordinateAxisRange.height * m_CoordinateAxisViewRange.height;
			}
			catch (NullReferenceException arg)
			{
				Debug.Log(this + " : " + arg);
			}
			return m_CoordinateAxisRange.width;
		}
	}

	public Rect coordinateAxisViewRangeInPixel
	{
		get
		{
			try
			{
				return new Rect(CoordinateToPixel(m_CoordinateAxisViewRange.position - m_CoordinateAxisRange.position), m_CoordinateAxisViewRange.size);
			}
			catch (NullReferenceException arg)
			{
				Debug.Log(this + " : " + arg);
			}
			return new Rect(CoordinateToPixel(m_CoordinateAxisRange.position), m_CoordinateAxisViewRange.size);
		}
	}

	public RectTransform coordinateRectT
	{
		get
		{
			RectTransform result;
			try
			{
				result = m_CoordinateRectT;
			}
			catch
			{
				result = GetComponent<RectTransform>();
			}
			return result;
		}
	}

	public int lineNum
	{
		get
		{
			return m_LineList.Count;
		}
	}

	public event CoordinateRectChangeHandler CoordinateRectChangeEvent;

	public event CoordinateScaleChangeHandler CoordinateScaleChangeEvent;

	public event CoordinateZeroPointChangeHandler CoordinateeZeroPointChangeEvent;

	protected override void Awake()
	{
		screenDpi = Screen.dpi;
		if (null == (m_DataDiagram = GetComponentInParent<DD_DataDiagram>()))
		{
			Debug.Log(this + "Awake Error : null == m_DataDiagram");
			return;
		}
		m_LinesPreb = (GameObject)Resources.Load("Prefabs/Lines");
		if (null == m_LinesPreb)
		{
			Debug.Log("Error : null == m_LinesPreb");
		}
		m_MarkTextPreb = (GameObject)Resources.Load("Prefabs/MarkText");
		if (null == m_MarkTextPreb)
		{
			Debug.Log("Error : null == m_MarkTextPreb");
		}
		try
		{
			m_CoordinateRectT = FindInChild(COORDINATE_RECT).GetComponent<RectTransform>();
			if (null == m_CoordinateRectT)
			{
				Debug.Log("Error : null == m_CoordinateRectT");
				return;
			}
		}
		catch (NullReferenceException arg)
		{
			Debug.Log(this + "," + arg);
		}
		FindExistMarkText(m_MarkHorizontalTexts);
		Rect rect = gameObject.transform.parent.gameObject.GetComponent<RectTransform>().rect;
		m_CoordinateAxisViewRange.position = m_CoordinateAxisRange.position;
		m_CoordinateAxisViewRange.size = new Vector2(1f, 1f);
		m_DataDiagram.RectChangeEvent += OnRectChange;
		m_DataDiagram.ZoomEvent += OnZoom;
		m_DataDiagram.MoveEvent += OnMove;
	}

	private void Update()
	{
	}

	private GameObject FindInChild(string name)
	{
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			if (name == transform.gameObject.name)
			{
				return transform.gameObject;
			}
		}
		return null;
	}

	private void ChangeRect(Rect newRect)
	{
		CoordinateRectChangeEvent(this, new DD_CoordinateRectChangeEventArgs(new Rect(CoordinateToPixel(m_CoordinateAxisRange.position - m_CoordinateAxisViewRange.position), newRect.size)));
	}

	private void ChangeScale(float ZoomX, float ZoomY)
	{
		Vector2 size = m_CoordinateAxisRange.size;
		Vector2 vector = new Vector2(m_CoordinateAxisViewRange.width * size.x, m_CoordinateAxisViewRange.height * size.y);
		float num = size.y / size.x;
		float num2 = ZoomX * m_ZoomSpeed.x;
		float num3 = ZoomY * m_ZoomSpeed.y * num;
		vector.x += num2;
		vector.y += num3;
		if (vector.x > m_CoordinateAxisMaxWidth)
		{
			vector.x = m_CoordinateAxisMaxWidth;
		}
		if (vector.x < m_CoordinateAxisMinWidth)
		{
			vector.x = m_CoordinateAxisMinWidth;
		}
		if (vector.y > m_CoordinateAxisMaxWidth * num)
		{
			vector.y = m_CoordinateAxisMaxWidth * num;
		}
		if (vector.y < m_CoordinateAxisMinWidth * num)
		{
			vector.y = m_CoordinateAxisMinWidth * num;
		}
		m_CoordinateAxisViewRange.width = vector.x / size.x;
		m_CoordinateAxisViewRange.height = vector.y / size.y;
	}

	private void OnRectChange(object sender, DD_RectChangeEventArgs e)
	{
		ChangeRect(m_CoordinateRectT.rect);
		UpdateGeometry();
	}

	private void OnZoom(object sender, DD_ZoomEventArgs e)
	{
		CoordinateScaleChangeEvent(this, new DD_CoordinateScaleChangeEventArgs(m_CoordinateAxisViewRange.width, m_CoordinateAxisViewRange.height));

		ChangeScale(e.ZoomX, e.ZoomY);
		UpdateGeometry();
	}

	private void OnMove(object sender, DD_MoveEventArgs e)
	{
		if (1f > Mathf.Abs(e.MoveX) && 1f > Mathf.Abs(e.MoveY))
		{
			return;
		}
		Vector2 vector = new Vector2(e.MoveX / m_CoordinateRectT.rect.width * m_CoordinateAxisViewSizeX, e.MoveY / m_CoordinateRectT.rect.height * m_CoordinateAxisViewSizeY);
		Vector2 vector2 = new Vector2(-vector.x * m_MoveSpeed.x, -vector.y * m_MoveSpeed.y);
		m_CoordinateAxisViewRange.position = m_CoordinateAxisViewRange.position + vector2;
		if (0f > m_CoordinateAxisViewRange.x)
		{
			m_CoordinateAxisViewRange.x = 0f;
		}

        CoordinateeZeroPointChangeEvent(this, new DD_CoordinateZeroPointChangeEventArgs(CoordinateToPixel(vector2)));
		UpdateGeometry();
	}

	private Vector2 CoordinateToPixel(Vector2 coordPoint)
	{
		return new Vector2(coordPoint.x / m_CoordinateAxisRange.width * m_CoordinateRectT.rect.width, coordPoint.y / m_CoordinateAxisRange.height * m_CoordinateRectT.rect.height);
	}

	private int CalcMarkNum(float pixelPerMark, float totalPixel)
	{
		return Mathf.CeilToInt(totalPixel / ((pixelPerMark > 0f) ? pixelPerMark : 1f));
	}

	private float CalcMarkLevel(float[] makeTab, int markNum, float viewMarkRange)
	{
		float num = viewMarkRange / (float)((markNum > 0) ? markNum : 1);
		float num2 = 1f;
		float num3 = makeTab[0];
		while (num < num3 * num2 || num >= num3 * num2 * 10f)
		{
			if (num < num3 * num2)
			{
				num2 /= 10f;
			}
			else
			{
				if (num < num3 * num2 * 10f)
				{
					break;
				}
				num2 *= 10f;
			}
		}
		num /= num2;
		for (int i = 1; i < makeTab.Length; i++)
		{
			if (Mathf.Abs(num3 - num) > Mathf.Abs(makeTab[i] - num))
			{
				num3 = makeTab[i];
			}
		}
		return num3 * num2;
	}

	private float CeilingFormat(float markLevel, float Val)
	{
		return (float)Mathf.CeilToInt(Val / markLevel) * markLevel;
	}

	private float[] CalcMarkVals(float markLevel, float startViewMarkVal, float endViewMarkVal)
	{
		List<float> list = new List<float>();
		for (float num = CeilingFormat(markLevel, startViewMarkVal); num < endViewMarkVal; num += markLevel)
		{
			list.Add(num);
		}
		float[] array = new float[list.Count];
		list.CopyTo(array);
		return array;
	}

	private float MarkValToPixel(float markVal, float startViewMarkVal, float endViewMarkVal, float stratCoordPixelVal, float endCoordPixelVal)
	{
		if (endViewMarkVal <= startViewMarkVal || markVal <= startViewMarkVal)
		{
			return stratCoordPixelVal;
		}
		return stratCoordPixelVal + (endCoordPixelVal - stratCoordPixelVal) * ((markVal - startViewMarkVal) / (endViewMarkVal - startViewMarkVal));
	}

	private float[] MarkValsToPixel(float[] markVals, float startViewMarkVal, float endViewMarkVal, float stratCoordPixelVal, float endCoordPixelVal)
	{
		float[] array = new float[markVals.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = MarkValToPixel(markVals[i], startViewMarkVal, endViewMarkVal, stratCoordPixelVal, endCoordPixelVal);
		}
		return array;
	}

	private void SetMarkText(GameObject markText, Rect rect, string str, bool isEnable)
	{
		if (null == markText)
		{
			Debug.Log("SetMarkText Error : null == markText");
			return;
		}
		RectTransform component = markText.GetComponent<RectTransform>();
		if (null == component)
		{
			Debug.Log("SetMarkText Error : null == rectTransform");
			return;
		}
		Text component2 = markText.GetComponent<Text>();
		if (null == component2)
		{
			Debug.Log("SetMarkText Error : null == Text");
			return;
		}
		component.anchorMin = new Vector2(0f, 0f);
		component.anchorMax = new Vector2(0f, 0f);
		component.pivot = new Vector2(0f, 0f);
		component.anchoredPosition = rect.position;
		component.sizeDelta = rect.size;
		component2.text = str;
		component2.enabled = isEnable;
	}

	private void ResetMarkText(GameObject markText)
	{
		SetMarkText(markText, new Rect(new Vector2(0f, m_CoordinateRectT.rect.y), new Vector2(m_CoordinateRectT.rect.x, m_MinMarkTextHeight)), null, false);
	}

	private void ResetAllMarkText(List<GameObject> markTexts)
	{
		if (markTexts == null)
		{
			UnityEngine.Debug.Log("DisableAllMarkText Error : null == markTexts");
			return;
		}
		foreach (GameObject markText in markTexts)
		{
			ResetMarkText(markText);
		}
	}

	private void DrawOneHorizontalMarkText(GameObject markText, float markValY, float pixelY, Rect coordinateRect)
	{
		SetMarkText(markText, new Rect(new Vector2(0f, pixelY - m_MinMarkTextHeight / 2f), new Vector2(coordinateRect.x - 2f, m_MinMarkTextHeight)), markValY.ToString(), true);
	}

	private IEnumerator DrawHorizontalTextMark(float[] marksVals, float[] marksPixel, Rect coordinateRect)
	{
		yield return new WaitForSeconds(0f);
		while (marksPixel.Length > m_MarkHorizontalTexts.Count)
		{
			GameObject gameObject = Instantiate<GameObject>(m_MarkTextPreb, transform);
			gameObject.name = string.Format("{0}{1}", MARK_TEXT_BASE_NAME, m_MarkHorizontalTexts.Count);
			m_MarkHorizontalTexts.Add(gameObject);
		}
		ResetAllMarkText(m_MarkHorizontalTexts);
		for (int i = 0; i < marksPixel.Length; i++)
		{
			DrawOneHorizontalMarkText(m_MarkHorizontalTexts[i], marksVals[i], marksPixel[i], coordinateRect);
		}
		yield return 0;
		yield break;
	}

	private void DrawOneHorizontalMark(VertexHelper vh, float pixelY, Rect coordinateRect)
	{
		Vector2 startPoint = new Vector2(coordinateRect.x, pixelY);
		Vector2 endPoint = new Vector2(coordinateRect.x + coordinateRect.width, pixelY);
		base.DrawHorizontalSegmet(vh, startPoint, endPoint, m_MarkColor, m_RectThickness / 2f, 1f, 1f);
	}

	private void DrawHorizontalMark(VertexHelper vh, Rect coordinateRect)
	{
		int markNum = CalcMarkNum(m_PixelPerMark, coordinateRect.height);
		float markLevel = CalcMarkLevel(MarkIntervalTab, markNum, m_CoordinateAxisViewSizeY);
		float[] array = CalcMarkVals(markLevel, m_CoordinateAxisViewRange.y, m_CoordinateAxisViewRange.y + m_CoordinateAxisViewSizeY);
		float[] array2 = MarkValsToPixel(array, m_CoordinateAxisViewRange.y, m_CoordinateAxisViewRange.y + m_CoordinateAxisViewSizeY, coordinateRect.y, coordinateRect.y + coordinateRect.height);
		for (int i = 0; i < array2.Length; i++)
		{
			DrawOneHorizontalMark(vh, array2[i], coordinateRect);
		}
		base.StartCoroutine(DrawHorizontalTextMark(array, array2, coordinateRect));
	}

	private void DrawRect(VertexHelper vh, Rect rect)
	{
		base.DrawRectang(vh, rect.position, new Vector2(rect.x, rect.y + rect.height), new Vector2(rect.x + rect.width, rect.y + rect.height), new Vector2(rect.x + rect.width, rect.y), m_BackgroundColor);
	}

	private void DrawRectCoordinate(VertexHelper vh)
	{
		Rect rect = new Rect(m_CoordinateRectT.offsetMin, m_CoordinateRectT.rect.size);
		DrawRect(vh, new Rect(rect));
		DrawHorizontalMark(vh, rect);
	}

	private void FindExistMarkText(List<GameObject> markTexts)
	{
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			if (Regex.IsMatch(transform.gameObject.name, MARK_TEXT_BASE_NAME))
			{
				transform.gameObject.name = string.Format("{0}{1}", MARK_TEXT_BASE_NAME, m_MarkHorizontalTexts.Count);
				markTexts.Add(transform.gameObject);
			}
		}
	}

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		vh.Clear();
		DrawRectCoordinate(vh);
	}

	public void InputPoint(GameObject line, Vector2 point)
	{
		line.GetComponent<DD_Lines>().AddPoint(CoordinateToPixel(point));
	}

	public GameObject AddLine(string name)
	{
		if (null == m_LinesPreb)
		{
			m_LinesPreb = (GameObject)Resources.Load("Prefabs/Lines");
		}
		try
		{
			m_LineList.Add(Instantiate<GameObject>(m_LinesPreb, m_CoordinateRectT));
		}
		catch (NullReferenceException arg)
		{
			Debug.Log(this + "," + arg);
			return null;
		}
		m_LineList[m_LineList.Count - 1].GetComponent<DD_Lines>().lineName = name;
		m_LineList[m_LineList.Count - 1].GetComponent<DD_Lines>().color = Color.yellow;
		m_LineList[m_LineList.Count - 1].name = string.Format("{0}{1}", LINES_BASE_NAME, m_LineList[m_LineList.Count - 1].GetComponent<DD_Lines>().lineName);
		return m_LineList[m_LineList.Count - 1];
	}

	public bool RemoveLine(GameObject line)
	{
		if (null == line)
		{
			return true;
		}
		if (!m_LineList.Remove(line))
		{
			return false;
		}
		try
		{
			line.GetComponent<DD_Lines>().Clear();
		}
		catch (NullReferenceException)
		{
		}
		UnityEngine.Object.Destroy(line);
		return true;
	}

}
