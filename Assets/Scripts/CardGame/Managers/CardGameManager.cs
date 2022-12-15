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
        [HideInInspector] public Contender opponent;
        [HideInInspector] public OpponentAI opponentAI;

        [SerializeField] private Transform _opponentParent;
        [SerializeField] private List<GameObject> _opponents;

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

        public void Initialize()
        {
            InitializeOpponent();
            InitializeDialogues();
            Board.Instance.InitializeBackground(opponent.GetInterviewBackground());
        }

        private void InitializeOpponent()
        {
            GameObject opponentObj = Instantiate(
                _opponents[(int)DeckManager.Instance.GetOpponentName()],
                _opponentParent);

            opponent = opponentObj.GetComponent<Contender>();

            DeckManager.Instance.SetOpponentCards(opponent.GetDeckCards());
        }

        private void InitializeDialogues()
        {
            _interviewDialogue = opponent.GetInterviewDialogue();
        }

        public void InitializeGame()
        {
            InitializeDecks();
            InitializeContenders();
        }
       
        private void InitializeDecks()
        {
            Board.Instance.InitializeDecks(DeckManager.Instance.GetPlayerCards(), DeckManager.Instance.GetOpponentCards());
        }

        private void InitializeContenders()
        {
            player.Initialize(
                Board.Instance.playerHand, 
                Board.Instance.playerDeck, 
                Board.Instance.playerCardZone, 
                Board.Instance.playerFieldCardZone);

            player.InitializeStats(
                settings.initialLife, 
                settings.initialManaCounter, 
                settings.maxManaCounter);


            opponent.Initialize(
                Board.Instance.opponentHand, 
                Board.Instance.opponentDeck,
                Board.Instance.opponentCardZone, 
                Board.Instance.opponentFieldCardZone);

            opponent.InitializeStats(
                settings.initialLife, 
                settings.initialManaCounter, 
                settings.maxManaCounter);

            opponentAI = opponent.GetAIScript();
            opponentAI.Initialize(opponent);
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
            if (player.life <= 0 || opponent.life <= 0 || alternateWinCondition)
            {
                UIManager.Instance.ShowEndButton(true);
                MouseController.Instance.enabled = false;

                if (alternateWinCondition) OnInterviewWin();
                else if (player.life <= 0) OnInterviewLose();
                else if (opponent.life <= 0) OnInterviewWin();
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