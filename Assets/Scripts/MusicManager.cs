using FMOD.Studio;
using FMODUnity;
using Santi.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : Singleton<MusicManager>
{

    [SerializeField] private EventReference _loungeMusicReference;
    [SerializeField] private EventReference _interviewMusicReference;
    [SerializeField] private EventReference _mainMenuReference;

    private EventInstance _loungeMusicInstance;
    private EventInstance _interviewMusicInstance;
    private EventInstance _mainMenuInstance;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        PlayMainMenuMusic();
    }

    public void PlayMainMenuMusic()
    {
        _mainMenuInstance = RuntimeManager.CreateInstance(_mainMenuReference);
        _mainMenuInstance.start();
    }

    public void PlayLoungeMusic()
    {
        StopMainMenuMusic();

        _loungeMusicInstance = RuntimeManager.CreateInstance(_loungeMusicReference);
        _loungeMusicInstance.start();
    }

    public void PlayInterviewMusic()
    {
        StopLoungeMusic();

        _interviewMusicInstance = RuntimeManager.CreateInstance(_interviewMusicReference);
        _interviewMusicInstance.start();
    }

    private void StopMainMenuMusic()
    {
        PLAYBACK_STATE playbackState;
        _mainMenuInstance.getPlaybackState(out playbackState);

        if (playbackState == PLAYBACK_STATE.PLAYING) _mainMenuInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    private void StopLoungeMusic()
    {
        PLAYBACK_STATE playbackState;
        _loungeMusicInstance.getPlaybackState(out playbackState);

        if (playbackState == PLAYBACK_STATE.PLAYING) _loungeMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public void StopInterviewMusic()
    {
        PLAYBACK_STATE playbackState;
        _interviewMusicInstance.getPlaybackState(out playbackState);

        if (playbackState == PLAYBACK_STATE.PLAYING) _interviewMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
