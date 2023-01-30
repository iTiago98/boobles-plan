using Booble.CardGame.AI;
using Booble.CardGame.Cards;
using Booble.CardGame.Dialogues;
using Booble.CardGame.Level;
using Booble.Interactables.Dialogues;
using Booble.Managers;
using Booble.UI;
using DG.Tweening;
using Santi.Utils;
using System;
using System.Collections.Generic;
using Booble.Flags;
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
        private OpponentAI _opponentAI;

        public OpponentAI opponentAI => _opponentAI;
        public void EnableOpponentAI() => _opponentAI.enabled = true;
        public void DisableOpponentAI() => _opponentAI.enabled = false;

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

        #region Tutorial

        public bool tutorial { private set; get; }
        public void StartTutorial() => tutorial = true;
        public void FinishTutorial() => tutorial = false;
        public void ContinueTutorial() => GetTutorial()?.GetTutorialAnimation().Continue();
        public TutorialDialogue GetTutorial()
        {
            if (_interviewDialogue != null && _interviewDialogue is TutorialDialogue)
            {
                return (TutorialDialogue)_interviewDialogue;
            }
            return null;
        }

        #endregion

        #region Mouse Controller

        public void EnableMouseController() => MouseController.Instance.enabled = true;
        public void DisableMouseController() => MouseController.Instance.enabled = false;

        #endregion

        #region Win Repeat Dialogue

        [SerializeField] private Dialogue _winRepeatDialogue;
        [SerializeField] private List<Option> _winRepeatOptions;

        #endregion

        public bool gamePaused { private set; get; }
        public bool playingCard { private set; get; }


        private bool _playingStoryMode;
        public bool playingStoryMode => _playingStoryMode;

        public bool dialogueEnd => (_interviewDialogue != null) ? _interviewDialogue.GetDialogueEnd() : false;

        private bool _initialized;
        private bool _gameEnded;


        private void Start()
        {
            if (!_initialized) Initialize(false);
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.S))
            {
                UnloadInterview();
            }
#endif
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

        public void InitializeDecks()
        {
            Board.Instance.InitializeDecks(DeckManager.Instance.GetPlayerCards(playingStoryMode), DeckManager.Instance.GetOpponentCards());
        }

        private void InitializeContenders()
        {
            player.Initialize(
                Board.Instance.playerHand,
                Board.Instance.playerDeck,
                Board.Instance.playerCardZone,
                Board.Instance.playerFieldCardZone);

            opponent.Initialize(
                Board.Instance.opponentHand,
                Board.Instance.opponentDeck,
                Board.Instance.opponentCardZone,
                Board.Instance.opponentFieldCardZone);

            _opponentAI = opponent.GetAIScript();
            _opponentAI.Initialize(opponent);
        }

        #endregion

        #region Game Flow 

        public void StartInterview()
        {
            if (_interviewDialogue != null && playingStoryMode) ThrowStartDialogue();
            else StartGame();
        }

        public void StartGame()
        {
            EnableMouseController();
            TurnManager.Instance.StartGame();
        }

        private bool _mouseControllerPreviousState;
        private bool _opponentAIPreviousState;
        private bool _endTurnButtonPreviousState;

        public void PauseGame()
        {
            gamePaused = true;

            _mouseControllerPreviousState = MouseController.Instance.enabled;
            _endTurnButtonPreviousState = UIManager.Instance.IsEndTurnButtonInteractable();

            if (TurnManager.Instance.IsOpponentTurn)
            {
                _opponentAIPreviousState = _opponentAI.enabled;
                DisableOpponentAI();
            }

            DisableMouseController();
            UIManager.Instance.SetEndTurnButtonInteractable(false);

            //DOTween.PauseAll();
        }

        public void ResumeGame()
        {
            gamePaused = false;

            if (TurnManager.Instance.IsOpponentTurn) _opponentAI.enabled = _opponentAIPreviousState;
            if (_mouseControllerPreviousState) EnableMouseController();
            else DisableMouseController();
            UIManager.Instance.SetEndTurnButtonInteractable(_endTurnButtonPreviousState);

            //DOTween.PlayAll();
        }

        #endregion

        #region Dialogues

        public void ThrowStartDialogue()
        {
            _interviewDialogue.ThrowStartDialogue();
        }

        public void ThrowEndDialogue()
        {
            _interviewDialogue.ThrowEndDialogue(_playerWin, GetOnEndAction());
        }

        public void ThrowRepeatDialogue()
        {
            _interviewDialogue.ThrowDialogue(_winRepeatDialogue, null, _winRepeatOptions, hideBackOption: true);
        }

        public bool CheckDialogue(Card cardPlayed)
        {
            if (!playingStoryMode) return false;
            if (_interviewDialogue == null) return false;
            return _interviewDialogue.CheckDialogue(cardPlayed);
        }

        #endregion

        #region Interview End

        public void SetAlternateWinCondition()
        {
            alternateWinCondition = true;
        }

        public bool CheckEnd()
        {
            if (!_gameEnded && (player.life <= 0 || opponent.life <= 0 || alternateWinCondition))
            {
                _gameEnded = true;
                DisableMouseController();

                _playerWin = alternateWinCondition || (player.life > 0 && opponent.life <= 0);
                OnInterviewEnd();
                return true;
            }
            else return false;
        }

        private void OnInterviewEnd()
        {
            if (alternateWinCondition)
            {
                switch (DeckManager.Instance.GetOpponentName())
                {
                    case Opponent_Name.Citriano: FlagManager.Instance.SetFlag(Flag.Reference.CitrianoVictoriaAlternativa); break;
                    case Opponent_Name.PPBros: FlagManager.Instance.SetFlag(Flag.Reference.PPBVictoriaAlternativa); break;
                    case Opponent_Name.Secretary: FlagManager.Instance.SetFlag(Flag.Reference.SecretaryVictoriaAlternativa); break;
                }
            }

            if (playingStoryMode) UIManager.Instance.InterviewEndAnimation(_playerWin, ThrowEndDialogue);
            else UIManager.Instance.InterviewEndAnimation(_playerWin, GetOnEndAction());
        }

        private Action GetOnEndAction()
        {
            if (alternateWinCondition) return UnloadInterview;
            else if (_playerWin)
            {
                if (playingStoryMode && DeckManager.Instance.HasAlternateWinConditionCard())
                {
                    return ThrowRepeatDialogue;
                }
                else
                {
                    return UnloadInterview;
                }
            }

            return UIManager.Instance.ShowLoseMenu;
        }

        public void UnloadInterview() { SceneLoader.Instance.UnloadInterviewScene(); }

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

            if (tutorial) return;

            if (playingCard) UIManager.Instance.SetEndTurnButtonInteractable(false);
            else UIManager.Instance.SetEndTurnButtonInteractable(TurnManager.Instance.IsPlayerTurn);

            if (TurnManager.Instance.IsOpponentTurn) _opponentAI.enabled = !value;
        }

        [ContextMenu("Skip interview")]
        public void SkipInterview()
        {
            UnloadInterview();
        }
    }
}