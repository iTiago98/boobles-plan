using CardGame.Cards;
using CardGame.Level;
using CardGame.Utils;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static CardGame.Managers.TurnManager;

namespace CardGame.Managers
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { private set; get; }

        #region Stats

        [Header("Contenders Stats")]

        public Image playerHealthImage;
        public Image playerExtraHealthImage;
        public Image playerExtraHealthImage2;
        public List<Image> playerManaList;

        public Image opponentHealthImage;
        public Image opponentExtraHealthImage;
        public Image opponentExtraHealthImage2;
        public List<Image> opponentManaList;

        public Sprite fullManaCristal;
        public Sprite emptyManaCristal;
        public Sprite fullExtraManaCristal;

        #endregion

        #region Extended Description

        [Header("Extended Description")]

        public GameObject extendedDescriptionPanel;

        public TextMeshProUGUI extendedDescriptionName;
        public TextMeshProUGUI extendedDescriptionType;
        public TextMeshProUGUI extendedDescriptionText;

        #endregion

        #region Deck Remaining Cards

        [Header("Deck Remaining Cards")]
        public GameObject playerDeckRemainingCardsPanel;
        public TextMeshProUGUI playerDeckRemainingCardsText;

        public GameObject opponentDeckRemainingCardsPanel;
        public TextMeshProUGUI opponentDeckRemainingCardsText;

        #endregion

        #region Interactables

        [Header("Interactables")]

        public MyButton continuePlayButton;
        public MyButton cancelPlayButton;

        public TextMeshProUGUI interviewEnd;
        public MyButton endTurnButton;
        public MyButton endButton;

        #endregion

        private Contender _player;
        private Contender _opponent;

        private int _shownPlayerLife;
        private int _shownPlayerMana;

        private int _shownOpponentLife;
        private int _shownOpponentMana;

        private int _defaultMaxMana;

        private const string PLAYER_TURN_BUTTON_TEXT = "Batirse";
        private const string OPPONENT_TURN_BUTTON_TEXT = "Turno enemigo";
        private const string DISCARDING_BUTTON_TEXT = "Descartando";
        private const string CLASH_BUTTON_TEXT = "Combate";
        private const string SKIP_COMBAT_BUTTON_TEXT = "Pasar turno";

        private const string INTERVIEW_WIN_TEXT = "Has ganado";
        private const string INTERVIEW_LOSE_TEXT = "Has perdido";

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _player = CardGameManager.Instance.player;
            _opponent = CardGameManager.Instance.opponent;
            _defaultMaxMana = CardGameManager.Instance.settings.maxManaCounter;
        }

        #region Stats

        public void UpdateUIStats()
        {
            UpdateUIStats(null);
        }

        public void UpdateUIStats(TweenCallback callback)
        {
            Sequence sequence = DOTween.Sequence();

            int loops = Mathf.Max(
                Mathf.Abs(_player.eloquence - _shownPlayerLife),
                Mathf.Abs(_player.currentMana - _shownPlayerMana),
                Mathf.Abs(_opponent.eloquence - _shownOpponentLife),
                Mathf.Abs(_opponent.currentMana - _shownOpponentMana)
                );

            for (int i = 0; i < loops; i++)
            {
                int newPlayerLife = CheckParameterDifference(ref _shownPlayerLife, _player.eloquence);
                int newPlayerMana = _shownPlayerMana;
                int playerCurrentMana = _player.currentMana;
                int playerCurrentMaxMana = _player.currentMaxMana;
                int playerExtraMana = _player.extraMana;

                int newOpponnentLife = CheckParameterDifference(ref _shownOpponentLife, _opponent.eloquence);
                int newOpponnentMana = _shownOpponentMana;
                int opponentCurrentMana = _opponent.currentMana;
                int opponentCurrentMaxMana = _opponent.currentMaxMana;
                int opponentExtraMana = _opponent.extraMana;

                sequence.AppendCallback(() =>
                {
                    SetStats(newPlayerLife, playerCurrentMana, playerCurrentMaxMana, playerExtraMana,
                        newOpponnentLife, opponentCurrentMana, opponentCurrentMaxMana, opponentExtraMana);
                });
                sequence.AppendInterval(0.1f);
            }

            if (callback != null) sequence.AppendCallback(callback);
            sequence.Play();

            _shownPlayerLife = _player.eloquence;
            _shownOpponentLife = _opponent.eloquence;
        }

        private int CheckParameterDifference(ref int shownValue, int currentValue)
        {
            if (shownValue < currentValue) shownValue++;
            else if (shownValue > currentValue) shownValue--;
            return shownValue;
        }

        private void SetStats(int newPlayerLife, int playerCurrentMana, int playerCurrentMaxMana, int playerExtraMana,
            int newOpponentLife, int opponentCurrentMana, int opponentCurrentMaxMana, int opponentExtraMana)
        {
            SetHealth(playerHealthImage, playerExtraHealthImage, playerExtraHealthImage2, newPlayerLife);
            SetHealth(opponentHealthImage, opponentExtraHealthImage, opponentExtraHealthImage2, newOpponentLife);

            if (_shownPlayerMana != playerCurrentMana) SetMana(playerManaList, ref _shownPlayerMana, playerCurrentMana, playerCurrentMaxMana, playerExtraMana);
            if (_shownOpponentMana != opponentCurrentMana) SetMana(opponentManaList, ref _shownOpponentMana, opponentCurrentMana, opponentCurrentMaxMana, opponentExtraMana);
        }

        private void SetHealth(Image healthImage, Image extraHealthImage, Image extraHealthImage2, int life)
        {
            int maxEloquence = CardGameManager.Instance.settings.initialEloquence;
            healthImage.fillAmount = (float)life / maxEloquence;

            if (life >= maxEloquence) extraHealthImage.fillAmount = (float)(life - maxEloquence) / maxEloquence;
            if (life >= (maxEloquence * 2)) extraHealthImage2.fillAmount = (float)(life - (maxEloquence * 2)) / maxEloquence;
        }

        private void SetMana(List<Image> manaList, ref int shownMana, int currentMana, int currentMaxMana, int extraMana)
        {
            //Debug.Log("Shown Mana: " + shownMana + " - CurrentMana: " + currentMana);
            if (shownMana < currentMana)
            {
                //Debug.Log("1: Mana " + (shownMana) + " full");
                if (IsExtraMana(extraMana, currentMaxMana, shownMana))
                    manaList[_defaultMaxMana + (shownMana - currentMaxMana + extraMana)].sprite = fullExtraManaCristal;
                
                else manaList[shownMana].sprite = fullManaCristal;

                shownMana++;
            }
            else
            {
                //Debug.Log("2: Mana " + (shownMana - 1) + " empty");
                shownMana--;
                int index;
                if (IsExtraMana(extraMana, currentMaxMana, shownMana)) index = _defaultMaxMana + (shownMana - currentMaxMana + extraMana);
                else index = shownMana;

                manaList[index].sprite = emptyManaCristal;
            }
        }

        private bool IsExtraMana(int extraMana, int currentMaxMana, int shownMana)
        {
            return extraMana > 0
                && (currentMaxMana - shownMana) <= extraMana
                && (currentMaxMana - extraMana) <= _defaultMaxMana;
        }

        public void UpdateMaxMana(Contender contender, int newMaxMana)
        {
            List<Image> manaList = (contender.role == Contender.Role.PLAYER) ? playerManaList : opponentManaList;
            manaList[newMaxMana - 1].sprite = emptyManaCristal;
        }

        #endregion

        #region End Turn Button

        public void OnEndTurnButtonClick()
        {
            if (CardGameManager.Instance.gameStarted)
            {
                TurnManager.Instance.FinishTurn();
            }
            else
            {
                CardGameManager.Instance.StartGame();
            }
        }

        public void SetEndTurnButtonInteractable(bool interactable)
        {
            endTurnButton.SetInteractable(interactable);
        }

        public void CheckEndTurnButtonState(Turn turn)
        {
            switch (turn)
            {
                case Turn.START:
                    endTurnButton.SetInteractable(false);
                    break;
                case Turn.OPPONENT:
                    endTurnButton.SetInteractable(false);
                    endTurnButton.SetText(OPPONENT_TURN_BUTTON_TEXT);
                    break;
                case Turn.PLAYER:
                    endTurnButton.SetInteractable(true);
                    if (TurnManager.Instance.skipCombat)
                        endTurnButton.SetText(SKIP_COMBAT_BUTTON_TEXT);
                    else 
                        endTurnButton.SetText(PLAYER_TURN_BUTTON_TEXT);
                    break;
                case Turn.DISCARDING:
                    endTurnButton.SetInteractable(false);
                    endTurnButton.SetText(DISCARDING_BUTTON_TEXT);
                    break;
                case Turn.CLASH:
                    endTurnButton.SetInteractable(false);
                    endTurnButton.SetText(CLASH_BUTTON_TEXT);
                    break;
            }
        }

        #endregion

        #region Extended Description

        public void ShowExtendedDescription(string name, string type, string description)
        {
            extendedDescriptionName.text = name;
            extendedDescriptionType.text = type;
            extendedDescriptionText.text = description;
            extendedDescriptionPanel.SetActive(true);
        }

        public void HideExtendedDescription()
        {
            extendedDescriptionPanel.SetActive(false);
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
            if (contender.role == Contender.Role.PLAYER) playerDeckRemainingCardsText.text = remainingCards + " / " + maxCards;
            else opponentDeckRemainingCardsText.text = remainingCards + " / " + maxCards;
        }

        #endregion

        public void OnContinuePlayButtonClick()
        {
            Card card = MouseController.Instance.holdingCard;
            if (card.IsPlayerCard) card.ContinuePlay();
            else card.ContinuePlayOpponent();
            HidePlayButtons();
        }

        public void OnCancelPlayButtonClick()
        {
            Card holdingCard = MouseController.Instance.holdingCard;
            if (holdingCard != null)
            {
                holdingCard.CancelPlay();

                if (MouseController.Instance.IsApplyingEffect)
                    MouseController.Instance.ResetApplyingEffect();
                else
                    Board.Instance.HighlightZoneTargets(holdingCard.type, holdingCard.contender, show: false);
            }

            HidePlayButtons();
        }

        public void ShowContinuePlayButton()
        {
            continuePlayButton.gameObject.SetActive(true);
        }
        public void ShowCancelPlayButton()
        {
            cancelPlayButton.gameObject.SetActive(true);
        }

        public void HidePlayButtons()
        {
            continuePlayButton.gameObject.SetActive(false);
            cancelPlayButton.gameObject.SetActive(false);
        }

        public void SetInterviewWinText(bool win)
        {
            if (win) interviewEnd.text = INTERVIEW_WIN_TEXT;
            else interviewEnd.text = INTERVIEW_LOSE_TEXT;
        }

        public void ShowEndButton(bool show)
        {
            endButton.gameObject.SetActive(show);
        }

        public void OnEndButtonClick()
        {
            SceneLoader.Instance.UnloadInterviewScene();
        }
    }
}