using System;
using UnityEngine;

namespace Game.GameScreen
{
	public interface IGameScreenComponent
	{
		Bounds GameScreenBounds { set; }
	}
}
