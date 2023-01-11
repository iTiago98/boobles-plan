using Booble.CardGame.Managers;
using Booble.Player;
using Booble.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Booble.Managers
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        [SerializeField] private Fader _fadeScreen;

        public bool InMainMenu => _currentScene == Scenes.MAIN_MENU;
        public bool InInterview => _currentScene == Scenes.INTERVIEW;
        public bool InExploration => !InMainMenu && !InInterview;

        private string _currentScene;
        private string _previousScene;
        private Camera _mainCamera;

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
            _currentScene = scene;

            _fadeScreen.FadeOut(() =>
            {
                var async = SceneManager.LoadSceneAsync(scene);
                async.completed += OnLoungeSceneLoaded;
            });
        }

        #region Quick Access

        public void LoadLoungeScene0()
        {
            LoadScene(Scenes.LOUNGE_0);
        }

        public void LoadNelaOffice0()
        {
            LoadScene(Scenes.NELA_OFFICE_0);
        }

        public void LoadNelaOffice1()
        {
            LoadScene(Scenes.NELA_OFFICE_1);
        } 
        
        public void LoadNelaOffice2()
        {
            LoadScene(Scenes.NELA_OFFICE_2);
        }

        public void LoadUpperHall1()
        {
            LoadScene(Scenes.UPPER_HALL_1);
        }
        
        public void LoadUpperHall2()
        {
            LoadScene(Scenes.UPPER_HALL_2);
        }
        
        public void LoadCanteenScene0()
        {
            LoadScene(Scenes.CANTEEN_0);
        }
        
        public void LoadCanteenScene2()
        {
            LoadScene(Scenes.CANTEEN_2);
        }

            public void LoadLowerHall1()
            {
                LoadScene(Scenes.LOWER_HALL_1);
            }

        public void LoadLowerHall2()
        {
            LoadScene(Scenes.LOWER_HALL_2);
        }
        
        public void LoadLoungeScene1()
        {
            LoadScene(Scenes.LOUNGE_1);
        }
        
        public void LoadLoungeScene2()
        {
            LoadScene(Scenes.LOUNGE_2);
        }

        public void LoadPPBOffice()
        {
            LoadScene(Scenes.PPB_OFFICE);
        }

        public void LoadHome0()
        {
            LoadScene(Scenes.HOME_0);
        }

        public void LoadHome2()
        {
            LoadScene(Scenes.HOME_2);
        }
        
        #endregion

        #region Interview

        public void LoadInterviewScene()
        {
            LoadInterviewScene(new List<GameObject>() { Camera.main.gameObject });
        }

        public void LoadInterviewScene(AsyncOperation op)
        {
            LoadInterviewScene(new List<GameObject>(), false);
        }

        public void LoadInterviewScene(List<GameObject> objects, bool updateCurrentScene = true)
        {
            if (updateCurrentScene)
            {
                _previousScene = _currentScene;
                _currentScene = Scenes.INTERVIEW;
                _disabledObjects = objects;
            }

            MusicManager.Instance.PlayMusic(MusicReference.Interview);
            _fadeScreen.FadeOut(() =>
            {
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
            });
        }

        public void ReloadInterview()
        {
            _fadeScreen.FadeOut(() =>
            {
                var async = SceneManager.UnloadSceneAsync(Scenes.INTERVIEW);
                async.completed += LoadInterviewScene;
            });
        }

        #endregion

        private void OnSceneLoaded(AsyncOperation op)
        {
            _fadeScreen.FadeIn();
        }

        private void OnLoungeSceneLoaded(AsyncOperation op)
        {
            //RestoreMainCamera(op);
            Controller.Instance.enabled = true;
            _fadeScreen.FadeIn();
        }

        private void OnInterviewLoaded(AsyncOperation op)
        {
            CardGameManager.Instance.Initialize(_previousScene != Scenes.MAIN_MENU);
        }

        private void EnableObjects(List<GameObject> objects, bool enable)
        {
            foreach (GameObject obj in objects)
            {
                obj.SetActive(enable);
            }
        }
    }
}