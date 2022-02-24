using System;
using UnityEngine;

public class DD_CoordinateRectChangeEventArgs : EventArgs
{
	public DD_CoordinateRectChangeEventArgs(Rect newRect)
	{
		this.viewRectInPixel = newRect;
	}

	public Rect viewRectInPixel;
}
