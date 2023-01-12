using Booble.CardGame;
using Booble.CardGame.Cards.DataModel;
using Booble.CardGame.Managers;
using Booble.Flags;
using Booble.Managers;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Booble.Managers.DeckManager;

namespace Booble.UI
{
    public class PauseMenu : MonoBehaviour
    {
        public static PauseMenu Instance { get; private set; }

        [SerializeField] private GameObject _pauseMenu;

        [SerializeField] private GameObject _mainMenuPanel;
        [SerializeField] private GameObject _cardObtainedText;

        [Header("Cards Menu")]
        [SerializeField] private AlertButton _cardMenuButton;
        [SerializeField] private CardMenu _cardMenu;
        [SerializeField] private ExtendedDescriptionPanel _extendedDescription;

        [Header("Options Menu")]
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

        public void InitializeCardMenu()
        {
            _pauseMenu.SetActive(true);
            _cardMenu.gameObject.SetActive(true);
            _cardMenu.SetCardsMenu();
            _cardMenu.gameObject.SetActive(false);
            _pauseMenu.SetActive(false);
        }

        public void ShowPauseMenu(bool value)
        {
            if (value)
            {
                _pauseMenu.SetActive(true);
                _cardMenuButton.SetInteractable(SceneLoader.Instance.InExploration);
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

        public void UpdateAlerts(bool add)
        {
            List<CardData> newCards = DeckManager.Instance.GetNewCards();
            _cardMenu.UpdateExtraCards(newCards);

            _pauseButton.ShowAlert(newCards.Count > 0);
            _cardMenuButton.ShowAlert(newCards.Count > 0);

            if (add) ShowCardObtainedText();
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

        public void ShowCardObtainedText()
        {
            float initialX = _cardObtainedText.transform.position.x;
            
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_cardObtainedText.transform.DOMoveX(initialX, 0.5f));
            sequence.Append(_cardObtainedText.transform.DOMoveX(0, 1f));
            sequence.AppendInterval(1f);
            sequence.Append(_cardObtainedText.transform.DOMoveX(initialX, 1f));

            sequence.Play();
        }
    }
}