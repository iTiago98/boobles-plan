using CardGame.AI;
using CardGame.Cards;
using CardGame.Level;
using CardGame.Utils;
using DG.Tweening;
using Santi.Utils;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame.Managers
{
    public class TurnManager : Singleton<TurnManager>
    {

        public enum Turn
        {
            START, PLAYER, OPPONENT, CLASH
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

        private Turn _turn = Turn.START;
        public Turn turn { get { return _turn; } private set { _turn = value; } }

        private bool _gameStarted;
        public bool gameStarted { get { return _gameStarted; } private set { _gameStarted = value; } }


        #region Turn flow 

        public void StartGame()
        {
            // START GAME ANIMATION

            InitializeDecks();
            InitializeContenders();
            DrawCards(settings.initialCardNumber);
            UIManager.Instance.CheckEndTurnButtonState(_turn);
            gameStarted = true;
            StartRound();
        }

        private void StartRound()
        {
            // START ROUND ANIMATION 

            FillMana();
            TweenCallback callback = () =>
            {
                SetTurn(Turn.OPPONENT);
                StartTurn();
            };

            UIManager.Instance.UpdateUIStats(callback);
        }

        private void StartTurn()
        {
            // START TURN ANIMATION
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
                    StartRound();
                }
            });

            Clash();
            // END ROUND ANIMATION

            finishRoundSequence.Play();
        }

        private void ChangeTurn()
        {
            if (_turn == Turn.OPPONENT)
            {
                opponentAI.enabled = false;
                SetTurn(Turn.PLAYER);
                StartTurn();
            }
            else if (_turn == Turn.PLAYER)
            {
                SetTurn(Turn.CLASH);
                FinishRound();
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

            //UIManager.Instance.UpdateUIStats();
        }

        private void InitializeDecks()
        {
            // TODO Change player.deckCards.cards for DeckManager.Instance.GetPlayerCards()
            board.InitializeDecks(DeckManager.Instance.GetPlayerCards(), opponent.deckCards.cards);
            //board.InitializeDecks(player.deckCards.cards, opponent.deckCards.cards);
        }

        private void DrawCards(int cardNumber)
        {
            board.DrawCards(cardNumber, _turn);
        }

        private void FillMana()
        {
            player.FillMana();
            opponent.FillMana();
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
            if (turn == Turn.OPPONENT)
                opponentAI.enabled = true;

            UIManager.Instance.CheckEndTurnButtonState(_turn);
        }

        private bool CheckInterviewEnd()
        {
            if (player.eloquence <= 0 || opponent.eloquence <= 0)
            {
                UIManager.Instance.ShowEndButton(true);
                MouseController.Instance.enabled = false;

                if (player.eloquence <= 0) OnInterviewLose();
                else if (opponent.eloquence <= 0) OnInterviewWin();
                return true;
            }
            else return false;
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