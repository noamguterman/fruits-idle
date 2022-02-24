using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Audio;
using Game.Press;
using Settings.UI.Tabs;
using Sirenix.OdinInspector;
using UI.ListItems;
using UI.ListItems.Booster;
using UI.ListItems.Upgrades.Piece;
using UIControllers;
using UnityEngine;
using Utilities;
using Utilities.AlphaAnimation;

namespace Game.Pieces
{
	public class PiecesController : MonoBehaviour, ISuspendable
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private PressManager pressManager;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private FunnelController funnelController;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private BonusView bonusView;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private Conveyor conveyor;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private MoneyManager moneyManager;

        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private Transform leftSpawnPosition;

        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private Transform rightSpawnPosition;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private GameSettings gameSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private PieceSettings pieceSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private UpgradeSettings upgradeSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private BoostersSettings boostersSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [Required]
        private SoundSettings soundSettings;

        [SerializeField]
        [Required]
        private GameObject fruitExplode;

        private HashSet<Piece> currentActivePieces;

        private HashSet<Piece> currentActivePiecesUnderPressure;

        private List<PieceListItem> activePieceTypes;

        private float bonusBallSpawnSpeedMultiplier;

        private float ballSpawnSpeedMultiplier;

        private bool isSuspended;

        private Sound pieceBreakSound;

        private IEnumerator<KeyValuePair<Transform, Reference<bool>>> ienumSpawnPos;

        private IEnumerator<PieceListItem> ienumActivePieceTypes;

        private Dictionary<Transform, Reference<bool>> ballSpawnPositions;

        private bool shouldReleaseOre;

        private readonly float waitTime = 60f;

        public List<PieceListItem> ActivePieceTypes
		{
			get
			{
				return this.activePieceTypes;
			}
		}

		public event Action<IReadOnlyList<PieceListItem>> ActivePieceTypesChange;

		public int CountPieces(int typeID)
		{
			return this.currentActivePieces.Count((Piece p) => p.TypeID == typeID && !p.IsRare);
		}

		private bool RareOreReleased
		{
			get
			{
				return bool.Parse(PlayerPrefs.GetString("RareOreReleased", "false"));
			}
			set
			{
				PlayerPrefs.GetString("RareOreReleased", value.ToString());
			}
		}

		private void Awake()
		{
			if (!this.RareOreReleased)
			{
				base.StartCoroutine(CoroutineUtils.Delay(this.waitTime, delegate()
				{
					this.shouldReleaseOre = true;
				}));
			}
			this.currentActivePieces = new HashSet<Piece>();
			this.currentActivePiecesUnderPressure = new HashSet<Piece>();
			this.activePieceTypes = new List<PieceListItem>();
			this.bonusBallSpawnSpeedMultiplier = 1f;
			this.ballSpawnSpeedMultiplier = 1f;
			List<Piece> list = new List<Piece>();
			list.AddRange(from u in this.upgradeSettings.PiecesUpgrades
			select u.Piece);
			list.AddRange(from r in this.upgradeSettings.RarePieceUpgrades
			select r.Piece);
			list.ForEach(delegate(Piece p)
			{
				p.HealthMultiplier = 1f;
				float lastPieceCost = 1f / (float)p.GetLastPiecesAmount();
				List<Piece> list2 = new List<Piece>();
				p.GetChildren(ref list2);
				list2.ForEach(delegate(Piece child)
				{
					child.HealthMultiplier = lastPieceCost * (float)child.GetLastPiecesAmount();
				});
			});
			this.OnGravityLevelIncrease();
			this.pieceBreakSound = this.soundSettings.GetSound(SoundType.OreBreak);
		}

		private void OnEnable()
		{
			this.upgradeSettings.AllUpgrades.First((UpgradeTypeListItem u) => u.UpgradeType == UpgradeType.Gravity).LevelIncreased += this.OnGravityLevelIncrease;
			this.upgradeSettings.PiecesUpgrades.ToList<PieceListItem>().ForEach(delegate(PieceListItem u)
			{
				u.LevelIncreased += this.OnPieceLevelIncrease;
			});
			this.pressManager.PressCrush += this.OnPressStartToCrush;
			this.bonusView.BonusStateChange += this.OnBonusStateChange;
			this.boostersSettings.ChangeBoosterTypeMutiplier += this.OnBoosterMutiplierChanged;
		}

