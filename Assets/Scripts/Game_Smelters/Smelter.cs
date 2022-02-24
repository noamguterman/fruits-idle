using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Audio;
using Game.Pieces;
using Game.Tube;
using Settings.UI.Tabs;
using Sirenix.OdinInspector;
using UI.ListItems;
using UI.ListItems.Upgrades.Piece.Smelter;
using UnityEngine;
using Utilities;
using Utilities.AlphaAnimation;

namespace Game.Smelters
{
	public class Smelter : MonoBehaviour
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private CollisionEvent topCollider;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private GismoCtrl smelterWheel;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [AssetsOnly]
        private Bar barPrefab;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private TubeNode tubeNode;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private TubeNode tubeNodeRestrictedByType;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private MoneyManager moneyManager;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private SmelterUpgrades smelterUpgrades;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private SoundSettings soundSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private SmeltersSettings smeltersSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private SoundType soundType;

        private int piecesInSmelterAmount;

        private int requiredAmount;

        private int currentTypeID;

        private float currentCost;

        private float meltTimeMultiplier;

        private float speedMultiplier;

        private List<int> allowedPieceTypeIDs;

        private AudioSourceListener sound;

        private Material currentMaterial;

        private Coroutine currentMeltRoutine;

        private Reference<float> combinedMeltMultiplier;

        private bool IsMelting
		{
			get
			{
				return currentMeltRoutine != null;
			}
		}

		public float MeltTimeMultiplier
		{
			set
			{
				meltTimeMultiplier = value;
				SetCombinedMeltMultiplier();
			}
		}

		public float SpeedMultiplier
		{
			set
			{
				speedMultiplier = value;
				SetCombinedMeltMultiplier();
			}
		}

		public event Action<Smelter, int> PieceEnter;

		private int GetCurrentRequiredAmount()
		{
            return 18;// (int)smelterUpgrades.GetUpgrade(SmelterUpgradeType.FruitAmount).Value;
        }

		public bool HasTypeID(int typeID)
		{
			return allowedPieceTypeIDs.Contains(typeID);
		}

		private void SetCombinedMeltMultiplier()
		{
			if (combinedMeltMultiplier == null)
			{
				combinedMeltMultiplier = new Reference<float>(1f);
				speedMultiplier = 1f;
				meltTimeMultiplier = 1f;
			}
			combinedMeltMultiplier.Value = speedMultiplier * meltTimeMultiplier;
            //Debug.Log("++++++++"+ combinedMeltMultiplier.Value + "   " + speedMultiplier + "   " + meltTimeMultiplier);
		}

		public void AddAllowedPieceTypeID(int id)
		{
			allowedPieceTypeIDs.Add(id);
            tubeNode.CurrentRequiredTypeID = allowedPieceTypeIDs.First<int>();

            if(allowedPieceTypeIDs.Count > 1)
                tubeNode.CurrentRequiredTypeID1 = allowedPieceTypeIDs[1];

            tubeNodeRestrictedByType.AddAllowedPieceTypeID(id);
		}

		private void RemoveAllowedPieceTypeID(int id)
		{
			allowedPieceTypeIDs.Remove(id);
			if (allowedPieceTypeIDs.Count > 0)
			{
				tubeNode.CurrentRequiredTypeID = allowedPieceTypeIDs.First<int>();
			}
            if (allowedPieceTypeIDs.Count > 1)
            {
                tubeNode.CurrentRequiredTypeID1 = allowedPieceTypeIDs[1];
            }

            tubeNodeRestrictedByType.RemoveAllowedPieceTypeID(id);
		}

		private void Awake()
		{
			sound = soundSettings.GetAudioSource(soundType);
			tubeNodeRestrictedByType.IsRestrictedByType = true;
			allowedPieceTypeIDs = new List<int>();
			combinedMeltMultiplier = new Reference<float>(1f);
			speedMultiplier = 1f;
			meltTimeMultiplier = 1f;
			//topCollider.TriggerEnter += OnTriggerEnter;
		}

