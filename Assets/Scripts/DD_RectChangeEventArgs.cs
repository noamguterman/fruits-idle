using System;
using UnityEngine;

public class DD_RectChangeEventArgs : EventArgs
{
	public DD_RectChangeEventArgs(Vector2 size)
	{
		this.m_Size = size;
	}

	public Vector2 size
	{
		get
		{
			return this.m_Size;
		}
	}

	private readonly Vector2 m_Size;
}
