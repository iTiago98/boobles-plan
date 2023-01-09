using FMOD.Studio;
using FMODUnity;
using Santi.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using Booble.Flags;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Booble.Managers
{
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance { get; private set; }

        [SerializeField] private EventReference _loungeMusicReference;
        [SerializeField] private EventReference _interviewMusicReference;
        [SerializeField] private EventReference _mainMenuReference;

        private Bus _masterBus;
        private Bus _bgmBus;
        private Bus _sfxBus;

        private EventInstance _currentInstance;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            FlagManager.Instance.ResetFlags();
            PlayMusic(MusicReference.MainMenu);
            InitializeBuses();
        }

        private void InitializeBuses()
        {
            string masterBusPath = "bus:/";
            _masterBus = RuntimeManager.GetBus(masterBusPath);

            string bgmBusPath = "bus:/BGM";
            _bgmBus = RuntimeManager.GetBus(bgmBusPath);

            string sfxBusPath = "bus:/SFX";
            _sfxBus = RuntimeManager.GetBus(sfxBusPath);
        }

        public void PlayMusic(MusicReference reference)
        {
            EventReference eventReference;
            switch (reference)
            {
                case MusicReference.Interview:
                    eventReference = _interviewMusicReference;
                    break;
                case MusicReference.Lounge:
                    eventReference = _loungeMusicReference;
                    break;
                case MusicReference.MainMenu:
                    eventReference = _mainMenuReference;
                    break;
                default: return;
            }

            _currentInstance.stop(STOP_MODE.IMMEDIATE);
            _currentInstance = RuntimeManager.CreateInstance(eventReference);
            _currentInstance.start();
            _currentInstance.release();
        }

        public float GetGeneralMusicVolume()
        {
            float volume;
            _masterBus.getVolume(out volume);
            return volume;
        }
        public void ChangeGeneralMusicVolume(float value)
        {
            _masterBus.setVolume(value);
        }

        public float GetBackgroundMusicVolume()
        {
            float volume;
            _bgmBus.getVolume(out volume);
            return volume;
        }
        public void ChangeBackgroundMusicVolume(float value)
        {
            _bgmBus.setVolume(value);
        }

        public float GetSFXMusicVolume()
        {
            float volume;
            _sfxBus.getVolume(out volume);
            return volume;
        }
        public void ChangeSFXVolume(float value)
        {
            _sfxBus.setVolume(value);
        }
    }

    public enum MusicReference
    {
        Lounge,
        Interview,
        MainMenu
    }
}
