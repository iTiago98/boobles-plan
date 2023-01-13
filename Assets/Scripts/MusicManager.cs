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

        private EventInstance _currentInstance;
        private MusicReference _currentReference;

        private void Start()
        {
            // FlagManager.Instance.ResetFlags();
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
            if (reference == _currentReference) return;

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

        #endregion
    }

    public enum MusicReference
    {
        Lounge,
        Interview,
        MainMenu
    }

    public enum SFXReference
    {
        InterviewWin,
        InterviewLose
    }
}
