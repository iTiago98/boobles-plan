using Booble.CardGame.Cards;
using Booble.CardGame.Cards.DataModel;
using Booble.CardGame.Dialogues;
using Booble.CardGame.Level;
using Booble.CardGame.UI;
using Booble.CardGame.Utils;
using Booble.Managers;
using Booble.UI;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Booble.CardGame.Managers.TurnManager;

namespace Booble.CardGame.Managers
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { private set; get; }

        [Header("Stats")]
        [SerializeField] private StatsUI _statsUI;

        [Header("Extended Description")]
        [SerializeField] private ExtendedDescriptionPanel _extendedDescriptionPanel;

        [Header("Turn Animation")]
        [SerializeField] private TurnAnimation _turnAnimation;

        #region Deck Remaining Cards Parameters

        [Header("Deck Remaining Cards")]
        [SerializeField] private GameObject playerDeckRemainingCardsPanel;
        [SerializeField] private TextMeshProUGUI playerDeckRemainingCardsText;

        [SerializeField] private GameObject opponentDeckRemainingCardsPanel;
        [SerializeField] private TextMeshProUGUI opponentDeckRemainingCardsText;

        #endregion

        #region Interactables Parameters

        [Header("Interactables")]

        [SerializeField] private GameObject continuePlayButton;
        [SerializeField] private GameObject cancelPlayButton;
        [SerializeField] private EndTurnButton _endTurnButton;
        #endregion

        #region Steal Cards From Deck

        [Header("Steal Cards From Deck")]
        [SerializeField] private GameObject stealCardsFromDeckButton;

        #endregion

        #region Particle Systems

        [Header("Particle Systems")]
        [SerializeField] private GameObject particleSystem_effectApply_Prefab;
        [SerializeField] private GameObject particleSystem_effectTargetPositive_Prefab;
        [SerializeField] private GameObject particleSystem_effectTargetNegative_Prefab;

        #endregion

        #region Interview Start

        [Header("Interview Start")]
        [SerializeField] private GameObject _playerBanner;
        [SerializeField] private GameObject _opponentBanner;

        private bool _bannersOn = true;

        #endregion

        #region Lose Menu

        [Header("Lose Menu")]
        [SerializeField] private GameObject _loseMenu;

        public bool loseMenuActive => _loseMenu.activeSelf;

        #endregion


        private void Awake()
        {
            Instance = this;
        }

        private void Update()
        {
            CheckMoveBanners();
        }

        #region Inputs

        private void CheckMoveBanners()
        {
            if (Input.GetMouseButtonUp(0) && _bannersOn) MoveBanners();
        }

        #endregion

        #region Turn Animation

        public void TurnAnimation(Turn turn)
        {
            _turnAnimation.SetTurnAnimation(turn);
            SetEndTurnButtonText(turn);
        }

        #endregion

        #region Stats

        public GameObject GetPlayerHealthBar() => _statsUI.GetPlayerHealthBar();
        public GameObject GetPlayerManaCounter() => _statsUI.GetPlayerManaCounter();

        public bool statsUpdated => _statsUI.statsUpdated;
        public void UpdateUIStats(bool hideEmptyCristals = false) { _statsUI.UpdateUIStats(hideEmptyCristals); }
        public void UpdateMaxMana(Contender contender, int mana) { _statsUI.UpdateMaxMana(contender, mana); }
        public void SetShownMana(int mana) { _statsUI.SetShownMana(mana); }

        #endregion

        #region End Turn Button

        public void SetEndTurnButtonText(Turn turn) { _endTurnButton.SetEndTurnButtonText(turn); }
        public void SetEndTurnButtonInteractable(bool value = true)
        {
            if (_endTurnButton.IsEndTurnButtonInteractable() != value)
            {
                _endTurnButton.SetEndTurnButtonInteractable(value);
            }
        }
        public bool IsEndTurnButtonInteractable() => _endTurnButton.IsEndTurnButtonInteractable();

        #endregion

        #region Extended Description

        public void ShowExtendedDescription(CardsData data)
        {
            _extendedDescriptionPanel.Show(data);
        }

        public void HideExtendedDescription()
        {
            _extendedDescriptionPanel.Hide();
        }

        #endregion

        #region Deck Remaining Cards

        public void ShowRemainingCards(Contender contender)
        {
            if (contender.role == Contender.Role.PLAYER) playerDeckRemainingCardsPanel.SetActive(true);
            else opponentDeckRemainingCardsPanel.SetActive(true);
        }

        public void UpdateRemainingCards(int remainingCards, int maxCards, Contender contender)
        {
            if (remainingCards < 0) remainingCards = 0;
            if (contender.isPlayer) playerDeckRemainingCardsText.text = remainingCards + " / " + maxCards;
            else opponentDeckRemainingCardsText.text = remainingCards + " / " + maxCards;
        }

        #endregion

        #region Play Buttons

        public void OnContinuePlayButtonClick()
        {
            Card card = MouseController.Instance.holdingCard;
            card.ContinueAction();
        }

        public void OnCancelPlayButtonClick()
        {
            Card holdingCard = MouseController.Instance.holdingCard;
            if (holdingCard != null && holdingCard.IsPlayerCard)
            {
                holdingCard.contender.hand.AddCard(holdingCard.gameObject);

                if (MouseController.Instance.IsApplyingEffect) MouseController.Instance.ResetApplyingEffect();
                if (holdingCard.IsAction) Board.Instance.RemoveTargetsHighlight();
                else Board.Instance.RemoveCardZonesHighlight(holdingCard);

                HidePlayButtons();
                SetEndTurnButtonInteractable();
                MouseController.Instance.SetHolding(null);
                CardGameManager.Instance.SetPlayingCard(false);
            }
        }

        public void ShowContinuePlayButton()
        {
            continuePlayButton.SetActive(true);
        }
        public void ShowCancelPlayButton()
        {
            cancelPlayButton.SetActive(true);
        }

        public void HidePlayButtons()
        {
            continuePlayButton.SetActive(false);
            cancelPlayButton.SetActive(false);
        }

        #endregion

        #region Steal Cards From Deck

        public bool stealing { private set; get; }

        public void SetStealing() { stealing = true; }

        public void ShowStealCardsFromDeckButton(bool show)
        {
            stealCardsFromDeckButton.SetActive(show);
        }

        public void OnStealCardsFromDeckButtonClick()
        {
            stealing = false;
            MouseController.Instance.SetSelecting();
            ShowStealCardsFromDeckButton(false);
        }

        #endregion

        #region Interview Start

        public void InitializeBanners(Sprite image)
        {
            _opponentBanner.GetComponent<Image>().sprite = image;

            _playerBanner.SetActive(true);
            _opponentBanner.SetActive(true);
        }

        public void MoveBanners()
        {
            _bannersOn = false;
            Sequence sequence = DOTween.Sequence();

            sequence.Join(_playerBanner.transform.DOMoveX(-20, 2f));
            sequence.Join(_opponentBanner.transform.DOMoveX(20, 2f));

            sequence.OnComplete(() =>
            {
                _playerBanner.SetActive(false);
                _opponentBanner.SetActive(false);

                CardGameManager.Instance.StartInterview();
            });

            sequence.Play();
        }

        #endregion

        #region Game End

        public void InterviewEndAnimation(bool win, Action endCallback)
        {
            _turnAnimation.SetInterviewEndAnimation(win, endCallback);
            MusicManager.Instance.PlayInterviewEnd(win);
        }

        public void ShowLoseMenu()
        {
            _loseMenu.SetActive(true);
        }

        #endregion
    }
}