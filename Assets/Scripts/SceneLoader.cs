using Booble.CardGame.Managers;
using Booble.Player;
using Booble.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Booble.Managers
{
    public class SceneLoader : DontDestroyOnLoad<SceneLoader>
    {
        [SerializeField] private Fader _fadeScreen;

        public bool InMainMenu => CurrentScene == Scenes.MAIN_MENU;
        public bool InInterview => CurrentScene == Scenes.INTERVIEW;
        public bool InExploration => !InMainMenu && !InInterview;
        public string CurrentScene { get; private set; }
        public string PreviousScene { get; private set; }

        private List<GameObject> _disabledObjects;

        private void Start()
        {
            CurrentScene = Scenes.MAIN_MENU;
            _disabledObjects = new List<GameObject>();
        }

        public void LoadMainMenuScene()
        {
            PreviousScene = CurrentScene;
            CurrentScene = Scenes.MAIN_MENU;

            _fadeScreen.FadeOut(() =>
            {
                var async = SceneManager.LoadSceneAsync(Scenes.MAIN_MENU);
                async.completed += OnSceneLoaded;
                async.completed += OnMainMenuSceneLoaded;
            });
        }

        public void LoadScene(string scene)
        {
            PreviousScene = CurrentScene;
            CurrentScene = scene;

            _fadeScreen.FadeOut(() =>
            {
                var async = SceneManager.LoadSceneAsync(scene);
                async.completed += OnSceneLoaded;
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

        public void LoadHome1()
        {
            LoadScene(Scenes.HOME_1);
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

        public void LoadInterviewScene(List<GameObject> objects, bool updateCurrentScene = true)
        {
            if (updateCurrentScene)
            {
                PreviousScene = CurrentScene;
                CurrentScene = Scenes.INTERVIEW;
                _disabledObjects = objects;
            }

            _fadeScreen.FadeOut(() =>
            {
                EnableObjects(objects, false);
                if (PreviousScene != Scenes.MAIN_MENU) Controller.Instance.enabled = false;

                var async = SceneManager.LoadSceneAsync(Scenes.INTERVIEW, LoadSceneMode.Additive);
                async.completed += OnSceneLoaded;
                async.completed += OnInterviewLoaded;
            });
        }

        public void UnloadInterviewScene()
        {
            CurrentScene = PreviousScene;

            _fadeScreen.FadeOut(() =>
            {
                EnableObjects(_disabledObjects, true);

                var async = SceneManager.UnloadSceneAsync(Scenes.INTERVIEW);
                async.completed += OnSceneLoaded;

                if (InMainMenu) 
                    async.completed += OnMainMenuSceneLoaded;
                else if (InExploration) 
                    async.completed += OnLoungeSceneLoaded;
            });
        }

        public void UnloadInterviewAndLoadScene(string scene)
        {
            CurrentScene = scene;

            _fadeScreen.FadeOut(() =>
            {
                var async = SceneManager.UnloadSceneAsync(Scenes.INTERVIEW);
                async.completed += OnInterviewUnloaded;
            });
        }

        public void ReloadInterview()
        {
            _fadeScreen.FadeOut(() =>
            {
                var async = SceneManager.UnloadSceneAsync(Scenes.INTERVIEW);
                async.completed += OnReloadInterviewScene;
            });
        }

        #endregion

        private void OnSceneLoaded(AsyncOperation op)
        {
            _fadeScreen.FadeIn();
        }

        private void OnMainMenuSceneLoaded(AsyncOperation op)
        {
            PauseMenu.Instance.ShowPauseButton(false);
            MusicManager.Instance.PlayMusic(MusicReference.MainMenu);
        }

        private void OnLoungeSceneLoaded(AsyncOperation op)
        {
            Controller.Instance.enabled = true;
            PauseMenu.Instance.ShowPauseButton(true);
            MusicManager.Instance.PlayMusic(MusicReference.Lounge);
        }

        private void OnInterviewLoaded(AsyncOperation op)
        {
            PauseMenu.Instance.ShowPauseButton(false);
            MusicManager.Instance.PlayMusic(MusicReference.Interview);
            CardGameManager.Instance.Initialize(PreviousScene != Scenes.MAIN_MENU);
        }

        private void OnInterviewUnloaded(AsyncOperation op)
        {
            _fadeScreen.FadeOut(() =>
            {
                var async = SceneManager.LoadSceneAsync(CurrentScene);
                async.completed += OnSceneLoaded;

                if (InMainMenu)
                    async.completed += OnMainMenuSceneLoaded;
                else if (InExploration)
                    async.completed += OnLoungeSceneLoaded;
            });
        }

        private void OnReloadInterviewScene(AsyncOperation op)
        {
            LoadInterviewScene(new List<GameObject>(), false);
        }

        private void EnableObjects(List<GameObject> objects, bool enable)
        {
            foreach (GameObject obj in objects)
            {
                obj.SetActive(enable);
            }
        }

        public bool CheckScenes(string currentScene, string previousScene)
        {
            return CurrentScene.StartsWith(currentScene) && PreviousScene.StartsWith(previousScene);
        }
    }
}