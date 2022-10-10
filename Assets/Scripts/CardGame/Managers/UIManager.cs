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
        public List<Image> playerManaList;

        public Image opponentHealthImage;
        public Image opponentExtraHealthImage;
        public List<Image> opponentManaList;

        public Sprite fullManaCristal;
        public Sprite emptyManaCristal;
        public Sprite fullExtraManaCristal;

        #endregion

        #region Extended Description

        [Header("Extended Description")]

        public GameObject extendedDescriptionPanel;
        public TextMeshProUGUI extendedDescriptionText;

        #endregion

        #region Interactables

        [Header("Interactables")]

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

        private int _shownMaxMana;

        private const string PLAYER_TURN_BUTTON_TEXT = "Batirse";
        private const string OPPONENT_TURN_BUTTON_TEXT = "Turno enemigo";
        private const string CLASH_BUTTON_TEXT = "Combate";

        private const string INTERVIEW_WIN_TEXT = "Has ganado";
        private const string INTERVIEW_LOSE_TEXT = "Has perdido";

        private void Awake()
        {
            Instance = this;

        }

        private void Start()
        {
            _player = TurnManager.Instance.player;
            _opponent = TurnManager.Instance.opponent;
        }

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
                int currentPlayerMana = _player.currentMana;

                int newOpponnentLife = CheckParameterDifference(ref _shownOpponentLife, _opponent.eloquence);
                int newOpponnentMana = _shownOpponentMana;
                int currentOpponentMana = _opponent.currentMana;

                sequence.AppendCallback(() =>
                {
                    SetStats(newPlayerLife, currentPlayerMana, newOpponnentLife, currentOpponentMana);
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

        private void SetStats(int newPlayerLife, int currentPlayerMana, int newOpponentLife, int currentOpponentMana)
        {
            SetHealth(playerHealthImage, playerExtraHealthImage, newPlayerLife);
            SetHealth(opponentHealthImage, opponentExtraHealthImage, newOpponentLife);

            if (_shownPlayerMana != currentPlayerMana) SetMana(playerManaList, ref _shownPlayerMana, currentPlayerMana);
            if (_shownOpponentMana != currentOpponentMana) SetMana(opponentManaList, ref _shownOpponentMana, currentOpponentMana);
        }

        private void SetHealth(Image healthImage, Image extraHealthImage, int life)
        {
            int maxEloquence = TurnManager.Instance.settings.initialEloquence;
            healthImage.fillAmount = (float)life / maxEloquence;

            extraHealthImage.fillAmount = (float)(life - maxEloquence) / maxEloquence;
        }

        private void SetMana(List<Image> manaList, ref int shownMana, int currentMana)
        {
            //Debug.Log("New Mana: " + shownMana + " - CurrentMana: " + currentMana);
            if (shownMana < currentMana)
            {
                //Debug.Log("1: Mana " + (shownMana) + " full");
                manaList[shownMana].sprite = (shownMana < TurnManager.Instance.settings.maxManaCounter) ? fullManaCristal : fullExtraManaCristal;
                shownMana++;
            }
            else
            {
                //Debug.Log("2: Mana " + (shownMana - 1) + " empty");
                manaList[shownMana - 1].sprite = emptyManaCristal;
                shownMana--;
            }
        }

        public void UpdateMaxMana(Contender contender, int shownMaxMana, int newMaxMana)
        {
            Sequence sequence = DOTween.Sequence();
            int loops = newMaxMana - shownMaxMana;
            _shownMaxMana = shownMaxMana;

            List<Image> manaList = (contender.role == Contender.Role.PLAYER) ? playerManaList : opponentManaList;

            for (int i = 0; i < loops; i++)
            {
                sequence.AppendCallback(() =>
                {
                    //Debug.Log("3: Mana " + (_shownMaxMana) + " empty");
                    manaList[_shownMaxMana].sprite = emptyManaCristal;
                    _shownMaxMana++;
                });
                sequence.AppendInterval(0.1f);
            }

            sequence.Play();
        }

        public void OnEndTurnButtonClick()
        {
            if (TurnManager.Instance.gameStarted)
            {
                TurnManager.Instance.FinishTurn();
            }
            else
            {
                TurnManager.Instance.StartGame();
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
                case Turn.PLAYER:
                    endTurnButton.SetInteractable(true);
                    endTurnButton.SetText(PLAYER_TURN_BUTTON_TEXT);
                    break;
                case Turn.OPPONENT:
                    endTurnButton.SetInteractable(false);
                    endTurnButton.SetText(OPPONENT_TURN_BUTTON_TEXT);
                    break;
                case Turn.CLASH:
                    endTurnButton.SetInteractable(false);
                    endTurnButton.SetText(CLASH_BUTTON_TEXT);
                    break;
            }
        }

        public void SetInterviewWinText(bool win)
        {
            if (win) interviewEnd.text = INTERVIEW_WIN_TEXT;
            else interviewEnd.text = INTERVIEW_LOSE_TEXT;
        }

        public void AddToLog(string text)
        {
            //log.text += text;
        }

        public void ShowEndButton(bool show)
        {
            endButton.gameObject.SetActive(show);
        }

        public void OnEndButtonClick()
        {
            SceneLoader.Instance.ReturnToLoungeScene();
        }

        public void ShowExtendedDescription(string text)
        {
            extendedDescriptionText.text = text;
            extendedDescriptionPanel.SetActive(true);
        }

        public void HideExtendedDescription()
        {
            extendedDescriptionPanel.SetActive(false);
        }
    }
}