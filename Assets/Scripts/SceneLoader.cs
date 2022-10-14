using Booble.Player;
using Booble.UI;
using Santi.Utils;
using System.Collections;
using System.Collections.Generic;
using Booble;
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
        // MusicManager.Instance.StopInterviewMusic();
        // MusicManager.Instance.StopMusic();
        // MusicManager.Instance.PlayMainMenuMusic();
        MusicManager.Instance.PlayMusic(MusicReference.MainMenu);
        _fadeScreen.FadeOut(() =>
        {
            var async = SceneManager.LoadSceneAsync(Scenes.MAIN_MENU);
            async.completed += OnSceneLoaded;
        });
    }

    public void LoadLoungeScene0()
    {
        // MusicManager.Instance.StopMainMenuMusic();
        // MusicManager.Instance.StopMusic();
        // MusicManager.Instance.PlayLoungeMusic();
        _fadeScreen.FadeOut(() =>
        {
            var async = SceneManager.LoadSceneAsync(Scenes.LOUNGE_0);
            async.completed += OnLoungeSceneLoaded;
        });
    }

    public void LoadNelaOffice0()
    {
        _fadeScreen.FadeOut(() =>
        {
            var async = SceneManager.LoadSceneAsync(Scenes.NELA_OFFICE_0);
            async.completed += OnLoungeSceneLoaded;
        });
    }
    
    public void LoadNelaOffice1()
    {
        _fadeScreen.FadeOut(() =>
        {
            var async = SceneManager.LoadSceneAsync(Scenes.NELA_OFFICE_1);
            async.completed += OnLoungeSceneLoaded;
        });
    }
    
    public void LoadInterviewScene()
    {
        // MusicManager.Instance.StopLoungeMusic();
        // MusicManager.Instance.StopMusic();
        // MusicManager.Instance.PlayInterviewMusic();
        MusicManager.Instance.PlayMusic(MusicReference.Interview);
        _fadeScreen.FadeOut(() =>
        {
            Camera.main.gameObject.SetActive(false);
            Controller.Instance.enabled = false;

            var async = SceneManager.LoadSceneAsync(Scenes.INTERVIEW, LoadSceneMode.Additive);
            async.completed += OnSceneLoaded;
        });
    }

    public void UnloadInterviewScene()
    {
        // MusicManager.Instance.StopInterviewMusic();
        // MusicManager.Instance.StopMusic();
        // MusicManager.Instance.PlayLoungeMusic();
        MusicManager.Instance.PlayMusic(MusicReference.Lounge);
        _fadeScreen.FadeOut(() =>
        {
            Controller.Instance.enabled = true;

            var async = SceneManager.UnloadSceneAsync(Scenes.INTERVIEW);
            async.completed += OnSceneLoaded;
            async.completed += RestoreMainCamera;
        });
    }

    public void LoadCanteenScene0()
    {
        _fadeScreen.FadeOut(() =>
        {
            var async = SceneManager.LoadSceneAsync(Scenes.CANTEEN_0);
            async.completed += OnLoungeSceneLoaded;
        });
    }

    public void LoadLowerHall1()
    {
        _fadeScreen.FadeOut(() =>
        {
            var async = SceneManager.LoadSceneAsync(Scenes.LOWER_HALL_1);
            async.completed += OnLoungeSceneLoaded;
        });
    }

    public void LoadLoungeScene1()
    {
        _fadeScreen.FadeOut(() =>
        {
            var async = SceneManager.LoadSceneAsync(Scenes.LOUNGE_1);
            async.completed += OnLoungeSceneLoaded;
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
