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

        public bool InMainMenu => InScene(Scenes.MAIN_MENU);
        public bool InExploration => InScene(new List<string>() {
            Scenes.LOUNGE_0, Scenes.LOUNGE_1, Scenes.LOUNGE_2, Scenes.LOUNGE_3, Scenes.LOUNGE_4,
            Scenes.NELA_OFFICE_0, Scenes.NELA_OFFICE_1, Scenes.NELA_OFFICE_2, Scenes.NELA_OFFICE_3,
            Scenes.LOWER_HALL_1, Scenes.LOWER_HALL_2, Scenes.LOWER_HALL_3,
            Scenes.UPPER_HALL_1, Scenes.UPPER_HALL_2, Scenes.UPPER_HALL_3,
            Scenes.BOSS_HALL_3, Scenes.BOSS_HALL_4, Scenes.BOSS_OFFICE,
            Scenes.CANTEEN_0, Scenes.CANTEEN_2, Scenes.PPB_OFFICE
        });
        public bool InInterview => InScene(Scenes.INTERVIEW);
        public bool InCredits => InScene(Scenes.CREDITS);

        private bool InCar => InScene(new List<string>() { Scenes.CAR_0, Scenes.CAR_1 });
        private bool InHome => InScene(new List<string>() { Scenes.HOME_0, Scenes.HOME_1, Scenes.HOME_2 });

        public bool InMainMenuBehaviour => InMainMenu || InCar || InHome || InCredits
            || InScene(new List<string>() { Scenes.NELA_OFFICE_4, Scenes.CANTEEN_ENDING, Scenes.HOME_ENDING });

        public bool InExplorationBehaviour => InExploration;
        public bool InHybridBehaviour => InScene(new List<string>() { Scenes.NELA_OFFICE_DAY_START, Scenes.BOSS_OFFICE_ENDING });

        public bool InCluesScene => InScene(new List<string>()
        {
            Scenes.LOUNGE_0, Scenes.LOUNGE_1, Scenes.LOUNGE_2, Scenes.LOUNGE_3,
            // Scenes.NELA_OFFICE_1, Scenes.NELA_OFFICE_2, Scenes.NELA_OFFICE_3,
            Scenes.LOWER_HALL_1, Scenes.LOWER_HALL_2, Scenes.LOWER_HALL_3,
            Scenes.UPPER_HALL_1, Scenes.UPPER_HALL_2, Scenes.BOSS_HALL_3,
            Scenes.CANTEEN_2, Scenes.PPB_OFFICE
        });
        
        public string CurrentScene { get; private set; }
        public string PreviousScene { get; private set; }

        private List<GameObject> _disabledObjects;

        private void Start()
        {
            CurrentScene = Scenes.MAIN_MENU;
            _disabledObjects = new List<GameObject>();
        }

        private bool InScene(string scene)
        {
            return InScene(new List<string>() { scene });
        }
        private bool InScene(List<string> scenes)
        {
            return scenes.Contains(CurrentScene);
        }

        public void LoadScene(string scene)
        {
            PreviousScene = CurrentScene;
            CurrentScene = scene;

            _fadeScreen.FadeOut(() =>
            {
                var async = SceneManager.LoadSceneAsync(scene);
                async.completed += OnSceneLoaded;

                if (InMainMenuBehaviour) async.completed += OnMainMenuSceneLoaded;
                else if (InExplorationBehaviour) async.completed += OnExplorationSceneLoaded;
                else if (InHybridBehaviour) async.completed += OnHybridSceneLoaded;
            });
        }

        #region Quick Access

        public void LoadMainMenuScene()
        {
            LoadScene(Scenes.MAIN_MENU);
        }

        public void LoadCar0()
        {
            LoadScene(Scenes.CAR_0);
        }

        public void LoadNelaOfficeDayStart()
        {
            LoadScene(Scenes.NELA_OFFICE_DAY_START);
        }

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

        public void LoadNelaOffice3()
        {
            LoadScene(Scenes.NELA_OFFICE_3);
        }

        public void LoadUpperHall1()
        {
            LoadScene(Scenes.UPPER_HALL_1);
        }

        public void LoadUpperHall2()
        {
            LoadScene(Scenes.UPPER_HALL_2);
        }

        public void LoadUpperHall3()
        {
            LoadScene(Scenes.UPPER_HALL_3);
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

        public void LoadLowerHall3()
        {
            LoadScene(Scenes.LOWER_HALL_3);
        }

        public void LoadBossHall3()
        {
            LoadScene(Scenes.BOSS_HALL_3);
        }

        public void LoadBossHall4()
        {
            LoadScene(Scenes.BOSS_HALL_4);
        }

        public void LoadBossOffice()
        {
            LoadScene(Scenes.BOSS_OFFICE);
        }

        public void LoadLoungeScene1()
        {
            LoadScene(Scenes.LOUNGE_1);
        }

        public void LoadLoungeScene2()
        {
            LoadScene(Scenes.LOUNGE_2);
        }

        public void LoadLoungeScene3()
        {
            LoadScene(Scenes.LOUNGE_3);
        }

        public void LoadLoungeScene4()
        {
            LoadScene(Scenes.LOUNGE_4);
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

        public void LoadBossOfficeEnding()
        {
            LoadScene(Scenes.BOSS_OFFICE_ENDING);
        }

        public void LoadCanteenEnding()
        {
            LoadScene(Scenes.CANTEEN_ENDING);
        }

        public void LoadHomeEnding()
        {
            LoadScene(Scenes.HOME_ENDING);
        }

        public void LoadCredits()
        {
            LoadScene(Scenes.CREDITS);
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

                if (InMainMenu) async.completed += OnMainMenuSceneLoaded;
                else if (InExploration) async.completed += OnExplorationSceneLoaded;
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

        #region Callbacks

        private void OnSceneLoaded(AsyncOperation op)
        {
            _fadeScreen.FadeIn();
        }

        private void OnMainMenuSceneLoaded(AsyncOperation op)
        {
            PauseMenu.Instance.ShowPauseButton(false);
            MusicManager.Instance.PlayMusic(MusicReference.MainMenu);
        }

        private void OnHybridSceneLoaded(AsyncOperation op)
        {
            PauseMenu.Instance.ShowPauseButton(false);
            MusicManager.Instance.PlayMusic(MusicReference.Lounge);
        }

        private void OnExplorationSceneLoaded(AsyncOperation op)
        {
            Controller.Instance.enabled = true;
            PauseMenu.Instance.ShowPauseButton(true);
            MusicManager.Instance.PlayMusic(MusicReference.Lounge);
        }

        private void OnInterviewLoaded(AsyncOperation op)
        {
            PauseMenu.Instance.ShowPauseButton(false);
            ClueUI.Instance.DisableCluesButton();
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
                    async.completed += OnExplorationSceneLoaded;
            });
        }

        private void OnReloadInterviewScene(AsyncOperation op)
        {
            LoadInterviewScene(new List<GameObject>(), false);
        }

        #endregion

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