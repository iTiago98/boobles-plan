using Booble.CardGame.Managers;
using Booble.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Booble.Managers
{
    public class GameManager : DontDestroyOnLoad<GameManager>
    {
        public bool gamePaused { private set; get; }

        private float _defaultTimeScale;

        private void Update()
        {
            CheckPauseMenu();
        }

        private void CheckPauseMenu()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (SceneLoader.Instance != null && (SceneLoader.Instance.InMainMenu || SceneLoader.Instance.InCredits)) return;
                if (UIManager.Instance != null && UIManager.Instance.loseMenuActive) return;
                SwitchPauseMenu();
            }
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