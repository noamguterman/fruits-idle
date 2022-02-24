using System;
using UnityEngine;

public class DD_CoordinateZeroPointChangeEventArgs : EventArgs
{
	public DD_CoordinateZeroPointChangeEventArgs(Vector2 zeroPoint)
	{
		this.zeroPoint = zeroPoint;
	}

	public Vector2 zeroPoint;
}
