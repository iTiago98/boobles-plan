using Booble.Player;
using Booble.UI;
using Santi.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    [SerializeField] private FadeIn _fadeScreen;

    [SerializeField] private Camera _explorationCam;
    [SerializeField] private Controller _explorationPlayerController;

    public void LoadInterviewScene()
    {
        MusicManager.Instance.PlayInterviewMusic();
        _fadeScreen.FadeOut(() =>
        {
            _explorationCam.gameObject.SetActive(false);
            _explorationPlayerController.enabled = false;

            var async = SceneManager.LoadSceneAsync(Scenes.INTERVIEW_SCENE, LoadSceneMode.Additive);
            async.completed += OnSceneLoaded;
        });
    }

    private void OnSceneLoaded(AsyncOperation op)
    {
        _fadeScreen.FadeIn2();
    }
}
