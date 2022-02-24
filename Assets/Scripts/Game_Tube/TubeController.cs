using System;
using System.Collections.Generic;
using System.Linq;
using Game.Pieces;
using Settings.UI.Tabs;
using Sirenix.OdinInspector;
using UI.ListItems;
using UI.ListItems.Upgrades.Piece;
using UnityEngine;
using Utilities;

namespace Game.Tube
{
	public class TubeController : MonoBehaviour
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        private List<TubeNode> allNodes;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        private TubeNode tubeEnter;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        private Sprite tubeSprite;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private CollisionEvent enterTubeTrigger;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private TubeSettings tubeSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private UpgradeSettings upgradeSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private PieceSettings pieceSettings;

        private List<ISuspendable> allSuspendableObjects;

        private HashSet<Piece> piecesInTrigger;

        private bool isEnabled;

        private void Start()
		{
			this.enterTubeTrigger.TriggerEnter += this.OnTriggerEnter;
			this.allSuspendableObjects = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>().OfType<ISuspendable>().ToList<ISuspendable>();
			this.piecesInTrigger = new HashSet<Piece>();
			this.isEnabled = (this.upgradeSettings.GetCurrentUpgrade(UpgradeType.SmeltersUnlock).Level > 0);
			if (!this.isEnabled)
			{
				this.upgradeSettings.GetUpgradeListItem(UpgradeType.SmeltersUnlock).LevelIncreased += this.OnSmeltersScreenUnlock;
				this.enterTubeTrigger.TriggerExit += this.OnTriggerExit;
			}
			this.allNodes.ForEach(delegate(TubeNode n)
			{
				if (n.IsLast)
				{
					n.BufferStateChange += this.OnNodeBufferStateChange;
				}
				n.transform.SetPositionZ(-5f);
			});
		}

		private void OnNodeBufferStateChange()
		{
			bool suspend = this.allNodes.Any((TubeNode n) => n.IsBufferFull);
			this.allSuspendableObjects.ForEach(delegate(ISuspendable s)
			{
				if (suspend)
				{
                    //Debug.LogError("Suspend");
					s.Suspend();
					return;
				}
				s.Resume();
			});
		}

		private void OnSmeltersScreenUnlock()
		{
			this.isEnabled = true;
			foreach (Piece piece in this.piecesInTrigger)
			{
				if (!(piece == null) && piece.gameObject.activeSelf)
				{
					this.tubeEnter.AddPiece(piece);
				}
			}
			this.piecesInTrigger.Clear();
			this.enterTubeTrigger.TriggerExit -= this.OnTriggerExit;
			this.upgradeSettings.GetUpgradeListItem(UpgradeType.SmeltersUnlock).LevelIncreased -= this.OnSmeltersScreenUnlock;
		}

		private void OnTriggerEnter(Collider other)
		{
			Piece component = other.GetComponent<Piece>();
			if (component == null)
			{
				return;
			}
			if (this.isEnabled)
			{
				this.tubeEnter.AddPiece(component);
                //component.transform.localScale = Vector3.one * 0.8f;
				return;
			}
			this.piecesInTrigger.Add(component);
		}

		private void OnTriggerExit(Collider other)
		{
			Piece component = other.GetComponent<Piece>();
			if (component == null)
			{
				return;
			}
			this.piecesInTrigger.Remove(component);
		}

	}
}
