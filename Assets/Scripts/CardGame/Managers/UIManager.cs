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

        public Text playerStats;
        public Text opponentStats;
        public TextMeshProUGUI interviewEnd;
        public MyButton endTurnButton;
        public MyButton endButton;
        public EloquenceBar eloquenceBar;
        public TextMeshProUGUI log;

        private Contender _player;
        private Contender _opponent;

        private int _shownPlayerLife;
        private int _shownPlayerMana;

        private int _shownOpponentLife;
        private int _shownOpponentMana;

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
                int newPlayerMana = CheckParameterDifference(ref _shownPlayerMana, _player.currentMana);
                int newOpponnentLife = CheckParameterDifference(ref _shownOpponentLife, _opponent.eloquence);
                int newOpponnentMana = CheckParameterDifference(ref _shownOpponentMana, _opponent.currentMana);

                sequence.AppendCallback(() =>
                {
                    playerStats.text = "Health: " + newPlayerLife + "\nMana: " + newPlayerMana + "/" + _player.currentMaxMana;
                    opponentStats.text = "Health: " + newOpponnentLife + "\nMana: " + newOpponnentMana + "/" + _opponent.currentMaxMana;
                });
                sequence.AppendInterval(0.1f);
            }

            if (callback != null) sequence.AppendCallback(callback);
            sequence.Play();

            _shownPlayerLife = _player.eloquence;
            _shownPlayerMana = _player.currentMana;
            _shownOpponentLife = _opponent.eloquence;
            _shownOpponentMana = _opponent.currentMana;
        }

        private int CheckParameterDifference(ref int shownValue, int currentValue)
        {
            if (shownValue < currentValue) shownValue++;
            else if (shownValue > currentValue) shownValue--;
            return shownValue;
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
            log.text += text;
        }

        public void ShowEndButton(bool show)
        {
            endButton.gameObject.SetActive(show);
        }

        public void OnEndButtonClick()
        {
            MusicManager.Instance.StopInterviewMusic();
            SceneManager.LoadScene(Scenes.MAIN_MENU_SCENE);
        }
    }
}