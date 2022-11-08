using Booble.Player;
using Booble.UI;
using Santi.Utils;
using System.Collections;
using System.Collections.Generic;
using Booble;
using UnityEngine;
using UnityEngine.SceneManagement;
using CardGame.Managers;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [SerializeField] private FadeIn _fadeScreen;

    private string _currentScene;
    private string _previousScene;
    private List<GameObject> _disabledObjects;
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

    private void Start()
    {
        _currentScene = Scenes.MAIN_MENU;
        _disabledObjects = new List<GameObject>();
    }

    public void LoadMainMenuScene()
    {
        _currentScene = Scenes.MAIN_MENU;

        MusicManager.Instance.PlayMusic(MusicReference.MainMenu);
        _fadeScreen.FadeOut(() =>
        {
            var async = SceneManager.LoadSceneAsync(Scenes.MAIN_MENU);
            async.completed += OnSceneLoaded;
        });
    }

    public void LoadScene(string scene)
    {
        _fadeScreen.FadeOut(() =>
        {
            var async = SceneManager.LoadSceneAsync(scene);
            async.completed += OnLoungeSceneLoaded;
        });
    }

    public void LoadLoungeScene0()
    {
        _currentScene = Scenes.LOUNGE_0;

        _fadeScreen.FadeOut(() =>
        {
            var async = SceneManager.LoadSceneAsync(Scenes.LOUNGE_0);
            async.completed += OnLoungeSceneLoaded;
        });
    }

    public void LoadNelaOffice0()
    {
        _currentScene = Scenes.NELA_OFFICE_0;

        _fadeScreen.FadeOut(() =>
        {
            var async = SceneManager.LoadSceneAsync(Scenes.NELA_OFFICE_0);
            async.completed += OnLoungeSceneLoaded;
        });
    }

    public void LoadNelaOffice1()
    {
        _currentScene = Scenes.NELA_OFFICE_1;

        _fadeScreen.FadeOut(() =>
        {
            var async = SceneManager.LoadSceneAsync(Scenes.NELA_OFFICE_1);
            async.completed += OnLoungeSceneLoaded;
        });
    }

    public void LoadInterviewScene()
    {
        LoadInterviewScene(new List<GameObject>() { Camera.main.gameObject });
    }

    public void LoadInterviewScene(List<GameObject> objects)
    {
        _previousScene = _currentScene;
        _currentScene = Scenes.INTERVIEW;

        MusicManager.Instance.PlayMusic(MusicReference.Interview);
        _fadeScreen.FadeOut(() =>
        {
            _disabledObjects = objects;
            EnableObjects(objects, false);
            if (_previousScene != Scenes.MAIN_MENU) Controller.Instance.enabled = false;

            var async = SceneManager.LoadSceneAsync(Scenes.INTERVIEW, LoadSceneMode.Additive);
            async.completed += OnSceneLoaded;
            async.completed += OnInterviewLoaded;
        });
    }

    public void UnloadInterviewScene()
    {
        _currentScene = _previousScene;

        if (_previousScene == Scenes.MAIN_MENU) MusicManager.Instance.PlayMusic(MusicReference.MainMenu);
        else MusicManager.Instance.PlayMusic(MusicReference.Lounge);

        _fadeScreen.FadeOut(() =>
        {
            EnableObjects(_disabledObjects, true);
            if (_previousScene != Scenes.MAIN_MENU)
                Controller.Instance.enabled = true;

            var async = SceneManager.UnloadSceneAsync(Scenes.INTERVIEW);
            async.completed += OnSceneLoaded;
            //async.completed += RestoreMainCamera;
        });
    }

    public void LoadCanteenScene0()
    {
        _currentScene = Scenes.CANTEEN_0;

        _fadeScreen.FadeOut(() =>
        {
            var async = SceneManager.LoadSceneAsync(Scenes.CANTEEN_0);
            async.completed += OnLoungeSceneLoaded;
        });
    }

    public void LoadLowerHall1()
    {
        _currentScene = Scenes.LOWER_HALL_1;

        _fadeScreen.FadeOut(() =>
        {
            var async = SceneManager.LoadSceneAsync(Scenes.LOWER_HALL_1);
            async.completed += OnLoungeSceneLoaded;
        });
    }

    public void LoadLoungeScene1()
    {
        _currentScene = Scenes.LOUNGE_1;

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
        //RestoreMainCamera(op);
        Controller.Instance.enabled = true;
        _fadeScreen.FadeIn2();
    }

    private void OnInterviewLoaded(AsyncOperation op)
    {
        TurnManager.Instance.InitializeBoardBackground();
    }

    private void EnableObjects(List<GameObject> objects, bool enable)
    {
        foreach(GameObject obj in objects)
        {
            obj.SetActive(enable);
        }
    }

}
