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

        #region Tweens

        public Sequence finishRoundSequence;

        #endregion

        public bool isPlayerTurn => _turn == Turn.PLAYER;
        public Contender currentPlayer => (_turn == Turn.PLAYER) ? player : opponent;
        public Contender otherPlayer => (_turn == Turn.PLAYER) ? opponent : player;

        private Turn _turn;
        private bool _gameStarted;
        public bool gameStarted { get { return _gameStarted; } private set { _gameStarted = value; } }

        private void Awake()
        {
            Instance = this;
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

        public void StartGame()
        {
            InitializeDecks();
            InitializeContenders();
            SetTurn(Turn.OPPONENT);
            DrawCards(settings.initialCardNumber);
            UIManager.Instance.CheckEndTurnButtonState(_turn);
            gameStarted = true;
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
            UIManager.Instance.CheckEndTurnButtonState(_turn);
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

            UIManager.Instance.UpdateUIStats();
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
            UIManager.Instance.UpdateUIStats();
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
            UIManager.Instance.SetInterviewWinText(true);
        }

        private void OnInterviewLose()
        {
            UIManager.Instance.SetInterviewWinText(false);
        }
    }
}