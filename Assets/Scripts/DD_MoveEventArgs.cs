using System;

public class DD_MoveEventArgs : EventArgs
{
	public DD_MoveEventArgs(float dx, float dy)
	{
		this._moveX = dx;
		this._moveY = dy;
	}

	public float MoveX
	{
		get
		{
			return this._moveX;
		}
	}

	public float MoveY
	{
		get
		{
			return this._moveY;
		}
	}

	private float _moveX;

	private float _moveY;
}
