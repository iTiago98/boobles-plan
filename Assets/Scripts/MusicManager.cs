using FMOD.Studio;
using FMODUnity;
using Santi.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using Booble.Flags;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Booble
{
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance { get; private set; }

        [SerializeField] private EventReference _loungeMusicReference;
        [SerializeField] private EventReference _interviewMusicReference;
        [SerializeField] private EventReference _mainMenuReference;

        private EventInstance _currentInstance;
        //
        // private EventInstance _loungeMusicInstance;
        // private EventInstance _interviewMusicInstance;
        // private EventInstance _mainMenuInstance;

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
            // PlayMainMenuMusic();
            FlagManager.Instance.ResetFlags();
            PlayMusic(MusicReference.MainMenu);
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
