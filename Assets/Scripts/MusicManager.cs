using FMOD.Studio;
using FMODUnity;
using Santi.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : Singleton<MusicManager>
{

    [SerializeField] private EventReference _loungeMusicReference;
    [SerializeField] private EventReference _interviewMusicReference;

    private EventInstance _loungeMusicInstance;
    private EventInstance _interviewMusicInstance;

    private void Start()
    {
        PlayLoungeMusic();
    }

    public void PlayLoungeMusic()
    {
        _loungeMusicInstance = RuntimeManager.CreateInstance(_loungeMusicReference);
        _loungeMusicInstance.start();
    }

    public void PlayInterviewMusic()
    {
        StopLoungeMusic();

        _interviewMusicInstance = RuntimeManager.CreateInstance(_interviewMusicReference);
        _interviewMusicInstance.start();
    }

    private void StopLoungeMusic()
    {
        PLAYBACK_STATE playbackState;
        _loungeMusicInstance.getPlaybackState(out playbackState);

        if (playbackState == PLAYBACK_STATE.PLAYING) _loungeMusicInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

}
