using Booble.CardGame.AI;
using Booble.CardGame.Cards;
using Booble.CardGame.Dialogues;
using Booble.CardGame.Level;
using Booble.Managers;
using Booble.UI;
using DG.Tweening;
using Santi.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Booble.CardGame.Managers
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

        public Contender currentPlayer => (TurnManager.Instance.IsPlayerTurn) ? player : opponent;
        public Contender otherPlayer => (TurnManager.Instance.IsPlayerTurn) ? opponent : player;

        public bool gamePaused { private set; get; }

        public bool playingCard { private set; get; }


        public bool alternateWinCondition { private set; get; }
        public int alternateWinConditionParameter { get; set; }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) || PauseMenu.Instance.hide)
            {
                PauseMenu.Instance.ShowHidePauseMenu();
                if (gamePaused) ResumeGame();
                else PauseGame();
            }
        }

        #region Initialize

        public void Initialize()
        {
            InitializeOpponent();
            InitializeDialogues();
            Board.Instance.InitializeBackground(opponent.GetInterviewBackground());
            UIManager.Instance.InitializeBanners(opponent.GetInterviewBanner());
            InitializeDecks();
            InitializeContenders();
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
            TurnManager.Instance.StartGame();
        }

        public void PauseGame()
        {
            gamePaused = true;
            if (!TurnManager.Instance.IsPlayerTurn) opponentAI.enabled = false;
            MouseController.Instance.enabled = false;
            DOTween.PauseAll();
        }

        public void ResumeGame()
        {
            gamePaused = false;
            if (!TurnManager.Instance.IsPlayerTurn) opponentAI.enabled = true;
            MouseController.Instance.enabled = true;
            DOTween.PlayAll();
        }

        #endregion

        #region Dialogues

        public void ThrowStartDialogue()
        {
            _interviewDialogue?.ThrowStartDialogue();
            if (_interviewDialogue == null) StartGame(); // TEMPORAL
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

        public void SetPlayingCard(bool value)
        {
            playingCard = value;

            if (playingCard) UIManager.Instance.SetEndTurnButtonInteractable(false);
            else UIManager.Instance.SetEndTurnButtonInteractable(TurnManager.Instance.IsPlayerTurn);
        }
    }
}