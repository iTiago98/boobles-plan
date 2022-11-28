using CardGame.AI;
using CardGame.Cards;
using CardGame.Level;
using Santi.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame.Managers
{
    public class CardGameManager : Singleton<CardGameManager>
    {
        public CardGameSettings settings;

        [Header("Contenders")]
        public Contender player;
        public Contender opponent;
        public BaseAI opponentAI;

        [Header("Dialogues")]
        public InterviewDialogue citrianoInterviewDialogue;
        public InterviewDialogue ppBrosInterviewDialogue;

        private InterviewDialogue _interviewDialogue;

        public Contender currentPlayer => (TurnManager.Instance.isPlayerTurn) ? player : opponent;
        public Contender otherPlayer => (TurnManager.Instance.isPlayerTurn) ? opponent : player;

        private bool _gameStarted;
        public bool gameStarted { get { return _gameStarted; } private set { _gameStarted = value; } }

        public bool alternateWinCondition { private set; get; }
        public int alternateWinConditionParameter { get; set; }

        private void Start()
        {
            ThrowStartDialogue();
        }

        #region Initialize

        public void InitializeGame()
        {
            InitializeDecks();
            InitializeContenders();
        }

        public void Initialize()
        {
            InitializeDialogues();
            Board.Instance.InitializeBackground(DeckManager.Instance.GetOpponentName());
        }

        private void InitializeDialogues()
        {
            switch (DeckManager.Instance.GetOpponentName())
            {
                case Opponent_Name.Tutorial:
                    _interviewDialogue = null;
                    break;
                case Opponent_Name.Citriano:
                    _interviewDialogue = citrianoInterviewDialogue;
                    break;
                case Opponent_Name.PingPongBros:
                    _interviewDialogue = ppBrosInterviewDialogue;
                    break;
                case Opponent_Name.Secretary:
                    _interviewDialogue = null;
                    break;
                case Opponent_Name.Jefe:
                    _interviewDialogue = null;
                    break;
            }
        }

        private void InitializeContenders()
        {
            player.Initialize(Board.Instance.playerHand, Board.Instance.playerCardZone, Board.Instance.playerFieldCardZone);
            player.InitializeStats(settings.initialEloquence, settings.initialManaCounter, settings.maxManaCounter);

            opponent.Initialize(Board.Instance.opponentHand, Board.Instance.opponentCardZone, Board.Instance.opponentFieldCardZone);
            opponent.InitializeStats(settings.initialEloquence, settings.initialManaCounter, settings.maxManaCounter);

            opponentAI.Initialize(opponent);
        }

        private void InitializeDecks()
        {
            Board.Instance.InitializeDecks(DeckManager.Instance.GetPlayerCards(), DeckManager.Instance.GetOpponentCards());
        }

        #endregion

        #region Game Flow 

        public void StartGame()
        {
            _gameStarted = true;
            InitializeGame();
            TurnManager.Instance.StartGame();
        }

        public void PauseGame()
        {
            if (!TurnManager.Instance.isPlayerTurn) opponentAI.enabled = false;
            MouseController.Instance.enabled = false;
        }

        public void ResumeGame()
        {
            if (!TurnManager.Instance.isPlayerTurn) opponentAI.enabled = true;
            MouseController.Instance.enabled = true;
        }

        #endregion

        #region Dialogues

        public void ThrowStartDialogue()
        {
            _interviewDialogue?.ThrowStartDialogue();
        }

        public void ThrowWinDialogue()
        {
            _interviewDialogue?.ThrowWinDialogue(); 
        }

        public void ThrowLoseDialogue()
        {
            _interviewDialogue?.ThrowLoseDialogue();
        }

        public void CheckDialogue(Card cardPlayed)
        {
            _interviewDialogue?.CheckDialogue(cardPlayed);
        }

        #endregion

        #region Interview End

        public void SetAlternateWinCondition()
        {
            alternateWinCondition = true;
        }

        public bool CheckEnd()
        {
            if (player.eloquence <= 0 || opponent.eloquence <= 0 || alternateWinCondition)
            {
                UIManager.Instance.ShowEndButton(true);
                MouseController.Instance.enabled = false;

                if (alternateWinCondition) OnInterviewWin();
                else if (player.eloquence <= 0) OnInterviewLose();
                else if (opponent.eloquence <= 0) OnInterviewWin();
                return true;
            }
            else return false;
        }

        private void OnInterviewWin()
        {
            UIManager.Instance.SetInterviewWinText(true);
            ThrowWinDialogue();
        }

        private void OnInterviewLose()
        {
            UIManager.Instance.SetInterviewWinText(false);
            ThrowLoseDialogue();
        }

        #endregion

        #region Getters 

        public Contender GetOtherContender(Contender contender)
        {
            if (contender.role == Contender.Role.PLAYER) return opponent;
            else return player;
        }

        #endregion

        public void FillMana()
        {
            player.FillMana();
            opponent.FillMana();
        }
    }
}