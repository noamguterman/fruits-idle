using System;

public class DD_ZoomEventArgs : EventArgs
{
	public DD_ZoomEventArgs(float valX, float valY)
	{
		this._zoomX = valX;
		this._zoomY = valY;
	}

	public float ZoomX
	{
		get
		{
			return this._zoomX;
		}
	}

	public float ZoomY
	{
		get
		{
			return this._zoomY;
		}
	}

	private float _zoomX;

	private float _zoomY;
}
