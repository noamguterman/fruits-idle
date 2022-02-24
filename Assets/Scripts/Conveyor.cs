using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using Game.Audio;
using Game.GameScreen;
using Game.Pieces;
using Settings.UI.Tabs;
using Sirenix.OdinInspector;
using UI.ListItems;
using UI.ListItems.Upgrades.Piece;
using UIControllers;
using UnityEngine;
using Utilities;

[RequireComponent(typeof(SpriteRenderer))]
public class Conveyor : MonoBehaviour, IGameScreenComponent, ISuspendable
{
    [SerializeField]
    [Required]
    private RectTransform moneyPanel;

    [SerializeField]
    [Required]
    private Sprite dividerSprite;

    [SerializeField]
    [Required]
    private Camera mainCamera;

    [SerializeField]
    [Required]
    private BoxCollider conveyorCollider;

    [SerializeField]
    [Required]
    private MoneyManager moneyManager;

    [SerializeField]
    [Required]
    private GameScreensManager gameScreensManager;

    [SerializeField]
    private SoundType soundType;

    [SerializeField]
    [Required]
    [SceneObjectsOnly]
    private Box box;

    [SerializeField]
    private float destructionZoneOffsetFromScreenEdge = 1f;

    [SerializeField]
    private float distanceBetweenDividers = 0.5f;

    [SerializeField]
    private float dividersOffsetFromTop = 0.05f;

    [SerializeField]
    private float boxOffsetFromConveyour;

    [SerializeField]
    private float leftOffset;

    [SerializeField]
    private float defaultSpeed;

    [SerializeField]
    private float maxSpeed;

    [SerializeField]
    private int piecesAmountAfterWhichAccelerate;

    [SerializeField]
    private float accelerationSpeed;

    [SerializeField]
    private float slowingDownSpeed;

    [SerializeField]
    private bool isLast;

    [SerializeField]
    private bool playSound;

    //[FoldoutGroup("Settings", 0)]
    [SerializeField]
    [Required]
    private BoostersSettings boostersSettings;

    //[FoldoutGroup("Settings", 0)]
    [SerializeField]
    [Required]
    private BonusView bonusView;

    //[FoldoutGroup("Settings", 0)]
    [SerializeField]
    [Required]
    private SoundSettings soundSettings;

    //[FoldoutGroup("Settings", 0)]
    [SerializeField]
    [Required]
    private UpgradeSettings upgradeSettings;

    private AudioSourceListener conveyorWorkCycleAudio;

    private CollisionEvent destroyingPiecesZone;

    private CollisionEvent conveyorCollision;

    private SpriteRenderer tiledSpriteRenderer;

    private List<SpriteRenderer> dividers;

    private HashSet<Rigidbody> rOnConveyor;

    private float currentSpeed;

    private Reference<float> speedMultiplier;

    private Coroutine changeMultiplierRoutine;

    private bool isSuspended;

    private Sound conveyorWorkCycleAudioSetting;

    private Sound oreHitConveyorSound;

    public Bounds GameScreenBounds { get; set; }

	private float ScreenWidthInUnits
	{
		get
		{
			return mainCamera.Bounds().size.x;
		}
	}

	private static int PiecesOnConveyorLayer
	{
		get
		{
			return LayerMask.NameToLayer("Pieces On Conveyor");
		}
	}

	private void Awake()
	{
		speedMultiplier = new Reference<float>(1f);
		dividers = new List<SpriteRenderer>();
		rOnConveyor = new HashSet<Rigidbody>();
		conveyorCollision = ((conveyorCollider.gameObject == gameObject) ? null : conveyorCollider.gameObject.AddComponent<CollisionEvent>());
	}

	private void OnEnable()
	{
		if (conveyorCollision)
		{
			conveyorCollision.CollisionEnter += OnCollisionEnter;
		}
		if (!isLast)
		{
			upgradeSettings.GetUpgradeListItem(UpgradeType.SmeltersUnlock).LevelIncreased += OnSmeltersScreenUnlock;
		}
	}

	private void OnDisable()
	{
		if (conveyorCollision)
		{
			conveyorCollision.CollisionEnter -= OnCollisionEnter;
		}
		if (!isLast)
		{
			upgradeSettings.GetUpgradeListItem(UpgradeType.SmeltersUnlock).LevelIncreased -= OnSmeltersScreenUnlock;
		}
	}

