using System;
using System.Collections.Generic;
using System.Linq;
using Game.Audio;
using Game.Pieces;
using Sirenix.OdinInspector;
using UI.ListItems;
using UnityEngine;
using Utilities;

namespace Game.Tube
{
	public class TubeNode : MonoBehaviour, ISuspendable
    {
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private List<TubeNode> nextNodes;

        //[SerializeField]
        //[SceneObjectsOnly]
        //private Gauge gauge;

        [SerializeField]
        [Required]
        private TubeSettings tubeSettings;

        [SerializeField]
        private SoundSettings soundSettings;

        [SerializeField]
        private SoundType soundType;

        private List<int> allowedPieceTypeIDs;

        private Dictionary<int, List<Piece>> piecesInNode;

        private int maxAllowedPieces;

        private int currentPiecesAmount;

        private Coroutine changeSpeedMultiplierRoutine;

        private Coroutine stopSoundDelayedRoutine;

        private Reference<float> speedMultiplier;

        private int currentRequiredTypeID;
        private int currentRequiredTypeID1;

        public event Action BufferStateChange;

        public ParticleGenerator liquidGenerator;

		public IReadOnlyList<TubeNode> NextNodes
		{
			get
			{
				return nextNodes;
			}
		}

		private Transform Transform { get; set; }

		public Vector3 Position
		{
			get
			{
				return Transform.position;
			}
			set
			{
				Transform.position = value;
			}
		}

		public bool IsRestrictedByType { get; set; }

		public bool IsLast
		{
			get
			{
				return nextNodes.Count == 0;
			}
		}

		public bool IsBufferFull
		{
			get
			{
				return IsLast && ((piecesInNode.ContainsKey(CurrentRequiredTypeID) && piecesInNode[CurrentRequiredTypeID].Count >= tubeSettings.BufferSize) || (piecesInNode.ContainsKey(CurrentRequiredTypeID1) && piecesInNode[CurrentRequiredTypeID1].Count >= tubeSettings.BufferSize));
			}
		}

		public int CurrentRequiredTypeID
		{
			get
			{
				return currentRequiredTypeID;
			}
			set
			{
                currentRequiredTypeID = value;
			}
		}

        public int CurrentRequiredTypeID1
        {
            get
            {
                return currentRequiredTypeID1;
            }
            set
            {
                currentRequiredTypeID1 = value;
            }
        }

        private int CurrentPiecesAmount
		{
			get
			{
				return currentPiecesAmount;
			}
			set
			{
				currentPiecesAmount = value;
				//if (!gauge.IsNull())
				//{
				//	gauge.CurrentCount = value;
				//}
			}
		}

		public int MaxAllowedPieces
		{
			set
			{
				maxAllowedPieces = value;
				ReleasePiecesIfNeeded();
			}
		}

		private bool ReleasePiecesIfNeeded()
		{
			if (piecesInNode == null)
			{
				piecesInNode = new Dictionary<int, List<Piece>>();
			}
			bool flag = false;
			float num = 0f;
			if (IsLast && (piecesInNode.ContainsKey(CurrentRequiredTypeID) || piecesInNode.ContainsKey(CurrentRequiredTypeID1)))
			{
                if (piecesInNode.ContainsKey(CurrentRequiredTypeID))
                {
                    foreach (Piece p in piecesInNode[CurrentRequiredTypeID].Take(maxAllowedPieces).ToList<Piece>())
                    {
                        flag = true;
                        ReleasePiece(p, num);
                        num += 0.025f;
                    }
                }
                if (piecesInNode.ContainsKey(CurrentRequiredTypeID1))
                {
                    foreach (Piece p in piecesInNode[CurrentRequiredTypeID1].Take(maxAllowedPieces).ToList<Piece>())
                    {
                        flag = true;
                        ReleasePiece(p, num);
                        num += 0.025f;
                    }
                }
                else
                {

                }
                
                AudioSourceListener sound = soundSettings.GetAudioSource(soundType);
				if (!sound || !flag)
				{
					return flag;
				}
				if (sound.IsPlaying)
				{
					if (stopSoundDelayedRoutine != null)
					{
						StopCoroutine(stopSoundDelayedRoutine);
					}
				}
				else
				{
					sound.Play(0.15f, false);
				}
				stopSoundDelayedRoutine = StartCoroutine(CoroutineUtils.Delay(num + 0.5f, delegate()
				{
					sound.Stop(0.25f, null);
				}));
			}
			return flag;
		}

		private bool IsAllowed(Piece piece)
		{
			return !IsRestrictedByType || allowedPieceTypeIDs.Contains(piece.TypeID);
		}

		private void Awake()
		{
			Transform = transform;
			speedMultiplier = new Reference<float>(1f);
			//if (!gauge.IsNull())
			//{
			//	gauge.MaxCount = tubeSettings.BufferSize;
			//}
			allowedPieceTypeIDs = new List<int>();
			piecesInNode = new Dictionary<int, List<Piece>>();
		}

