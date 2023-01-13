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
        
        [Header("Cards Menu")]
        [SerializeField] private AlertButton _cardMenuButton;
        [SerializeField] private CardMenu _cardMenu;
        [SerializeField] private ExtendedDescriptionPanel _extendedDescription;

        [Header("Options Menu")]
        [SerializeField] private OptionsMenu _optionsMenu;

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
                if (SceneLoader.Instance.InExploration || SceneLoader.Instance.InCar || SceneLoader.Instance.InHome) ShowPauseButton(true);
            }
        }

        #region Pause Button

        public void ShowPauseButton(bool value)
        {
            _pauseButton.gameObject.SetActive(value);
        }

        public void UpdateAlerts(List<CardData> newCards, bool showAddedText, bool showAlertIcons)
        {
            _cardMenu.UpdateExtraCards(newCards, showAlertIcons);

            _pauseButton.ShowAlert(showAlertIcons && newCards.Count > 0);
            _cardMenuButton.ShowAlert(showAlertIcons && newCards.Count > 0);

            if (showAddedText) ShowCardObtainedText();
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

        public void OnReturnToMenuButtonClick()
        {
            GameManager.Instance.ResumeGame();

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

        public void ShowSavedDataText()
        {
            ShowSavedDataText(() => {});
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