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

        public void ChangeGeneralMusicVolume(float value)
        {
            _masterBus.setVolume(value);
        }

        public void ChangeBackgroundMusicVolume(float value)
        {
            _bgmBus.setVolume(value);
        }

        public void ChangeSFXVolume(float value)
        {
            _sfxBus.setVolume(value);
        }

        // public void PlayMainMenuMusic()
        // {
        //     
        //     _currentInstance = RuntimeManager.CreateInstance(_mainMenuReference);
        //     _currentInstance.start();
        //     _mainMenuInstance = RuntimeManager.CreateInstance(_mainMenuReference);
        //     _mainMenuInstance.start();
        // }
        //
        // public void PlayLoungeMusic()
        // {
        //     _loungeMusicInstance = RuntimeManager.CreateInstance(_loungeMusicReference);
        //     _loungeMusicInstance.start();
        // }
        //
        // public void PlayInterviewMusic()
        // {
        //     _interviewMusicInstance = RuntimeManager.CreateInstance(_interviewMusicReference);
        //     _interviewMusicInstance.start();
        // }
        //
        // public void StopMusic()
        // {
        //     PLAYBACK_STATE playbackState;
        //     foreach (EventInstance instance in _eventInstances)
        //     {
        //         instance.getPlaybackState(out playbackState);
        //         if (playbackState == PLAYBACK_STATE.PLAYING) instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        //     }
        // }
        //
        // // public void StopMainMenuMusic()
        // // {
        // //     PLAYBACK_STATE playbackState;
        // //     _mainMenuInstance.getPlaybackState(out playbackState);
        // //
        // //     if (playbackState == PLAYBACK_STATE.PLAYING) _mainMenuInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        // // }
        // //
        // // public void StopLoungeMusic()
        // // {
        // //     PLAYBACK_STATE playbackState;
        // //     _loungeMusicInstance.getPlaybackState(out playbackState);
        // //
        // //     if (playbackState == PLAYBACK_STATE.PLAYING) _loungeMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        // // }
        // //
        // // public void StopInterviewMusic()
        // // {
        // //     PLAYBACK_STATE playbackState;
        // //     _interviewMusicInstance.getPlaybackState(out playbackState);
        // //
        // //     if (playbackState == PLAYBACK_STATE.PLAYING) _interviewMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        // // }
    }

    public enum MusicReference
    {
        Lounge,
        Interview,
        MainMenu
    }
}
