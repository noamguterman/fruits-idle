using System;

public class DD_CoordinateScaleChangeEventArgs : EventArgs
{
	public DD_CoordinateScaleChangeEventArgs(float scaleX, float scaleY)
	{
		this.scaleX = scaleX;
		this.scaleY = scaleY;
	}

	public float scaleX;

	public float scaleY;
}
