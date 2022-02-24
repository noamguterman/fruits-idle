using System;
using UnityEngine;

public class DD_ZoomButton : MonoBehaviour
{
    private DD_DataDiagram m_DataDiagram;

    private RTParam[] RTparams = new RTParam[2];

    private int paramSN;

    public ZoomButtonClickHandle ZoomButtonClickEvent;

    private struct RTParam
    {
        public Transform parent;

        public Rect rect;
    }

    public delegate void ZoomButtonClickHandle(object sender, ZoomButtonClickEventArgs args);

    private void Awake()
	{
		m_DataDiagram = GetComponentInParent<DD_DataDiagram>();
		if (null == m_DataDiagram)
		{
			Debug.LogWarning(this + "Awake Error : null == m_DataDiagram");
			return;
		}
	}

	private void Start()
	{
		if (null == m_DataDiagram)
		{
			return;
		}
		RTparams[0].parent = m_DataDiagram.transform.parent;
		RectTransform component = m_DataDiagram.GetComponent<RectTransform>();
		RTparams[0].rect = DD_CalcRectTransformHelper.CalcLocalRect(component.anchorMin, component.anchorMax, RTparams[0].parent.GetComponent<RectTransform>().rect.size, component.pivot, component.anchoredPosition, component.rect);
		RTparams[1].parent = GetComponentInParent<Canvas>().transform;
		RTparams[1].rect = new Rect(new Vector2((float)(Screen.width / 10), (float)(Screen.height / 10)), new Vector2((float)(Screen.width * 8 / 10), (float)(Screen.height * 8 / 10)));
		paramSN = 0;
	}

	private void Update()
	{
	}

	public void OnZoomButton()
	{
		if (null == m_DataDiagram)
		{
			return;
		}
		paramSN = (paramSN + 1) % 2;
		m_DataDiagram.transform.SetParent(RTparams[paramSN].parent);
		m_DataDiagram.rect = new Rect?(RTparams[paramSN].rect);

        ZoomButtonClickEvent(this, new ZoomButtonClickEventArgs());
	}

}
