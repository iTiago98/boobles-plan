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
        //[SerializeField] private GameObject _cardsMenuPanel;
        [SerializeField] private CardMenu _cardMenu;
        [SerializeField] private ExtendedDescriptionPanel _extendedDescription;

        [Header("Options Menu")]
        //[SerializeField] private GameObject _optionsMenuPanel;
        [SerializeField] private OptionsMenu _optionsMenu;

        [Header("Pause Button")]
        [SerializeField] private PauseButton _pauseButton;

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
                ShowPauseButton(false);
            }
            else
            {
                OnOptionsBackButtonClick();
                OnCardsBackButtonClick();
                _pauseMenu.SetActive(false);
                ShowPauseButton(true);
            }
        }

        #region Pause Button

        public void ShowPauseButton(bool value)
        {
            _pauseButton.gameObject.SetActive(value);
        }

        #endregion

        public void OnResumeButtonClick()
        {
            GameManager.Instance.ResumeGame();
        }

        #region Cards Menu

        public void OnCardsButtonClick()
        {
            _mainMenuPanel.SetActive(false);
            _cardMenu.gameObject.SetActive(true);

            _cardMenu.SetCardsMenu();
        }

        public void OnCardsBackButtonClick()
        {
            _cardMenu.gameObject.SetActive(false);
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
            _optionsMenu.gameObject.SetActive(true);

            _optionsMenu.SetSliderValue();
        }

        public void OnOptionsBackButtonClick()
        {
            _mainMenuPanel.SetActive(true);
            _optionsMenu.gameObject.SetActive(false);
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