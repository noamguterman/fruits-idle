using System;
using System.Collections.Generic;
using System.Linq;
using Game.Audio;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace UI.ListItems
{
	[CreateAssetMenu(menuName = "Settings/Sound Settings", fileName = "Sound Settings")]
	public class SoundSettings : ScriptableObject
    {
        [Space(20f)]
        [SerializeField]
        [Range(0f, 1f)]
        [OnValueChanged("OnValueChange", false)]
        private float masterVolume;

        [GenerateListFromEnum(EnumType = typeof(SoundType))]
        [SerializeField]
        [ListDrawerSettings(Expanded = true, IsReadOnly = true)]
        private List<Sound> sounds;

        private SoundManager soundManager;

        private SoundManager SoundManager
		{
			get
			{
				if (soundManager)
				{
					return soundManager;
				}
				soundManager = FindObjectOfType<SoundManager>();
				if (!soundManager)
				{
					soundManager = new GameObject("SoundManager".AddSpaces()).AddComponent<SoundManager>();
					soundManager.Initialize(this);
				}
				return soundManager;
			}
		}

		public static bool IsVibrationOn
		{
			get
			{
				return PlayerPrefs.GetInt("IsVibrationOn", 1) == 1;
			}
			set
			{
				if (value && !IsVibrationOn)
				{
					Handheld.Vibrate();
				}
				PlayerPrefs.SetInt("IsVibrationOn", value ? 1 : 0);
			}
		}

		public static bool IsSoundOn
		{
			get
			{
				return PlayerPrefs.GetInt("IsSoundOn", 1) == 1;
			}
			set
			{
				PlayerPrefs.SetInt("IsSoundOn", value ? 1 : 0);
				AudioListener.volume = (float)(value ? 1 : 0);
			}
		}

		public List<Sound> Sounds
		{
			get
			{
				return sounds;
			}
		}

		public float MasterVolume
		{
			get
			{
				return masterVolume;
			}
		}

		public event Action MasterVolumeChanged;

		[UsedImplicitly]
		private void OnValueChange()
		{
			Action masterVolumeChanged = MasterVolumeChanged;
			if (masterVolumeChanged == null)
			{
				return;
			}
			masterVolumeChanged();
		}

		public Sound GetSound(SoundType soundType)
		{
			return sounds.First((Sound s) => s.SoundType == soundType);
		}

		public AudioSourceListener GetAudioSource(SoundType soundType)
		{
			return SoundManager.GetAudioSourceListener(soundType);
		}

	}
}