		private void OnEnable()
		{
			tubeNode.MaxAllowedPieces = (requiredAmount = GetCurrentRequiredAmount());
		}

		public void ForceMelting()
		{
			RemoveAllowedPieceTypeID(currentTypeID);
			if (!IsMelting)
			{
				Debug.LogError("Force Melting");
				tubeNode.MaxAllowedPieces = 0;
				Melt(LiquidColors.Instance.FruitName[currentTypeID]);
			}
		}

		private void Melt(string pieceName)
		{
            if (currentMeltRoutine != null)
			{
				StopCoroutine(currentMeltRoutine);
			}
			sound.Play(0.25f, true);
			currentMeltRoutine = StartCoroutine(CoroutineUtils.Delay(smelterUpgrades.GetUpgrade(SmelterUpgradeType.MixerTime).Value, combinedMeltMultiplier, delegate()
			{
                currentMeltRoutine = null;
				sound.Stop(0.25f, null);
				Bar bar = barPrefab.PullOrCreate<Bar>();
				moneyManager.Subscribe(bar);
				bar.Initialize(currentCost * smelterUpgrades.GetUpgrade(SmelterUpgradeType.BarCost).Value);
				bar.Position = new Vector3(transform.position.x, transform.position.y, 0f);

                bar.transform.localScale = Vector3.zero;
                bar.GetComponent<Rigidbody>().isKinematic = true;
                bar.GetComponent<Collider>().enabled = false;
				//bar.Rotation = Quaternion.Euler(0f, -16.5f, 0f);
				piecesInSmelterAmount = 0;
				tubeNode.MaxAllowedPieces = (requiredAmount = GetCurrentRequiredAmount());
                smelterWheel.TurnOffLight();

                transform.parent.parent.GetComponent<Conveyor1>().BarAction(bar, gameObject, (smelterUpgrades.GetUpgrade(SmelterUpgradeType.MixerTime).Value / combinedMeltMultiplier.Value), LiquidColors.Instance.GetLiquidColor(pieceName));
            }));
		}

        public void EnterSmelter(Piece component)
        {
            StartCoroutine(Cor_EntereSmelter(0, component));
        }

        IEnumerator Cor_EntereSmelter(float time, Piece component)
        {
            yield return new WaitForSeconds(time);

            if (piecesInSmelterAmount == 0)
            {
                currentTypeID = component.TypeID;
                currentMaterial = component.Material;
                currentCost = component.InitalCost;
            }
            int typeID = component.TypeID;
            component.Push(false);

            Action<Smelter, int> pieceEnter = PieceEnter;
            pieceEnter(this, typeID);
            if (!IsMelting)
            {
                piecesInSmelterAmount++;
                smelterWheel.TurnOnLight(piecesInSmelterAmount, requiredAmount);
                if (piecesInSmelterAmount >= requiredAmount)
                {
                    Melt(component.name);
                }
            }
        }

		//private void OnTriggerEnter(Collider other)
		//{
		//	Piece component = other.GetComponent<Piece>();
		//	if (component == null)
		//	{
		//		return;
		//	}
		//	if (piecesInSmelterAmount == 0)
		//	{
		//		currentTypeID = component.TypeID;
		//		currentMaterial = component.Material;
		//		currentCost = component.InitalCost;
		//	}
		//	int typeID = component.TypeID;
		//	component.Push(false);

  //          Action<Smelter, int> pieceEnter = PieceEnter;
		//	pieceEnter(this, typeID);
  //          if (IsMelting)
		//	{
		//		return;
		//	}
  //          piecesInSmelterAmount++;
  //          smelterWheel.TurnOnLight(piecesInSmelterAmount, requiredAmount);
		//	if (piecesInSmelterAmount >= requiredAmount)
		//	{
		//		Melt();
		//	}
		//}

	}
}
