using Booble.CardGame;
using Booble.CardGame.Cards.DataModel;
using Booble.CardGame.Managers;
using Booble.Flags;
using Booble.Managers;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Booble.Managers.DeckManager;

namespace Booble.UI
{
    public class PauseMenu : DontDestroyOnLoad<PauseMenu>
    {
        [SerializeField] private GameObject _pauseMenu;

        [SerializeField] private GameObject _mainMenuPanel;
        [SerializeField] private GameObject _cardObtainedText;
        [SerializeField] private GameObject _savedDataText;

        [SerializeField] private GameObject _retryInterviewButton;

        [Header("Cards Menu")]
        [SerializeField] private AlertButton _cardMenuButton;
        [SerializeField] private CardMenu _cardMenu;
        [SerializeField] private ExtendedDescriptionPanel _extendedDescription;

        [Header("Options Menu")]
        [SerializeField] private OptionsMenu _optionsMenu;

        [Header("Return Menu")]
        [SerializeField] private GameObject _returnToMenu;

        [Header("Pause Button")]
        [SerializeField] private PauseButton _pauseButton;

        public void InitializeCardMenu()
        {
            _pauseMenu.SetActive(true);
            _cardMenu.gameObject.SetActive(true);
            _cardMenu.SetCardsMenu();
            _cardMenu.gameObject.SetActive(false);
            _pauseMenu.SetActive(false);
        }

        public bool IsActive()
        {
            return _pauseMenu.activeSelf;
        }

        public void ShowPauseMenu(bool value)
        {
            if (value)
            {
                _pauseMenu.SetActive(true);
                _retryInterviewButton.gameObject.SetActive(SceneLoader.Instance.InInterview);
                _cardMenuButton.gameObject.SetActive(SceneLoader.Instance.InExploration);
                ShowPauseButton(false);
            }
            else
            {
                OnOptionsBackButtonClick();
                OnCardsBackButtonClick();
                OnConfirmBackButtonClick();
                _pauseMenu.SetActive(false);
                if (SceneLoader.Instance.InExploration) ShowPauseButton(true);
            }
        }

        #region Pause Button

        public void ShowPauseButton(bool value)
        {
            _pauseButton.gameObject.SetActive(value);
        }

        public void AddNewCard(CardData card)
        {
            _cardMenu.AddNewCard(card);

            _pauseButton.ShowAlert(true);
            _cardMenuButton.ShowAlert(true);

            ShowCardObtainedText();
        }

        public void RemoveAlert(CardData card, int newCards)
        {
            _cardMenu.RemoveAlert(card);

            _pauseButton.ShowAlert(newCards > 0);
            _cardMenuButton.ShowAlert(newCards > 0);
        }

        #endregion

        public void OnResumeButtonClick()
        {
            GameManager.Instance.ResumeGame();
        }

        public void OnRetryInterviewButtonClick()
        {
            if (CardGameManager.Instance != null)
            {
                GameManager.Instance.ResumeGame();
                CardGameManager.Instance.RetryInterview();
            }
        }

        #region Cards Menu

        public void OnCardsButtonClick()
        {
            _mainMenuPanel.SetActive(false);
            _cardMenu.gameObject.SetActive(true);
            _cardMenu.Refresh();
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

        #region Return To Menu

        public void OnReturnClick()
        {
            _mainMenuPanel.SetActive(false);
            _returnToMenu.SetActive(true);
        }

        public void OnConfirmBackButtonClick()
        {
            _mainMenuPanel.SetActive(true);
            _returnToMenu.SetActive(false);
        }

        public void OnReportBugButtonClick()
        {
            Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSfE8YhMeTyWyeNwKKGdaAvwRhgYC0SzD5hs4ZukR_1JuMOKBQ/viewform?usp=sf_link");
        }

        public void OnReturnToMenuButtonClick()
        {
            GameManager.Instance.ResumeGame();
            _returnToMenu.SetActive(false);

            if (SceneLoader.Instance.InInterview)
            {
                if (CardGameManager.Instance.playingStoryMode)
                {
                    SceneLoader.Instance.UnloadInterviewAndLoadScene(Scenes.MAIN_MENU);
                }
                else
                {
                    SceneLoader.Instance.UnloadInterviewScene();
                }
            }
            else
            {
                SceneLoader.Instance.LoadMainMenuScene();
            }
        }

        #endregion

        float cardObtainedTextInitialX = -1;

        public void ShowCardObtainedText()
        {
            if (cardObtainedTextInitialX == -1) cardObtainedTextInitialX = _cardObtainedText.transform.position.x;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(_cardObtainedText.transform.DOMoveX(cardObtainedTextInitialX, 0.5f));
            sequence.Append(_cardObtainedText.transform.DOMoveX(0, 1f));
            sequence.AppendInterval(1f);
            sequence.Append(_cardObtainedText.transform.DOMoveX(cardObtainedTextInitialX, 1f));

            sequence.Play();
        }

        public void ShowSavedDataText()
        {
            ShowSavedDataText(() => { });
        }

        public void ShowSavedDataText(TweenCallback callback)
        {
            float initialX = _savedDataText.transform.position.x;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(_savedDataText.transform.DOMoveX(initialX, 0.5f));
            sequence.Append(_savedDataText.transform.DOMoveX(0, 1f));
            sequence.AppendInterval(1f);
            sequence.AppendCallback(callback);
            sequence.Append(_savedDataText.transform.DOMoveX(initialX, 1f));

            sequence.Play();
        }
    }
}