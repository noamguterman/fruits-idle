using System;
using Settings.UI.Tabs;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UI.ListItems.Upgrades.Piece;
using UnityEngine;
using Utilities;
using Utilities.AlphaAnimation;

namespace Game
{
	public class Box : MonoBehaviour, ISuspendable
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private ParticleSystem moneyParticleSystem;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private SpriteRenderer boxSprite;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private SpriteRenderer boxTube;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private Lamp lamp;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private bool useForOre;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [ShowIf("useForOre", true)]
        private Vector2Int oreLevelsRange;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [ShowIf("useForOre", true)]
        private Vector2 particlesRateRange;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [ShowIf("useForOre", true)]
        private UpgradeSettings upgradeSettings;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [ShowIf("useForOre", true)]
        private float maxWaitTimeForParticleEmit;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [HideIf("useForOre", true)]
        private float costPerParticle;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        [HideIf("useForOre", true)]
        private float delayTime = 0.05f;

        //[FoldoutGroup("Settings", 0)]
        [SerializeField]
        private int maxParticlesCount = 10;

        private Transform boxTransform;

        private float currentMoney;

        private float accumulatedParticlesAmount;

        private float lastParticleEmitTime;

        private bool isSuspended;

        private int currentLevel;

        private int CurrentLevel
		{
			get
			{
				return currentLevel;
			}
			set
			{
				currentLevel = value;
				PlayerPrefs.SetInt("CurrentOreLevelsForParticles", value);
			}
		}

		public Vector3 Position
		{
			get
			{
				return boxTransform.position;
			}
			set
			{
				boxTransform.position = value;
			}
		}

		private void Awake()
		{
			boxTransform = transform;
			if (!useForOre)
			{
				return;
			}
			currentLevel = PlayerPrefs.GetInt("CurrentOreLevelsForParticles");
			upgradeSettings.PiecesUpgrades.ForEach(delegate(PieceListItem p)
			{
				p.LevelIncreased += delegate()
				{
					int num = CurrentLevel;
					CurrentLevel = num + 1;
				};
			});
		}

		public void Initialize(bool withTube)
		{
            if(boxTube != null)
    			boxTube.gameObject.SetActive(withTube);
			//lamp.SetActive(!withTube, 0f);
		}

		public Bounds Bounds
		{
			get
			{
				return boxSprite.bounds;
			}
		}

		public void AddMoney(float money)
		{
			if (useForOre)
			{
				HandleParticesOre();
				return;
			}
			HandlePartices(money);
		}

		private void HandleParticesOre()
		{
			if (moneyParticleSystem.particleCount >= maxParticlesCount)
			{
				return;
			}
			float t = (float)(CurrentLevel - oreLevelsRange.x) / (float)(oreLevelsRange.y - oreLevelsRange.x);
			float num = particlesRateRange.Lerp(t);
			if (lastParticleEmitTime <= 0f)
			{
				lastParticleEmitTime = Time.time;
				moneyParticleSystem.Emit(1);
				return;
			}
			float num2 = Time.time - lastParticleEmitTime;
			float num3 = Mathf.Max(0f, num - num2);
			if (num3 <= maxWaitTimeForParticleEmit)
			{
				lastParticleEmitTime = Time.time + num3;
				StartCoroutine(CoroutineUtils.Delay(num3, delegate()
				{
					moneyParticleSystem.Emit(1);
				}));
			}
		}

		private void HandlePartices(float money)
		{
			currentMoney += money;
			if (currentMoney >= costPerParticle)
			{
				if (moneyParticleSystem.particleCount < maxParticlesCount)
				{
					int num = Mathf.FloorToInt(Mathf.Min((float)(maxParticlesCount - moneyParticleSystem.particleCount), currentMoney / costPerParticle));
					for (int i = 0; i < num; i++)
					{
						StartCoroutine(CoroutineUtils.Delay(delayTime * (float)i, delegate()
						{
							moneyParticleSystem.Emit(1);
						}));
					}
				}
				currentMoney = 0f;
			}
		}

		public void HideTube()
		{
            if (boxTube != null)
                boxTube.AnimateAlpha(0f, 0.15f, delegate(SpriteRenderer b)
			    {
				    b.gameObject.SetActive(false);
			    });
			StartCoroutine(CoroutineUtils.Delay(moneyParticleSystem.main.startLifetime.constantMax, delegate()
			{
				moneyParticleSystem.gameObject.SetActive(false);
			}));
			//lamp.SetActive(true, 0.1f);
		}

		public void Suspend()
		{
            if (boxTube != null)
                if (boxTube.gameObject.activeSelf || isSuspended)
			    {
				    return;
			    }
			isSuspended = true;
			//lamp.StartFlashing();
		}

		public void Resume()
		{
            if (boxTube != null)
                if (boxTube.gameObject.activeSelf || !isSuspended)
			    {
    				return;
	    		}
			isSuspended = false;
			//lamp.Stop();
		}

	}
}
