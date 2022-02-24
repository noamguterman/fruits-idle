using System;
using UnityEngine;

public class DD_PreDestroyLineEventArgs : EventArgs
{
	public DD_PreDestroyLineEventArgs(GameObject line)
	{
		this.m_Line = null;
		if (null == line)
		{
			return;
		}
		if (null == line.GetComponent<DD_Lines>())
		{
			return;
		}
		this.m_Line = line;
	}

	public GameObject line
	{
		get
		{
			return this.m_Line;
		}
	}

	private GameObject m_Line;
}
