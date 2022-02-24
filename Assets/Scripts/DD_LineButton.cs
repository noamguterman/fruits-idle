using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DD_LineButton : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public GameObject line
	{
		get
		{
			return this.m_Line;
		}
		set
		{
			DD_Lines component = value.GetComponent<DD_Lines>();
			if (null == component)
			{
				UnityEngine.Debug.LogWarning(this.ToString() + "LineButton error : set line null == value.GetComponent<Lines>()");
				return;
			}
			this.m_Line = value;
			this.SetLineButton(component);
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void SetLabel(DD_Lines lines)
	{
		if (null == this.m_Label || null == this.m_Label.GetComponent<Text>())
		{
			this.m_Label = null;
		}
		try
		{
			this.m_Label.GetComponent<Text>().text = lines.GetComponent<DD_Lines>().lineName;
			this.m_Label.GetComponent<Text>().color = lines.GetComponent<DD_Lines>().color;
		}
		catch
		{
			this.m_Label.GetComponent<Text>().color = Color.white;
		}
	}

	public void SetLineButton(DD_Lines lines)
	{
		base.name = string.Format("Button{0}", lines.gameObject.name);
		base.GetComponent<Image>().color = lines.color;
		this.SetLabel(lines);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (eventData.pointerCurrentRaycast.gameObject != base.gameObject)
		{
			return;
		}
		if (null == this.m_Label)
		{
			return;
		}
		DD_DataDiagram componentInParent = base.GetComponentInParent<DD_DataDiagram>();
		if (null == componentInParent)
		{
			return;
		}
		this.m_Label.transform.SetParent(componentInParent.transform);
		this.m_Label.transform.position = base.transform.position + new Vector3(0f, -base.GetComponent<RectTransform>().rect.height / 2f, 0f);
		this.m_Label.SetActive(true);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (null == this.m_Label)
		{
			return;
		}
		this.m_Label.transform.SetParent(base.transform);
		this.m_Label.SetActive(false);
	}

	public void OnButtonClick()
	{
		if (UnityEngine.Input.GetKey(KeyCode.LeftControl))
		{
			return;
		}
		if (null == this.m_Line)
		{
			UnityEngine.Debug.LogWarning(this.ToString() + "error OnButtonClick : null == m_Line");
			return;
		}
		DD_Lines component = this.m_Line.GetComponent<DD_Lines>();
		if (null == component)
		{
			UnityEngine.Debug.LogWarning(this.ToString() + "error OnButtonClick : null == DD_Lines");
			return;
		}
		component.IsShow = !component.IsShow;
	}

	public void OnButtonClickWithCtrl()
	{
		if (UnityEngine.Input.GetKey(KeyCode.LeftControl))
		{
			try
			{
				base.transform.GetComponentInParent<DD_DataDiagram>().DestroyLine(this.m_Line);
			}
			catch (NullReferenceException)
			{
				UnityEngine.Debug.LogWarning("OnButtonClickWithCtrl throw a NullReferenceException");
			}
		}
	}

	public void DestroyLineButton()
	{
		if (null != this.m_Label)
		{
			UnityEngine.Object.Destroy(this.m_Label);
		}
	}

	private GameObject m_Line;

	public GameObject m_Label;
}
