using CardGame.AI;
using CardGame.Cards;
using CardGame.Level;
using CardGame.Utils;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.Managers
{
    public class TurnManager : MonoBehaviour
    {

        public static TurnManager Instance { private set; get; }

        public enum Turn
        {
            PLAYER, OPPONENT, CLASH
        }

        public CardGameSettings settings;

        public Board board;

        [Header("Contenders")]
        public Contender player;
        public Contender opponent;

        public BaseAI opponentAI;

        #region UI

        [Header("UI")]
        public Text playerStats;
        public Text opponentStats;
        public TextMeshProUGUI interviewEnd;
        public MyButton endTurnButton;
        public EloquenceBar eloquenceBar;

        #endregion

        #region Tweens

        public Sequence finishRoundSequence;

        #endregion

        public bool isPlayerTurn => _turn == Turn.PLAYER;
        public Contender currentPlayer => (_turn == Turn.PLAYER) ? player : opponent;
        public Contender otherPlayer => (_turn == Turn.PLAYER) ? opponent : player;

        private Turn _turn;

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
            StartGame();
        }

        private void Update()
        {
            // DEBUG
            //if (Input.GetKeyDown(KeyCode.T))
            //{
            //    FinishTurn();
            //}
        }

        #region Turn flow 

        private void StartGame()
        {
            InitializeDecks();
            InitializeContenders();
            SetTurn(Turn.OPPONENT);
            DrawCards(settings.initialCardNumber);
            CheckEndTurnButtonState();
        }

        private void StartRound()
        {
            FillMana();
            DrawCards(1);
        }

        public void FinishTurn()
        {
            ChangeTurn();
        }

        private void FinishRound()
        {
            finishRoundSequence = DOTween.Sequence();
            finishRoundSequence.OnComplete(() =>
            {
                if (!CheckInterviewEnd())
                {
                    ChangeTurn();
                    StartRound();
                }
            });

            Clash();
            //END ROUND ANIMATION

            finishRoundSequence.Play();
        }

        private void ChangeTurn()
        {
            if (_turn == Turn.OPPONENT)
            {
                opponentAI.enabled = false;
                SetTurn(Turn.PLAYER);
            }
            else if (_turn == Turn.PLAYER)
            {
                SetTurn(Turn.CLASH);
                FinishRound();
            }
            else
            {
                SetTurn(Turn.OPPONENT);
            }
            CheckEndTurnButtonState();
        }

        #endregion

        #region UI

        public void UpdateUIStats()
        {
            playerStats.text = "Health: " + player.eloquence + "\nMana: " + player.currentMana + "/" + player.currentMaxMana;
            opponentStats.text = "Health: " + opponent.eloquence + "\nMana: " + opponent.currentMana + "/" + opponent.currentMaxMana;
        }

        public void SetEndButtonInteractable(bool interactable)
        {
            endTurnButton.SetInteractable(interactable);
        }

        public void CheckEndTurnButtonState()
        {
            switch (_turn)
            {
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

        #endregion

        public Contender GetContenderFromHand(Hand hand)
        {
            if (hand == board.playerHand) return player;
            else return opponent;
        }

        private void InitializeContenders()
        {
            player.Initialize(board.playerHand, board.playerCardZone);
            player.InitializeStats(settings.initialEloquence, settings.initialManaCounter, settings.maxManaCounter);

            opponent.Initialize(board.opponentHand, board.opponentCardZone);
            opponent.InitializeStats(settings.initialEloquence, settings.initialManaCounter, settings.maxManaCounter);

            opponentAI.Initialize(opponent);

            UpdateUIStats();
        }

        private void InitializeDecks()
        {
            board.InitializeDecks(player.deckCards.cards, opponent.deckCards.cards);
        }

        private void DrawCards(int cardNumber)
        {
            board.DrawCards(cardNumber, null);
        }

        private void FillMana()
        {
            player.FillMana();
            opponent.FillMana();
            UpdateUIStats();
        }

        private void Clash()
        {
            Sequence clashSequence = DOTween.Sequence();

            for (int i = 0; i < board.playerCardZone.Count; i++)
            {
                Card playerCard = board.playerCardZone[i].GetCard();
                Card opponentCard = board.opponentCardZone[i].GetCard();

                if (playerCard != null && opponentCard != null)
                {
                    clashSequence.Join(playerCard.Hit(opponentCard));
                    clashSequence.Join(opponentCard.Hit(playerCard));
                }
                else if (playerCard != null)
                {
                    clashSequence.Join(playerCard.Hit(opponent));
                }
                else if (opponentCard != null)
                {
                    clashSequence.Join(opponentCard.Hit(player));
                }

                if (playerCard != null || opponentCard != null)
                    clashSequence.AppendInterval(0.1f);
            }

            clashSequence.AppendInterval(1f);

            finishRoundSequence.Append(clashSequence);
        }

        private void SetTurn(Turn turn)
        {
            _turn = turn;
            if (turn == Turn.OPPONENT) opponentAI.enabled = true;
        }

        private bool CheckInterviewEnd()
        {
            if (player.eloquence <= 0) OnInterviewLose();
            else if (opponent.eloquence <= 0) OnInterviewWin();
            else return false;
            return true;
        }

        private void OnInterviewWin()
        {
            interviewEnd.text = INTERVIEW_WIN_TEXT;
        }

        private void OnInterviewLose()
        {
            interviewEnd.text = INTERVIEW_LOSE_TEXT;
        }
    }
}