using Booble.Player;
using Booble.UI;
using Santi.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [SerializeField] private FadeIn _fadeScreen;

    private Camera _explorationCam;
    //[SerializeField] private Controller _explorationPlayerController;

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

    public void LoadMainMenuScene()
    {
        MusicManager.Instance.StopInterviewMusic();
        MusicManager.Instance.PlayMainMenuMusic();
        _fadeScreen.FadeOut(() =>
        {
            var async = SceneManager.LoadSceneAsync(Scenes.MAIN_MENU_SCENE);
            async.completed += OnSceneLoaded;
        });
    }

    public void LoadLoungeScene()
    {
        MusicManager.Instance.StopMainMenuMusic();
        MusicManager.Instance.PlayLoungeMusic();
        _fadeScreen.FadeOut(() =>
        {
            var async = SceneManager.LoadSceneAsync(Scenes.LOUNGE_SCENE);
            async.completed += OnLoungeSceneLoaded;
        });
    }

    public void LoadInterviewScene()
    {
        MusicManager.Instance.StopLoungeMusic();
        MusicManager.Instance.PlayInterviewMusic();
        _fadeScreen.FadeOut(() =>
        {
            Camera.main.gameObject.SetActive(false);
            Controller.Instance.enabled = false;

            var async = SceneManager.LoadSceneAsync(Scenes.INTERVIEW_SCENE, LoadSceneMode.Additive);
            async.completed += OnSceneLoaded;
        });
    }

    public void ReturnToLoungeScene()
    {
        MusicManager.Instance.StopInterviewMusic();
        MusicManager.Instance.PlayLoungeMusic();
        _fadeScreen.FadeOut(() =>
        {
            Controller.Instance.enabled = true;

            var async = SceneManager.UnloadSceneAsync(Scenes.INTERVIEW_SCENE);
            async.completed += OnSceneLoaded;
            async.completed += RestoreMainCamera;
        });
    }

    private void OnSceneLoaded(AsyncOperation op)
    {
        _fadeScreen.FadeIn2();
    }

    private void OnLoungeSceneLoaded(AsyncOperation op)
    {
        SetExplorationCam();
        RestoreMainCamera(op);
        Controller.Instance.enabled = true;
        _fadeScreen.FadeIn2();
    }

    private void RestoreMainCamera(AsyncOperation op)
    {
        _explorationCam.gameObject.SetActive(true);
    }

    private void SetExplorationCam()
    {
        _explorationCam = Camera.main;
    }

}
