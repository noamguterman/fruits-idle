using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DD_DragBar : MonoBehaviour, IDragHandler, IEventSystemHandler
{
	public bool canDrag
	{
		get
		{
			return base.gameObject.activeSelf;
		}
		set
		{
			LayoutElement component = base.GetComponent<LayoutElement>();
			if (null == component)
			{
				UnityEngine.Debug.LogWarning(this + " : can not find LayoutElement");
				return;
			}
			if (value)
			{
				base.gameObject.SetActive(true);
				component.ignoreLayout = false;
				return;
			}
			base.gameObject.SetActive(false);
			component.ignoreLayout = true;
		}
	}

	private void Start()
	{
		this.GetZoomButton();
		DD_DataDiagram componentInParent = base.GetComponentInParent<DD_DataDiagram>();
		if (null == componentInParent)
		{
			UnityEngine.Debug.LogWarning(this + " : can not find any gameobject with a DataDiagram object");
			return;
		}
		this.m_DataDiagram = componentInParent.gameObject;
		this.m_DataDiagramRT = this.m_DataDiagram.GetComponent<RectTransform>();
		if (null == this.m_DataDiagram.transform.parent)
		{
			this.m_Parent = null;
		}
		else
		{
			this.m_Parent = this.m_DataDiagram.transform.parent.gameObject;
		}
		if (null == this.m_Parent)
		{
			UnityEngine.Debug.LogWarning(this + " : can not DataDiagram's parent");
			return;
		}
		if (null == this.m_Parent.GetComponent<Canvas>())
		{
			this.canDrag = false;
			return;
		}
		this.canDrag = true;
	}

	private void GetZoomButton()
	{
		if (!(null == this.m_ZoomButton))
		{
			DD_ZoomButton zoomButton = this.m_ZoomButton;
			zoomButton.ZoomButtonClickEvent = (DD_ZoomButton.ZoomButtonClickHandle)Delegate.Combine(zoomButton.ZoomButtonClickEvent, new DD_ZoomButton.ZoomButtonClickHandle(this.OnCtrlButtonClick));
			return;
		}
		GameObject gameObject = GameObject.Find("ZoomButton");
		if (null == gameObject)
		{
			UnityEngine.Debug.LogWarning(this + " : can not find gameobject ZoomButton");
			return;
		}
		if (null == gameObject.GetComponentInParent<DD_DataDiagram>())
		{
			UnityEngine.Debug.LogWarning(this + " : the gameobject ZoomButton is not under the DataDiagram");
			return;
		}
		this.m_ZoomButton = gameObject.GetComponent<DD_ZoomButton>();
		if (null == this.m_ZoomButton)
		{
			UnityEngine.Debug.LogWarning(this + " : can not find object DD_ZoomButton");
			return;
		}
		DD_ZoomButton zoomButton2 = this.m_ZoomButton;
		zoomButton2.ZoomButtonClickEvent = (DD_ZoomButton.ZoomButtonClickHandle)Delegate.Combine(zoomButton2.ZoomButtonClickEvent, new DD_ZoomButton.ZoomButtonClickHandle(this.OnCtrlButtonClick));
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (null == this.m_DataDiagramRT)
		{
			return;
		}
		this.m_DataDiagramRT.anchoredPosition += eventData.delta;
	}

	private void OnCtrlButtonClick(object sender, ZoomButtonClickEventArgs e)
	{
		if (null == this.m_DataDiagram.transform.parent)
		{
			UnityEngine.Debug.LogWarning(this + " OnCtrlButtonClick : can not DataDiagram's parent");
			return;
		}
		if (this.m_Parent != this.m_DataDiagram.transform.parent.gameObject)
		{
			this.m_Parent = this.m_DataDiagram.transform.parent.gameObject;
			if (null != this.m_Parent.GetComponent<Canvas>())
			{
				this.canDrag = true;
				return;
			}
			this.canDrag = false;
		}
	}

	private DD_ZoomButton m_ZoomButton;

	private GameObject m_DataDiagram;

	private GameObject m_Parent;

	private RectTransform m_DataDiagramRT;
}
