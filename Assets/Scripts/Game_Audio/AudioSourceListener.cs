using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.GameScreen;
using UI.ListItems;
using UnityEngine;
using Utilities;

namespace Game.Audio
{
	[RequireComponent(typeof(AudioSource))]
	public class AudioSourceListener : MonoBehaviour
    {
        [SerializeField]
        private SoundSettings soundSettings;

        [SerializeField]
        private SoundType soundType;

        [SerializeField]
        private Sound currentSound;

        private AudioSource audioSource;

        private Coroutine changeVolumeRoutine;

        private float internalVolumeMultiplier;

        private bool wasPlayingLoop;

        public SoundType SoundType
		{
			get
			{
				return soundType;
			}
		}

		public float Pitch
		{
			get
			{
				return audioSource.pitch;
			}
			set
			{
				audioSource.pitch = value;
			}
		}

		public bool IsPlaying
		{
			get
			{
				return audioSource.isPlaying;
			}
		}

		public float InitialVolume { get; private set; }

		public float Volume
		{
			get
			{
				return audioSource.volume;
			}
			set
			{
				audioSource.volume = value;
			}
		}

		public void Initialize(SoundSettings soundSettings, Sound sound, GameScreensManager gameScreensManager)
		{
			this.soundSettings = soundSettings;
			currentSound = sound;
			soundType = sound.SoundType;
			audioSource.spatialBlend = (sound.ConfineSoundToGameScreen ? 1f : 0f);
			gameScreensManager.GameScreensCreated += OnGameScreensCreated;
		}

		private void Awake()
		{
			audioSource = GetComponent<AudioSource>();
			audioSource.playOnAwake = false;
		}

		private void Start()
		{
			this.InitializeSound();
		}

		private void OnGameScreensCreated(IReadOnlyList<GameScreenController> gameScreens)
		{
			if (!currentSound.ConfineSoundToGameScreen)
			{
				return;
			}
			GameScreenController gameScreenController = gameScreens.First((GameScreenController g) => g.Index == currentSound.GameScreenIndex);
			transform.position = gameScreenController.Position.SetZ(CameraUtils.MainCamera.transform.position.z);
			audioSource.maxDistance = gameScreenController.Size.x / 2f;
		}

		public void Play(bool loop = false)
		{
			if (changeVolumeRoutine != null)
			{
				StopCoroutine(changeVolumeRoutine);
			}
			Volume = InitialVolume;
			audioSource.loop = loop;
			if (currentSound.IsSoundEnabled)
			{
				SetAudioClip();
				audioSource.Play();
			}
		}

		public void Play(float volumeIncreaseTime, bool loop = false)
		{
			Play(loop);
			Volume = 0f;
			changeVolumeRoutine = StartCoroutine(ChangeVolume(volumeIncreaseTime, InitialVolume, null));
		}

		public void Play(float delay, float volumeIncreaseTime, bool loop = false)
		{
			changeVolumeRoutine = StartCoroutine(CoroutineUtils.Delay(delay, delegate()
			{
				Play(volumeIncreaseTime, loop);
			}));
		}

		public void Stop(float volumeDecreaseTime, Action callback = null)
		{
			if (changeVolumeRoutine != null)
			{
				StopCoroutine(changeVolumeRoutine);
			}
			changeVolumeRoutine = StartCoroutine(ChangeVolume(volumeDecreaseTime, 0f, delegate
			{
				Stop();
				Action callback2 = callback;
				if (callback2 == null)
				{
					return;
				}
				callback2();
			}));
		}

		private IEnumerator ChangeVolume(float time, float value, Action postExecute = null)
		{
			float initialValue = Volume;
			float currentTime = 0f;
			while (currentTime < time)
			{
				currentTime = Mathf.Min(currentTime + Time.deltaTime, time);
				Volume = Mathf.Lerp(initialValue, value, currentTime / time);
				yield return null;
			}
            if(postExecute != null)
			    postExecute();
			yield break;
		}

		private void SetAudioClip()
		{
			if (currentSound.UsePitchRange)
			{
				audioSource.pitch = UnityEngine.Random.Range(currentSound.MinPitch, currentSound.MaxPitch);
			}
			if (!currentSound.UseRandomAudioClipsRange)
			{
				audioSource.clip = currentSound.AudioClip;
				return;
			}
			if (currentSound.AudioClipsList == null || currentSound.AudioClipsList.Count == 0)
			{
				audioSource.clip = null;
				return;
			}
			audioSource.clip = currentSound.AudioClipsList.Random<AudioClip>();
		}

		public void PlayOneShot()
		{
			if (currentSound.IsSoundEnabled)
			{
				audioSource.PlayOneShot(currentSound.UseRandomAudioClipsRange ? currentSound.AudioClipsList.Random<AudioClip>() : currentSound.AudioClip);
			}
		}

		public void Stop()
		{
			if (changeVolumeRoutine != null)
			{
				StopCoroutine(changeVolumeRoutine);
			}
			AudioSource audioSource = this.audioSource;
			if (audioSource == null)
			{
				return;
			}
			audioSource.Stop();
		}

		private void InitializeSound()
		{
			currentSound = soundSettings.GetSound(soundType);
			if (!currentSound.UsePitchRange)
			{
				audioSource.pitch = currentSound.Pitch;
			}
			audioSource.volume = (InitialVolume = currentSound.SoundVolume * soundSettings.MasterVolume);
		}

	}
}