		private void OnDisable()
		{
			this.upgradeSettings.AllUpgrades.First((UpgradeTypeListItem u) => u.UpgradeType == UpgradeType.Gravity).LevelIncreased -= this.OnGravityLevelIncrease;
			this.upgradeSettings.PiecesUpgrades.ToList<PieceListItem>().ForEach(delegate(PieceListItem u)
			{
				u.LevelIncreased -= this.OnPieceLevelIncrease;
			});
			this.pressManager.PressCrush -= this.OnPressStartToCrush;
			this.bonusView.BonusStateChange -= this.OnBonusStateChange;
			this.boostersSettings.ChangeBoosterTypeMutiplier -= this.OnBoosterMutiplierChanged;
		}

		private IEnumerator Start()
		{
			this.ballSpawnPositions = new Dictionary<Transform, Reference<bool>>
			{
				{
					this.leftSpawnPosition,
					this.funnelController.IsLeftTouching
				},
				{
					this.rightSpawnPosition,
					this.funnelController.IsRightTouching
				}
			};
			if (this.ballSpawnPositions.Count == 0)
			{
				yield break;
			}
			this.ienumSpawnPos = this.ballSpawnPositions.GetEnumerator();
			this.OnPieceLevelIncrease();
			while (base.enabled)
			{
				if (this.isSuspended)
				{
					yield return new WaitUntil(() => !this.isSuspended);
				}
				this.ienumActivePieceTypes.MoveNextCycled<PieceListItem>();
				this.ienumSpawnPos.MoveNextCycled<KeyValuePair<Transform, Reference<bool>>>();
				KeyValuePair<Transform, Reference<bool>> keyValuePair = this.ienumSpawnPos.Current;
				if (!(keyValuePair.Key == null))
				{
					KeyValuePair<Transform, Reference<bool>> keyValuePair2 = this.ballSpawnPositions.Random<Transform, Reference<bool>>();
					if (!keyValuePair2.Value.Value)
					{
						Piece piece = this.SetPieceType(this.ienumActivePieceTypes.Current, this.activePieceTypes);
						piece.Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
						piece.Position = keyValuePair2.Key.position;
						piece.Rotation = Quaternion.Euler(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f));
                        //Debug.Log("------------");
                    }
                    //Debug.Log("@@@@@@@@@@@@@");
					yield return new WaitForSeconds(this.upgradeSettings.GetCurrentUpgrade(UpgradeType.PieceSpawnInterval).Value / this.ballSpawnSpeedMultiplier / this.bonusBallSpawnSpeedMultiplier);
				}
			}
			this.ienumSpawnPos.Dispose();
			yield break;
		}

		private Piece SetPieceType(PieceListItem pieceType, List<PieceListItem> activePieceTypes)
		{

			foreach (RarePieceListItem rarePieceListItem in from p in this.upgradeSettings.RarePieceUpgrades
			orderby p.CurrentLevelUpgrade.Value descending
			select p)
			{
				if (this.shouldReleaseOre)
				{
					float num = activePieceTypes.Max((PieceListItem t) => t.CurrentLevelUpgrade.Value);
					if (this.shouldReleaseOre)
					{
						this.RareOreReleased = true;
						this.shouldReleaseOre = false;
					}
					Piece piece = this.CreatePiece(rarePieceListItem.Piece, rarePieceListItem.PieceMaterial, rarePieceListItem.TypeID, rarePieceListItem.MoneyType, true);
					piece.Cost = rarePieceListItem.PieceCost;
					piece.Health = num * rarePieceListItem.PieceHealthMultiplier;
					piece.InitialHealth = piece.Health;
					return piece;
				}
                else if(GameData.CurrentLevel < 15 && UnityEngine.Random.value < 0.4f / 100f)
                {
                    float num = activePieceTypes.Max((PieceListItem t) => t.CurrentLevelUpgrade.Value);

                    Piece piece = this.CreatePiece(rarePieceListItem.Piece, rarePieceListItem.PieceMaterial, rarePieceListItem.TypeID, rarePieceListItem.MoneyType, true);
                    piece.Cost = rarePieceListItem.PieceCost;
                    piece.Health = num * rarePieceListItem.PieceHealthMultiplier;
                    piece.InitialHealth = piece.Health;
                    return piece;
                }
                else if(UnityEngine.Random.value < rarePieceListItem.CurrentLevelUpgrade.Value / 100f)
                {
                    float num = activePieceTypes.Max((PieceListItem t) => t.CurrentLevelUpgrade.Value);
                    
                    Piece piece = this.CreatePiece(rarePieceListItem.Piece, rarePieceListItem.PieceMaterial, rarePieceListItem.TypeID, rarePieceListItem.MoneyType, true);
                    piece.Cost = rarePieceListItem.PieceCost;
                    piece.Health = num * rarePieceListItem.PieceHealthMultiplier;
                    piece.InitialHealth = piece.Health;
                    return piece;
                }
			}
			Piece piece2 = this.CreatePiece(pieceType.Piece, pieceType.PieceMaterial, pieceType.TypeID, pieceType.MoneyType, false);
			piece2.Cost = pieceType.CurrentLevelUpgrade.Value;
			piece2.InitialHealth = piece2.Health;
			return piece2;
		}

		private Piece CreatePiece(Piece piecePrefab, Material pieceMaterial, int typeID, MoneyType mType, bool isRare = false)
		{
			Piece piece = piecePrefab.PullOrCreate<Piece>();
			this.moneyManager.Subscribe(piece);
			piece.CrossedPressureZone += this.OnPieceCrossedPressureZone;
			piece.SmallestPieceTouchedConveyorPiece += this.conveyor.OnSmallestPieceTouchedConveyorPiece;
			piece.PieceDestroyed += delegate(Piece p)
			{
				this.currentActivePieces.Remove(p);
			};
			piece.IsRare = isRare;
			piece.TypeID = typeID;
			piece.HealthMultiplier = piecePrefab.HealthMultiplier;
			piece.Parent = piecePrefab.Parent;
			//piece.Material = pieceMaterial;
			piece.Rigidbody.velocity = Vector3.zero;
			piece.MoneyType = mType;
			this.currentActivePieces.Add(piece);
			return piece;
		}

		private void CrushPiece(Piece piece, float force)
		{
			piece.Health = Mathf.Max(piece.Health - force, 0f);
			bool flag = this.BreakPiece(piece, null);
			if (flag)
			{
				AudioSourceListener audioSource = this.soundSettings.GetAudioSource(SoundType.OreBreak);
				if (audioSource)
				{
					if (this.pieceBreakSound.UsePitchRange)
					{
						audioSource.Pitch = UnityEngine.Random.Range(this.pieceBreakSound.MinPitch, this.pieceBreakSound.MaxPitch);
					}
					audioSource.Play(false);
				}
			}
			if (flag || piece.Health <= 0f)
			{
				piece.Push(piece.IsRare && !piece.IsBreakable);
				this.currentActivePiecesUnderPressure.Remove(piece);
			}
		}

		private bool BreakPiece(Piece currentPiece, Piece initialPiece = null)
		{
			if (initialPiece == null)
			{
				initialPiece = currentPiece;
			}
			bool result = false;
			if (currentPiece != initialPiece && currentPiece.ShouldCreateBreakPart(initialPiece.InitalCost, initialPiece.Health))
			{
				Piece piece = this.CreatePiece(currentPiece, initialPiece.Material, initialPiece.TypeID, initialPiece.MoneyType, initialPiece.IsRare);
				piece.Position = initialPiece.Position;
				piece.Rotation = initialPiece.Rotation;
				piece.transform.localScale = initialPiece.transform.localScale;
				piece.Cost = initialPiece.Cost / initialPiece.HealthMultiplier * currentPiece.HealthMultiplier;
				piece.InitialHealth = piece.Health;
				piece.Health = initialPiece.Health;
                GameObject fruitExplodeObj = Instantiate(fruitExplode, piece.transform.position, Quaternion.identity);
                Color col = LiquidColors.Instance.GetLiquidColor(piece.name);
                fruitExplodeObj.GetComponent<Renderer>().material.color = col;
                Destroy(fruitExplodeObj, 1);

				if (piece.IsBreakable)
				{
					piece.Rigidbody.AddExplosionForce(1000f, initialPiece.Position, 10f);
				}
				else
				{
					GameObject gameObject = piece.gameObject;
					gameObject.tag = "Smallest Pieces";
					gameObject.layer = Piece.SmallestPiecesLayer;
                    piece.Rigidbody.isKinematic = false;
                    piece.GetComponent<Collider>().enabled = true;
                    piece.Rigidbody.AddForce(((initialPiece.Position.x > this.pressManager.transform.position.x) ? Vector2.right : Vector2.left) * this.pieceSettings.OnPieceCreateForce.x + Vector2.up * this.pieceSettings.OnPieceCreateForce.y);
                }
                return true;
			}
			foreach (Piece currentPiece2 in currentPiece.BreakPieces)
			{
				if (this.BreakPiece(currentPiece2, initialPiece))
				{
					result = true;
				}
			}
			return result;
		}

		private void OnPieceCrossedPressureZone(Piece b, bool enterPressureZone)
		{
			if (enterPressureZone)
			{
				b.tag = Piece.BallToCrushTag;
				this.currentActivePiecesUnderPressure.Add(b);
				return;
			}
			b.tag = Tag.Untagged;
			this.currentActivePiecesUnderPressure.Remove(b);
		}

		private void OnBonusStateChange(Bonus b, bool isEnabled)
		{
			if (b.BonusType != BonusType.SpeedBoost)
			{
				return;
			}
			this.bonusBallSpawnSpeedMultiplier = (isEnabled ? b.SpawnAmountMultiplier : 1f);
		}

		private void OnBoosterMutiplierChanged(BoosterType boosterType, float multiplier, float time, bool isEnabled)
		{
			if (boosterType == BoosterType.GameSpeed)
			{
				this.ballSpawnSpeedMultiplier = multiplier;
				return;
			}
			if (boosterType != BoosterType.MultiplierIncome)
			{
				throw new ArgumentOutOfRangeException("boosterType", boosterType, null);
			}
		}

		private void OnGravityLevelIncrease()
		{
			Physics.gravity = Vector3.down * this.upgradeSettings.GetCurrentUpgrade(UpgradeType.Gravity).Value;
		}

		private void OnPieceLevelIncrease()
		{
			this.activePieceTypes.ForEach(delegate(PieceListItem t)
			{
				t.IsActiveInGame = false;
			});
			this.activePieceTypes = new List<PieceListItem>((from u in this.upgradeSettings.PiecesUpgrades
			where u.CurrentLevelUpgrade.Level > 0
			select u).Reverse<PieceListItem>().Take(this.pieceSettings.MaxPieceTypesSpawn));
			this.activePieceTypes.ForEach(delegate(PieceListItem t)
			{
				t.IsActiveInGame = true;
			});
			foreach (PieceListItem pieceListItem in this.upgradeSettings.PiecesUpgrades.Where((PieceListItem t, int i) => i < this.upgradeSettings.PiecesUpgrades.ToList<PieceListItem>().IndexOf(this.activePieceTypes.Last<PieceListItem>())))
			{
				pieceListItem.SetMaxLevel();
			}
			this.ienumActivePieceTypes = this.activePieceTypes.GetEnumerator();
			Action<IReadOnlyList<PieceListItem>> activePieceTypesChange = this.ActivePieceTypesChange;
			if (activePieceTypesChange == null)
			{
				return;
			}
			activePieceTypesChange(this.activePieceTypes);
		}

		private void OnPressStartToCrush(float force)
		{
			if (this.currentActivePiecesUnderPressure.Count <= 0)
			{
				AudioSourceListener audioSource = this.soundSettings.GetAudioSource(SoundType.PressCrush);
				Sound sound = this.soundSettings.GetSound(SoundType.PressCrush);
				if (audioSource)
				{
					if (sound.UsePitchRange)
					{
						audioSource.Pitch = UnityEngine.Random.Range(sound.MinPitch, sound.MaxPitch);
					}
					audioSource.Play(false);
				}
			}
			new List<Piece>(this.currentActivePiecesUnderPressure).ForEach(delegate(Piece p)
			{
				this.CrushPiece(p, force);
			});
		}

		public void Suspend()
		{
			this.isSuspended = true;
		}

		public void Resume()
		{
			this.isSuspended = false;
		}

	}
}
