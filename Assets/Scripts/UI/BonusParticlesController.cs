using System;
using Sirenix.OdinInspector;
using UI.ListItems;
using UIControllers;
using UnityEngine;
using Utilities;

namespace UI
{
	public class BonusParticlesController : MonoBehaviour
    {
  //      [SerializeField]
  //      [Required]
  //      [AssetsOnly]
  //      private ParticleSystem moneyBonusParticleSystemPrefab;

  //      [SerializeField]
  //      [Required]
  //      private BonusView bonusView;

  //      private ParticleSystem moneyBonusParticleSystem;

  //      private void OnEnable()
		//{
		//	bonusView.BonusStateChange += OnBonusStateChange;
		//}

		//private void OnDisable()
		//{
		//	bonusView.BonusStateChange -= OnBonusStateChange;
		//}

		//private void OnBonusStateChange(Bonus b, bool isEnabled)
		//{
		//	Camera mainCamera = CameraUtils.MainCamera;
		//	if (b.BonusType != BonusType.Money)
		//	{
		//		return;
		//	}
		//	if (isEnabled)
		//	{
		//		moneyBonusParticleSystem = moneyBonusParticleSystemPrefab.PullOrCreate<ParticleSystem>();
		//		moneyBonusParticleSystem.transform.SetParent(mainCamera.transform, true);
		//		moneyBonusParticleSystem.transform.SetLocalPositionX(0f);
		//		moneyBonusParticleSystem.Play();
		//		return;
		//	}
		//	ParticleSystem particleSystem = moneyBonusParticleSystem;
		//	if (particleSystem != null)
		//	{
		//		particleSystem.Stop();
		//	}
		//	StartCoroutine(CoroutineUtils.Delay(3f, delegate()
		//	{
		//		ParticleSystem particleSystem2 = moneyBonusParticleSystem;
		//		if (particleSystem2 != null)
		//		{
		//			particleSystem2.Push<ParticleSystem>();
		//		}
		//		moneyBonusParticleSystem = null;
		//	}));
		//}

	}
}
