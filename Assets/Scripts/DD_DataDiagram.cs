using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DD_DataDiagram : MonoBehaviour, IScrollHandler, IEventSystemHandler, IDragHandler
{
	public event DD_DataDiagram.RectChangeHandler RectChangeEvent;

	public event DD_DataDiagram.ZoomHandler ZoomEvent;

	public event DD_DataDiagram.MoveHandler MoveEvent;

	public event DD_DataDiagram.PreDestroyLineHandler PreDestroyLineEvent;

	public Rect? rect
	{
		get
		{
			RectTransform component = base.gameObject.GetComponent<RectTransform>();
			if (null == component)
			{
				return null;
			}
			return new Rect?(component.rect);
		}
		set
		{
			Rect value2 = value.Value;
			if (this.MinRectSize.x > value2.width)
			{
				value2.width = this.MinRectSize.x;
			}
			if (this.MinRectSize.y > value2.height)
			{
				value2.height = this.MinRectSize.y;
			}
			RectTransform component = base.gameObject.GetComponent<RectTransform>();
			if (null == component)
			{
				return;
			}
			component.anchoredPosition = DD_CalcRectTransformHelper.CalcAnchorPosition(value2, component.anchorMin, component.anchorMax, base.transform.parent.GetComponentInParent<RectTransform>().rect.size, component.pivot);
			component.sizeDelta = DD_CalcRectTransformHelper.CalcSizeDelta(value2, component.anchorMin, component.anchorMax, base.transform.parent.GetComponentInParent<RectTransform>().rect.size);
			if (this.RectChangeEvent != null)
			{
				this.RectChangeEvent(this, new DD_RectChangeEventArgs(value2.size));
			}
		}
	}

	private void Awake()
	{
		DD_CoordinateAxis componentInChildren = base.transform.GetComponentInChildren<DD_CoordinateAxis>();
		if (null == componentInChildren)
		{
			this.m_CoordinateAxis = UnityEngine.Object.Instantiate<GameObject>((GameObject)Resources.Load("Prefabs/CoordinateAxis"), base.gameObject.transform);
			this.m_CoordinateAxis.name = "CoordinateAxis";
		}
		else
		{
			this.m_CoordinateAxis = componentInChildren.gameObject;
		}
		DD_LineButtonsContent componentInChildren2 = base.GetComponentInChildren<DD_LineButtonsContent>();
		if (null == componentInChildren2)
		{
			UnityEngine.Debug.LogWarning(this + "Awake Error : null == lineButtonsContent");
			return;
		}
		if (null == (this.lineButtonsContent = componentInChildren2.gameObject))
		{
			UnityEngine.Debug.LogWarning(this + "Awake Error : null == lineButtonsContent");
			return;
		}
	}

	private void Start()
	{
		if (this.RectChangeEvent != null)
		{
			try
			{
				this.RectChangeEvent(this, new DD_RectChangeEventArgs(base.gameObject.GetComponent<RectTransform>().rect.size));
			}
			catch (NullReferenceException message)
			{
				UnityEngine.Debug.LogWarning(message);
			}
		}
	}

	private void Update()
	{
	}

	public void OnDrag(PointerEventData eventData)
	{
		this.MoveEvent(this, new DD_MoveEventArgs(eventData.delta.x, eventData.delta.y));
	}

	public void OnScroll(PointerEventData eventData)
	{
		if (Input.GetMouseButton(0))
		{
			this.ZoomEvent(this, new DD_ZoomEventArgs(-eventData.scrollDelta.y, 0f));
			return;
		}
		if (Input.GetMouseButton(1))
		{
			this.ZoomEvent(this, new DD_ZoomEventArgs(0f, eventData.scrollDelta.y));
			return;
		}
		this.ZoomEvent(this, new DD_ZoomEventArgs(-eventData.scrollDelta.y, -eventData.scrollDelta.y));
	}

	private void SetLineButtonColor(GameObject line, Color color)
	{
		foreach (object obj in this.lineButtonsContent.transform)
		{
			Transform transform = (Transform)obj;
			if (line == transform.gameObject.GetComponent<DD_LineButton>().line)
			{
				transform.gameObject.GetComponent<DD_LineButton>().line = line;
				break;
			}
		}
	}

	private void SetLineColor(GameObject line, Color color)
	{
		if (null == line)
		{
			return;
		}
		DD_Lines component = line.GetComponent<DD_Lines>();
		if (null == component)
		{
			UnityEngine.Debug.LogWarning(line.ToString() + " SetLineColor error : null == lines");
			return;
		}
		component.color = color;
		this.SetLineButtonColor(line, color);
	}

	private bool AddLineButton(GameObject line)
	{
		if (null == this.lineButtonsContent)
		{
			UnityEngine.Debug.LogWarning(this + "AddLineButton Error : null == lineButtonsContent");
			return false;
		}
		if (this.lineButtonsContent.transform.childCount >= this.maxLineNum)
		{
			return false;
		}
		if (null == line)
		{
			UnityEngine.Debug.LogWarning(this + "AddLineButton Error : null == line");
			return false;
		}
		DD_Lines component = line.GetComponent<DD_Lines>();
		if (null == component)
		{
			UnityEngine.Debug.LogWarning(this + "AddLineButton Error : null == lines");
			return false;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>((GameObject)Resources.Load("Prefabs/LineButton"), this.lineButtonsContent.transform);
		if (null == gameObject)
		{
			UnityEngine.Debug.LogWarning(this + "AddLineButton Error : null == button");
			return false;
		}
		gameObject.GetComponent<DD_LineButton>().line = line;
		return true;
	}

	private bool DestroyLineButton(GameObject line)
	{
		if (null == this.lineButtonsContent)
		{
			UnityEngine.Debug.Log(this + "AddLineButton Error : null == lineButtonsContent");
			return false;
		}
		foreach (object obj in this.lineButtonsContent.transform)
		{
			Transform transform = (Transform)obj;
			try
			{
				if (line == transform.gameObject.GetComponent<DD_LineButton>().line)
				{
					transform.gameObject.GetComponent<DD_LineButton>().DestroyLineButton();
					UnityEngine.Object.Destroy(transform.gameObject);
					return true;
				}
			}
			catch (NullReferenceException)
			{
				return false;
			}
		}
		return false;
	}

	public void InputPoint(GameObject line, Vector2 point)
	{
		this.m_CoordinateAxis.GetComponent<DD_CoordinateAxis>().InputPoint(line, point);
	}

	public GameObject AddLine(string name)
	{
		DD_CoordinateAxis component = this.m_CoordinateAxis.GetComponent<DD_CoordinateAxis>();
		if (component.lineNum >= this.maxLineNum)
		{
			MonoBehaviour.print("coordinate.lineNum > maxLineNum");
			return null;
		}
		if (component.lineNum != this.lineButtonsContent.transform.childCount)
		{
			MonoBehaviour.print("coordinate.lineNum != m_LineButtonList.Count");
		}
		GameObject gameObject = component.AddLine(name);
		if (!this.AddLineButton(gameObject))
		{
			if (!component.RemoveLine(gameObject))
			{
				MonoBehaviour.print(this.ToString() + " AddLine error : false == coordinate.RemoveLine(line)");
			}
			gameObject = null;
		}
		return gameObject;
	}

	public GameObject AddLine(string name, Color color)
	{
		GameObject gameObject = this.AddLine(name);
		this.SetLineColor(gameObject, color);
		return gameObject;
	}

	public bool DestroyLine(GameObject line)
	{
		if (this.PreDestroyLineEvent != null)
		{
			this.PreDestroyLineEvent(this, new DD_PreDestroyLineEventArgs(line));
		}
		if (!this.DestroyLineButton(line))
		{
			return false;
		}
		try
		{
			if (!this.m_CoordinateAxis.GetComponent<DD_CoordinateAxis>().RemoveLine(line))
			{
				return false;
			}
		}
		catch (NullReferenceException)
		{
			return false;
		}
		return true;
	}

	private readonly Vector2 MinRectSize = new Vector2(100f, 80f);

	private GameObject m_CoordinateAxis;

	private GameObject lineButtonsContent;

	public int maxLineNum = 5;

	public int m_MaxPointNum = 65535;

	public float m_CentimeterPerMark = 1f;

	public float m_CentimeterPerCoordUnitX = 1f;

	public float m_CentimeterPerCoordUnitY = 1f;

	public delegate void RectChangeHandler(object sender, DD_RectChangeEventArgs e);

	public delegate void ZoomHandler(object sender, DD_ZoomEventArgs e);

	public delegate void MoveHandler(object sender, DD_MoveEventArgs e);

	public delegate void PreDestroyLineHandler(object sender, DD_PreDestroyLineEventArgs e);
}
