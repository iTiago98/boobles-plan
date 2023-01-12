using Booble.CardGame.Cards.DataModel;
using Booble.CardGame.Managers;
using Booble.Flags;
using Booble.Managers;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Booble.UI
{
    public class PauseMenu : MonoBehaviour
    {
        public static PauseMenu Instance { get; private set; }

        [SerializeField] private GameObject _pauseMenu;

        [SerializeField] private GameObject _mainMenuPanel;

        [Header("Cards Menu")]
        [SerializeField] private Button _cardMenuButton;
        [SerializeField] private GameObject _cardsMenuPanel;
        [SerializeField] private CardMenu _cardMenu;
        [SerializeField] private ExtendedDescriptionPanel _extendedDescription;

        [Header("Options Menu")]
        [SerializeField] private GameObject _optionsMenuPanel;
        [SerializeField] private OptionsMenu _optionsMenu;

        [Header("Pause Menu Button")]
        [SerializeField] private GameObject _pauseMenuButton;

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

        public void ShowPauseMenu(bool value)
        {
            if (value)
            {
                _pauseMenu.SetActive(true);
                _cardMenuButton.interactable = SceneLoader.Instance.InExploration;
                ShowPauseMenuButton(false);
            }
            else
            {
                OnOptionsBackButtonClick();
                OnCardsBackButtonClick();
                _pauseMenu.SetActive(false);
                ShowPauseMenuButton(true);
            }
        }

        public void ShowPauseMenuButton(bool value)
        {
            _pauseMenuButton.SetActive(value);
        }

        public void OnPauseMenuButtonClick()
        {
            GameManager.Instance.SwitchPauseMenu();
        }

        public void OnResumeButtonClick()
        {
            GameManager.Instance.ResumeGame();
        }

        #region Cards Menu

        public void OnCardsButtonClick()
        {
            _mainMenuPanel.SetActive(false);
            _cardsMenuPanel.SetActive(true);

            _cardMenu.SetCardsMenu();
        }

        public void OnCardsBackButtonClick()
        {
            _cardsMenuPanel.SetActive(false);
            _mainMenuPanel.SetActive(true);
        }

        public void ShowExtendedDescription(CardsData data)
        {
            _extendedDescription.Show(data);
        }

        public void HideExtendedDescription()
        {
            _extendedDescription.Hide();
        }

        #endregion

        #region Options Menu

        public void OnOptionsButtonClick()
        {
            _mainMenuPanel.SetActive(false);
            _optionsMenuPanel.SetActive(true);

            _optionsMenu.SetSliderValue();
        }

        public void OnOptionsBackButtonClick()
        {
            _mainMenuPanel.SetActive(true);
            _optionsMenuPanel.SetActive(false);
        }

        #endregion

        public void OnReturnToMenuButtonClick()
        {
            GameManager.Instance.ResumeGame();

            if (SceneLoader.Instance.InInterview)
            {
                SceneLoader.Instance.UnloadInterviewScene();
                if (CardGameManager.Instance.playingStoryMode) SceneLoader.Instance.LoadMainMenuScene();
            }
            else if (SceneLoader.Instance.InExploration) SceneLoader.Instance.LoadMainMenuScene();
        }
    }
}