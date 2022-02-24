using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using Utilities;

namespace UI.ListItems
{
	[Serializable]
	public class Sound
	{

        //[FoldoutGroup("$Name", 0)]
        [OnValueChanged("OnValueChange", false)]
        [SerializeField]
        private bool isSoundEnabled;

        //[FoldoutGroup("$Name", 0)]
        [OnValueChanged("OnValueChange", false)]
        [SerializeField]
        [HideIf("useRandomAudioClipsRange", true)]
        [ShowIf("isSoundEnabled", true)]
        private AudioClip audioClip;

        //[FoldoutGroup("$Name", 0)]
        [OnValueChanged("OnValueChange", false)]
        [SerializeField]
        [ShowIf("useRandomAudioClipsRange", true)]
        [ShowIf("isSoundEnabled", true)]
        private List<AudioClip> audioClipsList;

        //[FoldoutGroup("$Name", 0)]
        [OnValueChanged("OnValueChange", false)]
        [SerializeField]
        [ShowIf("isSoundEnabled", true)]
        private bool useRandomAudioClipsRange;

        //[FoldoutGroup("$Name", 0)]
        [OnValueChanged("OnValueChange", false)]
        [SerializeField]
        [ShowIf("isSoundEnabled", true)]
        [Range(0f, 1f)]
        private float soundVolume;

        //[FoldoutGroup("$Name", 0)]
        [OnValueChanged("OnValueChange", false)]
        [SerializeField]
        [HideIf("usePitchRange", true)]
        [ShowIf("isSoundEnabled", true)]
        [Range(-3f, 3f)]
        private float pitch = 1f;

        //[FoldoutGroup("$Name", 0)]
        [OnValueChanged("OnValueChange", false)]
        [SerializeField]
        [ShowIf("usePitchRange", true)]
        [ShowIf("isSoundEnabled", true)]
        [MinMaxSlider(-3f, 3f, true)]
        private Vector2 pitchRange = Vector2.one;

        //[FoldoutGroup("$Name", 0)]
        [OnValueChanged("OnValueChange", false)]
        [SerializeField]
        [ShowIf("isSoundEnabled", true)]
        private bool usePitchRange;

        //[FoldoutGroup("$Name", 0)]
        [SerializeField]
        private bool confineSoundToGameScreen;

        //[FoldoutGroup("$Name", 0)]
        [SerializeField]
        [ShowIf("confineSoundToGameScreen", true)]
        private int gameScreenIndex;

        //[FoldoutGroup("$Name", 0)]
        [SerializeField]
        public SoundType SoundType;

        [UsedImplicitly]
		public string Name
		{
			get
			{
				return SoundType.ToString().AddSpaces();
			}
		}

		public AudioClip AudioClip
		{
			get
			{
				return audioClip;
			}
		}

		public List<AudioClip> AudioClipsList
		{
			get
			{
				return audioClipsList;
			}
		}

		public float SoundVolume
		{
			get
			{
				return soundVolume;
			}
		}

		public float Pitch
		{
			get
			{
				return pitch;
			}
		}

		public float MinPitch
		{
			get
			{
				return pitchRange.x;
			}
		}

		public float MaxPitch
		{
			get
			{
				return pitchRange.y;
			}
		}

		public int GameScreenIndex
		{
			get
			{
				return gameScreenIndex;
			}
		}

		public bool UsePitchRange
		{
			get
			{
				return usePitchRange;
			}
		}

		public bool IsSoundEnabled
		{
			get
			{
				return isSoundEnabled;
			}
		}

		public bool UseRandomAudioClipsRange
		{
			get
			{
				return useRandomAudioClipsRange;
			}
		}

		public bool ConfineSoundToGameScreen
		{
			get
			{
				return confineSoundToGameScreen;
			}
		}

		public event Action ValueChanged;

		[UsedImplicitly]
		private void OnValueChange()
		{
			Action valueChanged = ValueChanged;
			if (valueChanged == null)
			{
				return;
			}
			valueChanged();
		}

	}
}
