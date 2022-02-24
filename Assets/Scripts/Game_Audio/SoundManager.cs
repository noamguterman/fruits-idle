using System;
using System.Collections.Generic;
using System.Linq;
using Game.GameScreen;
using Sirenix.OdinInspector;
using UI.ListItems;
using UnityEngine;
using Utilities;

namespace Game.Audio
{
	public class SoundManager : MonoBehaviour
    {
        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        [SceneObjectsOnly]
        private GameScreensManager gameScreensManager;

        //[FoldoutGroup("References", 0)]
        [SerializeField]
        [Required]
        private SoundSettings soundSettings;

        private List<AudioSourceListener> sourceListeners;

        public AudioSourceListener GetAudioSourceListener(SoundType soundType)
		{
			return sourceListeners.FirstOrDefault((AudioSourceListener s) => s.SoundType == soundType);
		}

		private void Awake()
		{
			gameObject.hideFlags = HideFlags.None;
			if (!soundSettings)
			{
				return;
			}
			CreateSourceListeners(soundSettings.Sounds);
			if (!SoundSettings.IsSoundOn)
			{
				return;
			}
			AudioListener.volume = 0f;
			Preloader.LogoHided += delegate()
			{
				StartCoroutine(CoroutineUtils.LerpFloat(0f, 1f, 1f, delegate(float value)
				{
					AudioListener.volume = value;
				}));
			};
		}

		public void Initialize(SoundSettings soundSettings)
		{
			this.soundSettings = soundSettings;
			CreateSourceListeners(soundSettings.Sounds);
		}

		private void CreateSourceListeners(List<Sound> sounds)
		{
			sourceListeners = new List<AudioSourceListener>();
			foreach (object obj in transform)
			{
				Destroy(((Transform)obj).gameObject);
			}
			sounds.ForEach(delegate(Sound s)
			{
				AudioSourceListener audioSourceListener = new GameObject(s.SoundType.ToString().AddSpaces()).AddComponent<AudioSourceListener>();
				sourceListeners.Add(audioSourceListener);
				audioSourceListener.transform.SetParent(transform);
				audioSourceListener.Initialize(soundSettings, s, gameScreensManager);
			});
		}

	}
}
