using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace Game.GameScreen
{
	[RequireComponent(typeof(RectTransform))]
	public class GameScreenController : MonoBehaviour
    {
        //[FoldoutGroup("References", 0)]
        //[SerializeField]
        //[Required]
        //private SpriteMask gameScreenMask;

        [SerializeField]
        private string sortingLayer;

        [SerializeField]
        private int index;

        private RectTransform rTransform;

        public int Index
		{
			get
			{
				return index;
			}
		}

		public string SortLayer
		{
			get
			{
				return sortingLayer;
			}
		}

		public bool IsReachable { get; private set; }

		public RectTransform Transform
		{
			get
			{
				if (!rTransform)
				{
					return rTransform = GetComponent<RectTransform>();
				}
				return rTransform;
			}
		}

		public Vector2 Size { get; private set; }

		public Vector2 Position
		{
			get
			{
				return rTransform.position;
			}
			set
			{
				rTransform.position = value;
			}
		}

		public void Initialize(GameScreenSetting gameScreenSetting)
		{
			rTransform.sizeDelta = (Size = gameScreenSetting.GameScreenSize);
			//gameScreenMask.transform.localScale = Size;
			//gameScreenMask.isCustomRangeActive = true;
			//StartCoroutine(CoroutineUtils.ExecuteAfter(delegate()
			//{
			//	foreach (SpriteRenderer spriteRenderer in from sr in GetComponentsInChildren<SpriteRenderer>()
			//	where sr.sortingLayerName != "Always On Top"
			//	select sr)
			//	{
			//		spriteRenderer.sortingLayerID = SortingLayer.NameToID(sortingLayer);
			//	}
			//}, (YieldInstruction)null));
			Vector2 sizeDelta = rTransform.sizeDelta;
			Vector3 position = rTransform.position;
			Bounds gameScreenBounds = new Bounds
			{
				center = transform.position,
				extents = new Vector3(sizeDelta.x / 2f, sizeDelta.y / 2f),
				max = position + new Vector3(sizeDelta.x / 2f, sizeDelta.y / 2f),
				min = position - new Vector3(sizeDelta.x / 2f, sizeDelta.y / 2f),
				size = sizeDelta
			};
			IGameScreenComponent[] componentsInChildren = GetComponentsInChildren<IGameScreenComponent>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].GameScreenBounds = gameScreenBounds;
			}
			//gameScreenMask.frontSortingLayerID = SortingLayer.NameToID(sortingLayer);
			//gameScreenMask.backSortingLayerID = SortingLayer.NameToID(sortingLayer);
			//gameScreenMask.frontSortingOrder = 100;
			//gameScreenMask.backSortingOrder = -100;
			IsReachable = gameScreenSetting.IsReachable;
		}

	}
}
