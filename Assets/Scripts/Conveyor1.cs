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
public class Conveyor1 : MonoBehaviour, IGameScreenComponent, ISuspendable
{
    [SerializeField]
    [Required]
    private RectTransform moneyPanel;

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

    [SerializeField]
    [Required]
    private VenderTubeManager venderTube;

    [SerializeField]
    [Required]
    private VenderController venderControl;

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

    public GameObject[] pipes;
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
		//if (conveyorCollision)
		//{
		//	conveyorCollision.CollisionEnter += OnCollisionEnter;
		//}
		if (!isLast)
		{
			upgradeSettings.GetUpgradeListItem(UpgradeType.VenderUnlock).LevelIncreased += OnVendersScreenUnlock;
		}
	}

	private void OnDisable()
	{
		//if (conveyorCollision)
		//{
		//	conveyorCollision.CollisionEnter -= OnCollisionEnter;
		//}
		if (!isLast)
		{
			upgradeSettings.GetUpgradeListItem(UpgradeType.VenderUnlock).LevelIncreased -= OnVendersScreenUnlock;
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
		bool flag = isLast || upgradeSettings.GetCurrentUpgrade(UpgradeType.VenderUnlock).Level == 0;
		tiledSpriteRenderer = base.GetComponent<SpriteRenderer>();
		//tiledSpriteRenderer.size = new Vector2(ScreenWidthInUnits + (isLast ? gameScreensManager.IntermediateZoneWidth : 0f), tiledSpriteRenderer.size.y);
		currentSpeed = defaultSpeed;

        Vector3 vector = new Vector3(GameScreenBounds.center.x, mainCamera.ScreenToWorldPoint(moneyPanel.position).y, mainCamera.ScreenToWorldPoint(moneyPanel.position).z);
        transform.position = vector + new Vector3(-(isLast ? (gameScreensManager.IntermediateZoneWidth / 2f) : 0f), tiledSpriteRenderer.size.y / 2f, 5f);
        
  //      conveyorCollider.size = new Vector3(GameScreenBounds.size.x + (flag ? destructionZoneOffsetFromScreenEdge : 0f) + leftOffset + (isLast ? gameScreensManager.IntermediateZoneWidth : 0f), conveyorCollider.size.y, conveyorCollider.size.z);
		//conveyorCollider.center += Vector3.right * (((flag ? destructionZoneOffsetFromScreenEdge : 0f) - leftOffset) / 2f);
		transform.SetLocalPositionX(0f);
		transform.SetPositionZ(1);
		//if (flag)
		//{
		//	destroyingPiecesZone = new GameObject("Destruction Zone", new Type[]
		//	{
		//		typeof(BoxCollider)
		//	}).AddComponent<CollisionEvent>();
		//	BoxCollider component = destroyingPiecesZone.GetComponent<BoxCollider>();
		//	component.isTrigger = true;
		//	component.size = new Vector3(10f, 10f, 5f);
		//	destroyingPiecesZone.transform.SetParent(transform);
		//	destroyingPiecesZone.gameObject.layer = gameObject.layer;
		//	destroyingPiecesZone.TriggerEnter += OnPieceEnteredDestructionZone;
		//	destroyingPiecesZone.transform.position = new Vector3
		//	{
		//		x = GameScreenBounds.max.x + component.size.x / 2f + destructionZoneOffsetFromScreenEdge,
		//		y = base.transform.position.y
		//	};
		//}
		//box.Position = new Vector3(GameScreenBounds.max.x - box.Bounds.extents.x, box.transform.position.y, -5f);
		box.Initialize(flag);
        //Debug.Log(vector + " " + ScreenWidthInUnits + "  " + gameScreensManager.IntermediateZoneWidth + "    " + tiledSpriteRenderer.size + "    " + dividersOffsetFromTop);

		yield break;
	}

	public void OnSmallestPieceTouchedConveyorPiece(Piece piece)
	{
		if (piece == null)
		{
			return;
		}
		piece.gameObject.layer = PiecesOnConveyorLayer;
	}

	//private void OnCollisionEnter(Collision other)
	//{
	//	Rigidbody component = other.gameObject.GetComponent<Rigidbody>();
	//	if (component == null)
	//	{
	//		return;
	//	}
	//	if (component.gameObject.layer != PiecesOnConveyorLayer)
	//	{
	//		component.gameObject.layer = PiecesOnConveyorLayer;
	//		rOnConveyor.Add(component);
	//	}
	//	else if (!rOnConveyor.Contains(component))
	//	{
	//		rOnConveyor.Add(component);
	//	}
	//	ISoundable component2 = other.gameObject.GetComponent<ISoundable>();
	//	if (component2 == null)
	//	{
	//		return;
	//	}
	//	AudioSourceListener audioSource = soundSettings.GetAudioSource(component2.SoundType);
	//	if (audioSource)
	//	{
	//		if (oreHitConveyorSound.UsePitchRange)
	//		{
	//			audioSource.Pitch = UnityEngine.Random.Range(oreHitConveyorSound.MinPitch, oreHitConveyorSound.MaxPitch);
	//		}
	//		audioSource.PlayOneShot();
	//	}
	//}

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

	private void OnVendersScreenUnlock()
	{
		if (destroyingPiecesZone != null)
		{
			Destroy(destroyingPiecesZone.gameObject);
		}
		box.HideTube();

        PlayerPrefs.SetInt("UnlockedScreen3", 1);
    }

    public void BarAction(Costable component, GameObject smelterObj, float duration, Color col)
    {
        if (smelterObj.name.Contains("1"))
        {
            pipes[0].GetComponent<LiquidAnimation>().PlayOnceAnim(duration, col);
        }
        else if (smelterObj.name.Contains("2"))
        {
            pipes[1].GetComponent<LiquidAnimation>().PlayOnceAnim(duration, col);
        }

        if (PlayerPrefs.GetInt("UnlockedScreen3", 0) == 0)
        {
            StartCoroutine(CoroutineUtils.Delay(duration, delegate ()
            {
                box.AddMoney(moneyManager.GetCost(component));
                component.Push(true);
            }));
        }
        else
        {
            StartCoroutine(CoroutineUtils.Delay(duration, delegate ()
            {
                component.Push(true);
                venderTube.PlayOnceAnim();
                float multiplier = this.upgradeSettings.GetCurrentUpgrade(UpgradeType.BottleValue).Value;
                venderControl.ReceiveLiquidFromTube(moneyManager.GetCost(component) * multiplier, duration);
            }));
        }
        
    }

 //   private void OnPieceEnteredDestructionZone(Collider collider)
	//{
 //       Debug.Log(collider.gameObject.name);
	//	Costable component = collider.GetComponent<Costable>();
	//	if (component == null)
	//	{
	//		return;
	//	}
 //       Debug.Log("++++++++++");
 //       rOnConveyor.Remove(component.Rigidbody);
	//	box.AddMoney(moneyManager.GetCost(component));
	//	component.Push(true);
	//}

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