	private IEnumerator Start()
	{
		conveyorWorkCycleAudio = soundSettings.GetAudioSource(soundType);
		if (playSound)
		{
			AudioSourceListener audioSourceListener = conveyorWorkCycleAudio;
			if (audioSourceListener != null)
			{
				audioSourceListener.Play(true);
			}
		}
		conveyorWorkCycleAudioSetting = soundSettings.GetSound(soundType);
		oreHitConveyorSound = soundSettings.GetSound(SoundType.OreHitConveyor);
		yield return null;
		bool flag = isLast || upgradeSettings.GetCurrentUpgrade(UpgradeType.SmeltersUnlock).Level == 0;
		tiledSpriteRenderer = base.GetComponent<SpriteRenderer>();
		tiledSpriteRenderer.size = new Vector2(ScreenWidthInUnits + (isLast ? gameScreensManager.IntermediateZoneWidth : 0f), tiledSpriteRenderer.size.y);
		currentSpeed = defaultSpeed;

        Vector3 vector = new Vector3(GameScreenBounds.center.x, mainCamera.ScreenToWorldPoint(moneyPanel.position).y, mainCamera.ScreenToWorldPoint(moneyPanel.position).z);
        transform.position = vector + new Vector3(-(isLast ? (gameScreensManager.IntermediateZoneWidth / 2f) : 0f), tiledSpriteRenderer.size.y / 2f, 5f);
        
        conveyorCollider.size = new Vector3(GameScreenBounds.size.x + (flag ? destructionZoneOffsetFromScreenEdge : 0f) + leftOffset + (isLast ? gameScreensManager.IntermediateZoneWidth : 0f), conveyorCollider.size.y, conveyorCollider.size.z);
		conveyorCollider.center += Vector3.right * (((flag ? destructionZoneOffsetFromScreenEdge : 0f) - leftOffset) / 2f);
		conveyorCollider.transform.SetLocalPositionX(0f);
		conveyorCollider.transform.SetPositionZ(1);
		if (flag)
		{
			destroyingPiecesZone = new GameObject("Destruction Zone", new Type[]
			{
				typeof(BoxCollider)
			}).AddComponent<CollisionEvent>();
			BoxCollider component = destroyingPiecesZone.GetComponent<BoxCollider>();
			component.isTrigger = true;
			component.size = new Vector3(10f, 10f, 10f);
			destroyingPiecesZone.transform.SetParent(transform);
			destroyingPiecesZone.gameObject.layer = gameObject.layer;
			destroyingPiecesZone.TriggerEnter += OnPieceEnteredDestructionZone;
			destroyingPiecesZone.transform.position = new Vector3
			{
				x = GameScreenBounds.max.x + component.size.x / 2f + destructionZoneOffsetFromScreenEdge,
				y = base.transform.position.y
			};
		}
		//box.Position = new Vector3(GameScreenBounds.max.x - box.Bounds.extents.x, box.transform.position.y, -5f);
		box.Initialize(flag);
        //Debug.Log(vector + " " + ScreenWidthInUnits + "  " + gameScreensManager.IntermediateZoneWidth + "    " + tiledSpriteRenderer.size + "    " + dividersOffsetFromTop);

        base.StartCoroutine(DividersMovement(new Vector2(vector.x, vector.y) - new Vector2(ScreenWidthInUnits / 2f + (isLast ? gameScreensManager.IntermediateZoneWidth : 0f), -tiledSpriteRenderer.size.y / 2f + dividersOffsetFromTop), new Vector2(vector.x, vector.y) + new Vector2(ScreenWidthInUnits / 2f + (isLast ? (gameScreensManager.IntermediateZoneWidth / 2f) : 0f), 0f)));
		yield break;
	}

	private IEnumerator DividersMovement(Vector2 startPos, Vector2 endPos)
	{
		float travelDistance = (startPos - endPos).magnitude + dividerSprite.bounds.size.x;
		int num = (int)(travelDistance / distanceBetweenDividers) + 1;
		for (int i = 0; i < num; i++)
		{
			SpriteRenderer spriteRenderer = new GameObject().AddComponent<SpriteRenderer>();
			spriteRenderer.name = "Divider";
			spriteRenderer.sprite = dividerSprite;
			spriteRenderer.sortingLayerID = tiledSpriteRenderer.sortingLayerID;
			spriteRenderer.sortingOrder = tiledSpriteRenderer.sortingOrder + 1;
			spriteRenderer.maskInteraction = SpriteMaskInteraction.None;
			Transform transform = spriteRenderer.transform;
			transform.SetParent(base.transform);
			Bounds bounds = spriteRenderer.bounds;
			transform.position = new Vector3(startPos.x + distanceBetweenDividers * i - bounds.size.x / 2f, startPos.y + bounds.size.y / 4f, 5f);
			dividers.Add(spriteRenderer);
		}
		while (true)
		{
			foreach (SpriteRenderer spriteRenderer2 in dividers)
			{
				Transform transform2 = spriteRenderer2.transform;
				transform2.position += Vector3.right * currentSpeed * Time.deltaTime;
				if (transform2.position.x > endPos.x + spriteRenderer2.bounds.size.x / 2f)
				{
					transform2.position = spriteRenderer2.transform.position - new Vector3(travelDistance, 0f);
				}
			}
			yield return null;
		}
	}

