using System;
using System.Collections.Generic;
using UnityEngine;

public class Sample : MonoBehaviour
{
    private List<GameObject> lineList = new List<GameObject>();

    private DD_DataDiagram m_DataDiagram;

    private bool m_IsContinueInput;

    private float m_Input;

    private float h;

    private void AddALine()
	{
		if (null == m_DataDiagram)
		{
			return;
		}
		Color color = Color.HSVToRGB(((h += 0.1f) > 1f) ? (h - 1f) : h, 0.8f, 0.8f);
		GameObject gameObject = m_DataDiagram.AddLine(color.ToString(), color);
		if (null != gameObject)
		{
			lineList.Add(gameObject);
		}
	}

	private void Start()
	{
		GameObject gameObject = GameObject.Find("DataDiagram");
		if (null == gameObject)
		{
			Debug.LogWarning("can not find a gameobject of DataDiagram");
			return;
		}
		m_DataDiagram = gameObject.GetComponent<DD_DataDiagram>();
		m_DataDiagram.PreDestroyLineEvent += delegate(object s, DD_PreDestroyLineEventArgs e)
		{
			lineList.Remove(e.line);
		};
		AddALine();
	}

	private void Update()
	{
	}

	private void FixedUpdate()
	{
		m_Input += Time.deltaTime;
		ContinueInput(m_Input);
	}

	private void ContinueInput(float f)
	{
		if (null == m_DataDiagram)
		{
			return;
		}
		if (!m_IsContinueInput)
		{
			return;
		}
		float num = 0f;
		foreach (GameObject line in lineList)
		{
			m_DataDiagram.InputPoint(line, new Vector2(0.1f, (Mathf.Sin(f + num) + 1f) * 2f));
			num += 1f;
		}
	}

	public void onButton()
	{
		if (null == m_DataDiagram)
		{
			return;
		}
		foreach (GameObject line in lineList)
		{
			m_DataDiagram.InputPoint(line, new Vector2(1f, UnityEngine.Random.value * 4f));
		}
	}

	public void OnAddLine()
	{
		AddALine();
	}

	public void OnRectChange()
	{
		if (null == m_DataDiagram)
		{
			return;
		}
		Rect value = new Rect(UnityEngine.Random.value * (float)Screen.width, UnityEngine.Random.value * (float)Screen.height, UnityEngine.Random.value * (float)Screen.width / 2f, UnityEngine.Random.value * (float)Screen.height / 2f);
		m_DataDiagram.rect = new Rect?(value);
	}

	public void OnContinueInput()
	{
		m_IsContinueInput = !m_IsContinueInput;
	}

}
