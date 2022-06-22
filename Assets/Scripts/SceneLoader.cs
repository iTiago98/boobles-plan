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

    //[SerializeField] private Camera _explorationCam;
    //[SerializeField] private Controller _explorationPlayerController;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if(Instance == null)
        {
            Instance = this;
        } else
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
            var async = SceneManager.LoadSceneAsync(Scenes.MAIN_MENU_SCENE);
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

    private void OnSceneLoaded(AsyncOperation op)
    {
        _fadeScreen.FadeIn2();
    }

    private void OnLoungeSceneLoaded(AsyncOperation op)
    {
        Camera.main.gameObject.SetActive(true);
        Controller.Instance.enabled = true;
        _fadeScreen.FadeIn2();
    }
}
