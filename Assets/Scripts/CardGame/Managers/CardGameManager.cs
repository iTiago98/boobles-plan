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

        #region Contenders

        [Header("Contenders")]
        public Contender player;
        [HideInInspector] public Contender opponent;
        [HideInInspector] public OpponentAI opponentAI;

        [SerializeField] private Transform _opponentParent;
        [SerializeField] private List<GameObject> _opponents;

        private InterviewDialogue _interviewDialogue;

        public Contender currentPlayer => (TurnManager.Instance.IsPlayerTurn) ? player : opponent;
        public Contender otherPlayer => (TurnManager.Instance.IsPlayerTurn) ? opponent : player;

        #endregion

        #region Interview End Parameters

        public bool alternateWinCondition { private set; get; }
        public int alternateWinConditionParameter { get; set; }

        private bool _playerWin;

        #endregion

        public bool gamePaused { private set; get; }
        public bool playingCard { private set; get; }

        private bool _initialized;
        private bool _playingStoryMode;
        public bool playingStoryMode => _playingStoryMode;

        public bool dialogueEnd => (_interviewDialogue != null) ? _interviewDialogue.GetDialogueEnd() : false;

        private void Start()
        {
            if (!_initialized) Initialize(false);
        }

        #region Initialize

        public void Initialize(bool playingStoryMode)
        {
            _initialized = true;
            _playingStoryMode = playingStoryMode;

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

        public void SwitchGameState()
        {
            if (gamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        private bool _mouseControllerPreviousState;
        private bool _opponentAIPreviousState;
        private bool _endTurnButtonPreviousState;

        public void PauseGame()
        {
            gamePaused = true;
            
            _mouseControllerPreviousState = MouseController.Instance.enabled;
            _endTurnButtonPreviousState = UIManager.Instance.IsEndTurnButtonInteractable();

            if (!TurnManager.Instance.IsPlayerTurn)
            {
                _opponentAIPreviousState = opponentAI.enabled;
                opponentAI.enabled = false;
            }

            MouseController.Instance.enabled = false;
            UIManager.Instance.SetEndTurnButtonInteractable(false);

            DOTween.PauseAll();
        }

        public void ResumeGame()
        {
            gamePaused = false;

            if (!TurnManager.Instance.IsPlayerTurn) opponentAI.enabled = _opponentAIPreviousState;
            MouseController.Instance.enabled = _mouseControllerPreviousState;
            UIManager.Instance.SetEndTurnButtonInteractable(_endTurnButtonPreviousState);

            DOTween.PlayAll();
        }

        #endregion

        #region Dialogues

        public void ThrowStartDialogue()
        {
            _interviewDialogue?.ThrowStartDialogue();
            if (_interviewDialogue == null) StartGame(); // TEMPORAL
        }

        public void ThrowEndDialogue()
        {
            _interviewDialogue?.ThrowEndDialogue(_playerWin);
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
                MouseController.Instance.enabled = false;

                _playerWin = alternateWinCondition || (player.life > 0 && opponent.life <= 0);
                OnInterviewEnd();
                return true;
            }
            else return false;
        }

        private void OnInterviewEnd()
        {
            UIManager.Instance.InterviewEndAnimation(_playerWin, ThrowEndDialogue);
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