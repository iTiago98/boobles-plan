using Booble.CardGame.Managers;
using Booble.UI;
using System.Collections;
using System.Collections.Generic;
using Booble.Interactables;
using UnityEngine;
using Cursor = Booble.UI.Cursor;

namespace Booble.Managers
{
    public class GameManager : DontDestroyOnLoad<GameManager>
    {
        public bool gamePaused { private set; get; }

        private float _defaultTimeScale;

        protected override void Awake()
        {
            base.Awake();
            PlayerConfig.InitializeValues();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) CheckPauseMenu();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!focus && !gamePaused)
            {
                CheckPauseMenu();
            }
        }

        private void CheckPauseMenu()
        {
            if (SceneLoader.Instance != null &&
                (SceneLoader.Instance.InMainMenu || SceneLoader.Instance.InCredits 
                || SceneLoader.Instance.IsBetweenScenes || !SceneLoader.Instance.IsInitialized)) return;
            if (UIManager.Instance != null && UIManager.Instance.loseMenuActive) return;
            SwitchPauseMenu();
        }

        public void SwitchPauseMenu()
        {
            if (gamePaused) ResumeGame();
            else PauseGame();
        }

        public void ResumeGame()
        {
            gamePaused = false;

            Time.timeScale = _defaultTimeScale;

            PauseMenu.Instance.ShowPauseMenu(false);

            if (CardGameManager.Instance != null) CardGameManager.Instance.ResumeGame();

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.GetComponent<Interactable>())
            {
                Cursor.Instance.ShowActionText(true);
            }
        }

        private void PauseGame()
        {
            gamePaused = true;

            _defaultTimeScale = Time.timeScale;
            Time.timeScale = 0;

            PauseMenu.Instance.ShowPauseMenu(true);

            if (CardGameManager.Instance != null) CardGameManager.Instance.PauseGame();
        }
    }
}