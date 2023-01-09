using Booble.CardGame.Managers;
using Booble.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Booble.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public bool gamePaused { private set; get; }

        private float _defaultTimeScale;

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

        private void Update()
        {
            CheckPauseMenu();
        }

        private void CheckPauseMenu()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (SceneLoader.Instance != null && SceneLoader.Instance.InMainMenu) return;
                if (UIManager.Instance != null && UIManager.Instance.loseMenuActive) return;

                if (gamePaused) ResumeGame();
                else PauseGame();
            }
        }

        public void ResumeGame()
        {
            gamePaused = false;

            Time.timeScale = _defaultTimeScale;

            PauseMenu.Instance.ShowHidePauseMenu();

            if (CardGameManager.Instance != null) CardGameManager.Instance.ResumeGame();
        }


        private void PauseGame()
        {
            gamePaused = true;

            _defaultTimeScale = Time.timeScale;
            Time.timeScale = 0;

            PauseMenu.Instance.ShowHidePauseMenu();

            if (CardGameManager.Instance != null) CardGameManager.Instance.PauseGame();
        }
    }
}