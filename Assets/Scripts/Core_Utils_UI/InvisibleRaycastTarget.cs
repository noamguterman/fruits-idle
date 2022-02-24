using System;
using UnityEngine.UI;

namespace Core.Utils.UI
{
	public sealed class InvisibleRaycastTarget : Graphic
	{
		public override void SetMaterialDirty()
		{
		}

		public override void SetVerticesDirty()
		{
		}

		protected override void OnPopulateMesh(VertexHelper vertexHelper)
		{
			vertexHelper.Clear();
		}
	}
}
