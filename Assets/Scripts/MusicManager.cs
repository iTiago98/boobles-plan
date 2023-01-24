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
    public class MusicManager : DontDestroyOnLoad<MusicManager>
    {
        [Header("Background music")]
        [SerializeField] private EventReference _loungeMusicReference;
        [SerializeField] private EventReference _interviewMusicReference;
        [SerializeField] private EventReference _mainMenuReference;

        [Header("SFX")]
        [SerializeField] private EventReference _interviewWinSoundReference;
        [SerializeField] private EventReference _interviewLoseSoundReference;

        private Bus _masterBus;
        private Bus _bgmBus;
        private Bus _sfxBus;

        private const string MasterBusPath = "bus:/";
        private const string BGMBusPath = "bus:/BGM";
        private const string SFXBusPath = "bus:/SFX";

        private EventInstance _currentInstance;
        private MusicReference _currentReference;

        private void Start()
        {
            PlayMusic(MusicReference.MainMenu);
            InitializeBuses();
        }

        private void InitializeBuses()
        {
            _masterBus = RuntimeManager.GetBus(MasterBusPath);
            _bgmBus = RuntimeManager.GetBus(BGMBusPath);
            _sfxBus = RuntimeManager.GetBus(SFXBusPath);

            _masterBus.setVolume(PlayerConfig.GetMasterVolume());
            _bgmBus.setVolume(PlayerConfig.GetBGMVolume());
            _sfxBus.setVolume(PlayerConfig.GetSFXVolume());
        }

        public void PlayMusic(MusicReference reference)
        {
            if (_currentReference != MusicReference.Interview && reference == _currentReference) return;

            _currentReference = reference;

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
        
        public void StopMusic()
        {
            _currentInstance.stop(STOP_MODE.IMMEDIATE);
            _currentReference = MusicReference.None;
        }

        public void PlaySound(SFXReference reference)
        {
            EventReference eventReference;
            switch (reference)
            {
                case SFXReference.InterviewWin:
                    eventReference = _interviewWinSoundReference;
                    break;
                case SFXReference.InterviewLose:
                    eventReference = _interviewLoseSoundReference;
                    break;
                default: return;
            }

            EventInstance instance = RuntimeManager.CreateInstance(eventReference);
            instance.start();
            instance.release();
        }

        public void PlayInterviewEnd(bool playerWin)
        {
            if (playerWin) PlaySound(SFXReference.InterviewWin);
            else PlaySound(SFXReference.InterviewLose);
        }

        #region Bus Volume

        public float GetGeneralMusicVolume()
        {
            return PlayerConfig.GetMasterVolume();
        }

        public void ChangeGeneralMusicVolume(float value)
        {
            _masterBus.setVolume(value);
            PlayerConfig.SetMasterVolume(value);
        }

        public float GetBackgroundMusicVolume()
        {
            return PlayerConfig.GetBGMVolume();
        }
        public void ChangeBackgroundMusicVolume(float value)
        {
            _bgmBus.setVolume(value);
            PlayerConfig.SetBGMVolume(value);
        }

        public float GetSFXMusicVolume()
        {
            return PlayerConfig.GetSFXVolume();
        }

        public void ChangeSFXVolume(float value)
        {
            _sfxBus.setVolume(value);
            PlayerConfig.SetSFXVolume(value);
        }

        #endregion
    }

    public enum MusicReference
    {
        Lounge,
        Interview,
        MainMenu,
        None
    }

    public enum SFXReference
    {
        InterviewWin,
        InterviewLose
    }
}