		private void AddPieceDict(Piece p)
		{
			int num = CurrentPiecesAmount;
			CurrentPiecesAmount = num + 1;
			if (piecesInNode.ContainsKey(p.TypeID))
			{
				piecesInNode[p.TypeID].Add(p);
			}
			else
			{
				piecesInNode.Add(p.TypeID, new List<Piece>
				{
					p
				});
			}
			//if (gauge.IsNull())
			//{
			//	return;
			//}
			//gauge.CurrentCount = piecesInNode.SelectMany((KeyValuePair<int, List<Piece>> kvp) => kvp.Value).Count<Piece>();
			//if (!gauge.IsNull())
			//{
			//	Material material = piecesInNode.First<KeyValuePair<int, List<Piece>>>().Value[0].Material;
			//	gauge.Color = (material.color / material.color.SetAlpha(0f).ToArray().Max()).SetAlpha(1f);
			//}
		}

		private void RemovePieceDict(Piece p)
		{
			int num = CurrentPiecesAmount;
			CurrentPiecesAmount = num - 1;
			piecesInNode[p.TypeID].Remove(p);
			if (piecesInNode[p.TypeID].Count == 0)
			{
				piecesInNode.Remove(p.TypeID);
			}
			Action bufferStateChange = BufferStateChange;
			if (bufferStateChange == null)
			{
				return;
			}
			bufferStateChange();
		}

		public void RemoveAllowedPieceTypeID(int id)
		{
			allowedPieceTypeIDs.Remove(id);
			IsRestrictedByType = (allowedPieceTypeIDs.Count > 0);
		}

		public void AddAllowedPieceTypeID(int id)
		{
			IsRestrictedByType = true;
			allowedPieceTypeIDs.Add(id);
		}

		private void ReleasePiece(Piece p, float delay = 0f)
		{
			RemovePieceDict(p);
			maxAllowedPieces--;
			StartCoroutine(CoroutineUtils.Delay(delay, delegate()
			{
                liquidGenerator.TapOpen(p.name);
                p.Rigidbody.isKinematic = true;
                p.transform.localScale = Vector3.zero;
                p.GetComponent<Collider>().enabled = false;
                //p.Rigidbody.isKinematic = false;
                //p.Rigidbody.velocity = Vector3.zero;
                //p.gameObject.layer = Piece.SmallestPiecesLayer;
                //p.gameObject.GetComponent<Renderer>().enabled = false;
                if (gameObject.name == "J5")
                {
                    Game.Smelters.SmeltersController.Instance.smelters[0].EnterSmelter(p);
                }
                else if(gameObject.name == "J7")
                {
                    Game.Smelters.SmeltersController.Instance.smelters[1].EnterSmelter(p);
                }
			
			}));
		}

		public void AddPiece(Piece piece)
		{
			int typeID = piece.TypeID;
			if (piecesInNode.ContainsKey(typeID) && piecesInNode[typeID].Contains(piece))
			{
				return;
			}
			AddPieceDict(piece);
			piece.Rigidbody.isKinematic = true;
			piece.tag = Tag.Untagged;
			piece.gameObject.layer = LayerMask.NameToLayer("Default");

            if(gameObject.name == "J5" || gameObject.name == "J7")
            {
                piece.transform.localScale = Vector3.zero;
                //piece.transform.GetComponent<Renderer>().enabled = false;
            }

            ReleasePiecesIfNeeded();
			if (!piecesInNode.ContainsKey(typeID) || !piecesInNode[typeID].Contains(piece))
			{
				return;
			}
			TubeNode nextNode = GetNextNode(piece);
			if (nextNode != null)
			{
				MovePieceToNextNode(nextNode, piece);
			}
            
            Action bufferStateChange = BufferStateChange;
			if (bufferStateChange == null)
			{
				return;
			}
			bufferStateChange();
		}

		private TubeNode GetNextNode(Piece piece)
		{
			List<TubeNode> list = (from n in nextNodes
			where n.IsAllowed(piece)
			select n).ToList<TubeNode>();
			if (list.Count != 0)
			{
				return list.Random<TubeNode>();
			}
			return null;
		}

		private void MovePieceToNextNode(TubeNode node, Piece piece)
		{
			Vector3 vector = new Vector3(node.Position.x, node.Position.y, 0f);
			float magnitude = (new Vector3(piece.Position.x, piece.Position.y, 0f) - vector).magnitude;
			piece.transform.MovePosition(vector, magnitude / tubeSettings.PiecesSpeedInTube, delegate()
			{
				RemovePieceDict(piece);
				node.AddPiece(piece);
			}, speedMultiplier);
		}

		void ISuspendable.Suspend()
		{
			if (changeSpeedMultiplierRoutine != null)
			{
				StopCoroutine(changeSpeedMultiplierRoutine);
			}
			changeSpeedMultiplierRoutine = StartCoroutine(CoroutineUtils.LerpFloat(speedMultiplier, 0f, 0.5f));
		}

		void ISuspendable.Resume()
		{
			if (changeSpeedMultiplierRoutine != null)
			{
				StopCoroutine(changeSpeedMultiplierRoutine);
			}
			changeSpeedMultiplierRoutine = StartCoroutine(CoroutineUtils.LerpFloat(speedMultiplier, 1f, 0.3f));
		}

	}
}