	public void OnSmallestPieceTouchedConveyorPiece(Piece piece)
	{
		if (piece == null)
		{
			return;
		}
		piece.gameObject.layer = PiecesOnConveyorLayer;
	}

	private void OnCollisionEnter(Collision other)
	{
		Rigidbody component = other.gameObject.GetComponent<Rigidbody>();
		if (component == null)
		{
			return;
		}
		if (component.gameObject.layer != PiecesOnConveyorLayer)
		{
			component.gameObject.layer = PiecesOnConveyorLayer;
			rOnConveyor.Add(component);
		}
		else if (!rOnConveyor.Contains(component))
		{
			rOnConveyor.Add(component);
		}
		ISoundable component2 = other.gameObject.GetComponent<ISoundable>();
		if (component2 == null)
		{
			return;
		}
		AudioSourceListener audioSource = soundSettings.GetAudioSource(component2.SoundType);
		if (audioSource)
		{
			if (oreHitConveyorSound.UsePitchRange)
			{
				audioSource.Pitch = UnityEngine.Random.Range(oreHitConveyorSound.MinPitch, oreHitConveyorSound.MaxPitch);
			}
			audioSource.PlayOneShot();
		}
	}

	private void FixedUpdate()
	{
		RecalculateSpeed();
		foreach (Rigidbody rigidbody in new List<Rigidbody>(rOnConveyor))
		{
			rigidbody.velocity = new Vector3(currentSpeed, rigidbody.velocity.y);
		}
	}

	private void RecalculateSpeed()
	{
		rOnConveyor.RemoveWhere((Rigidbody p) => !p.gameObject.activeSelf || p.isKinematic);
		if (rOnConveyor.Count > piecesAmountAfterWhichAccelerate)
		{
			currentSpeed += Mathf.Min(accelerationSpeed * Time.deltaTime, maxSpeed);
		}
		else if (currentSpeed > defaultSpeed)
		{
			currentSpeed = Mathf.Max(currentSpeed - slowingDownSpeed * Time.deltaTime, defaultSpeed);
		}
		currentSpeed *= speedMultiplier.Value;
		float minPitch = conveyorWorkCycleAudioSetting.MinPitch;
		float maxPitch = conveyorWorkCycleAudioSetting.MaxPitch;
		conveyorWorkCycleAudio.Pitch = Mathf.Lerp(minPitch, maxPitch, (currentSpeed - defaultSpeed) / (maxSpeed - defaultSpeed));
		if (currentSpeed <= 0f)
		{
			if (conveyorWorkCycleAudio.IsPlaying)
			{
				conveyorWorkCycleAudio.Stop(0.15f, null);
				return;
			}
		}
		else if (!conveyorWorkCycleAudio.IsPlaying)
		{
			conveyorWorkCycleAudio.Play(0.15f, false);
		}
	}

	private void OnSmeltersScreenUnlock()
	{
		if (destroyingPiecesZone != null)
		{
			Destroy(destroyingPiecesZone.gameObject);
		}
		box.HideTube();
	}

	private void OnPieceEnteredDestructionZone(Collider collider)
	{
		Costable component = collider.GetComponent<Costable>();
		if (component == null)
		{
			return;
		}
		rOnConveyor.Remove(component.Rigidbody);
		box.AddMoney(moneyManager.GetCost(component));
		component.Push(true);
	}

	public void Suspend()
	{
		if (isLast || isSuspended)
		{
			return;
		}
		isSuspended = true;
		if (changeMultiplierRoutine != null)
		{
			StopCoroutine(changeMultiplierRoutine);
		}
		changeMultiplierRoutine = StartCoroutine(CoroutineUtils.LerpFloat(speedMultiplier, 0f, 0.15f));
	}

	public void Resume()
	{
		if (isLast || !isSuspended)
		{
			return;
		}
		isSuspended = false;
		if (changeMultiplierRoutine != null)
		{
			StopCoroutine(changeMultiplierRoutine);
		}
		changeMultiplierRoutine = StartCoroutine(CoroutineUtils.LerpFloat(speedMultiplier, 1f, 0.15f));
	}

}
