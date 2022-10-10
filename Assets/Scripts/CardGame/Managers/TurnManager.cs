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

        private bool _alternateWinCondition;
        private bool _skipCombat;

        #region Turn flow 

        private void Start()
        {
            InitializeBoardBackground();
        }

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
                //Debug.Log("OnComplete");
                if (!CheckInterviewEnd())
                {
                    // Apply end round effects
                    endTurnEffectsDelegate?.Invoke();

                    StartRound();
                }
            });

            if (_skipCombat) _skipCombat = false;
            else Clash();

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

        #region Clash

        private void Clash()
        {
            //Debug.Log("Clash");
            Sequence clashSequence = ClashContinue(0);

            clashSequence.AppendInterval(1f);

            finishRoundSequence.Append(clashSequence);
        }

        private Sequence ClashContinue(int index)
        {
            //Debug.Log("Clash continue " + index);
            Sequence clashSequence = DOTween.Sequence();

            Card playerCard = board.playerCardZone[index].GetCard();
            Card opponentCard = board.opponentCardZone[index].GetCard();

            if (playerCard != null && opponentCard != null)
            {
                if (_playerGuardCard && _opponentGuardCard)
                {
                    clashSequence.Join(playerCard.HitSequence(_opponentGuardCard, () => { ApplyCombatActions(playerCard, _opponentGuardCard, false); }));
                    clashSequence.Join(opponentCard.HitSequence(_playerGuardCard, () => { ApplyCombatActions(opponentCard, _playerGuardCard, false); }));
                }
                else if (_playerGuardCard)
                {
                    clashSequence.Join(playerCard.HitSequence(opponentCard, () => { ApplyCombatActions(playerCard, opponentCard, false); }));
                    clashSequence.Join(opponentCard.HitSequence(_playerGuardCard, () => { ApplyCombatActions(opponentCard, _playerGuardCard, false); }));
                }
                else if (_opponentGuardCard)
                {
                    clashSequence.Join(playerCard.HitSequence(_opponentGuardCard, () => { ApplyCombatActions(playerCard, _opponentGuardCard, false); }));
                    clashSequence.Join(opponentCard.HitSequence(playerCard, () => { ApplyCombatActions(opponentCard, playerCard, false); }));
                }
                else
                {
                    clashSequence.Join(playerCard.HitSequence(opponentCard, () => { ApplyCombatActions(playerCard, opponentCard, true); }));
                    clashSequence.Join(opponentCard.HitSequence(playerCard, null));
                }
            }
            else
            {
                object playerTarget = (_opponentGuardCard != null) ? _opponentGuardCard : opponent;
                object opponentTarget = (_playerGuardCard != null) ? _playerGuardCard : player;

                if (playerCard != null)
                {
                    clashSequence.Join(playerCard.HitSequence(playerTarget, () => { ApplyCombatActions(playerCard, playerTarget, false); }));
                }
                else if (opponentCard != null)
                {
                    clashSequence.Join(opponentCard.HitSequence(opponentTarget, () => { ApplyCombatActions(opponentCard, opponentTarget, false); }));
                }
            }

            if (playerCard != null || opponentCard != null)
                clashSequence.AppendInterval(0.1f);

            if (index < 3) clashSequence.Append(ClashContinue(index + 1));
            else clashSequence.AppendInterval(0.5f);

            return clashSequence;
        }

        public void ApplyCombatActions(object object1, object object2, bool hitBack)
        {
            Card card1 = (object1 != null && object1 is Card) ? (Card)object1 : null;
            Card card2 = (object2 != null && object2 is Card) ? (Card)object2 : null;

            if (card1) card1.ApplyCombatEffects(object2);
            if (card2) card2.ApplyCombatEffects(object1);

            if (card1) card1.Hit(object2);

            if (card2)
            {
                if (card2.strength == 0 || !hitBack)
                    card2.CheckDestroy();
                else if (hitBack)
                    card2.Hit(object1);
            }
        }

        #endregion

        #region Initialize

        private void InitializeBoardBackground()
        {
            Board.Instance.InitializeBackground(DeckManager.Instance.GetOpponentName());
        }

        private void InitializeContenders()
        {
            player.Initialize(board.playerHand, board.playerCardZone, board.playerFieldCardZone);
            player.InitializeStats(settings.initialEloquence, settings.initialManaCounter, settings.maxManaCounter);

            opponent.Initialize(board.opponentHand, board.opponentCardZone, board.opponentFieldCardZone);
            opponent.InitializeStats(settings.initialEloquence, settings.initialManaCounter, settings.maxManaCounter);

            opponentAI.Initialize(opponent);
        }

        private void InitializeDecks()
        {
            // TODO Change player.deckCards.cards for DeckManager.Instance.GetPlayerCards()
            //board.InitializeDecks(DeckManager.Instance.GetPlayerCards(), opponent.deckCards.cards);
            board.InitializeDecks(DeckManager.Instance.GetPlayerCards(), DeckManager.Instance.GetOpponentCards());
            //board.InitializeDecks(player.deckCards.cards, opponent.deckCards.cards);
        }

        #endregion

        #region Interview End

        public void CheckEnd()
        {
            if (player.eloquence <= 0 || opponent.eloquence <= 0 || _alternateWinCondition)
            {
                if (turn == Turn.CLASH)
                {
                    finishRoundSequence.Kill();
                }

                SkipCombat();
                FinishRound();
            }
        }

        private bool CheckInterviewEnd()
        {
            if (player.eloquence <= 0 || opponent.eloquence <= 0 || _alternateWinCondition)
            {
                UIManager.Instance.ShowEndButton(true);
                MouseController.Instance.enabled = false;

                if (_alternateWinCondition) OnInterviewWin();
                else if (player.eloquence <= 0) OnInterviewLose();
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

        #endregion

        #region Play Argument Effects

        public delegate void PlayArgumentEffects();
        private PlayArgumentEffects playArgumentEffectsDelegate;

        public void AddPlayArgumentEffects(PlayArgumentEffects method)
        {
            if (playArgumentEffectsDelegate == null)
            {
                playArgumentEffectsDelegate = method;
            }
            else
            {
                playArgumentEffectsDelegate += method;
            }
        }

        public void RemovePlayArgumentEffect(PlayArgumentEffects method)
        {
            playArgumentEffectsDelegate -= method;
        }

        public void ApplyPlayArgumentEffects()
        {
            playArgumentEffectsDelegate?.Invoke();
        }

        #endregion

        #region End Turn Effects

        public delegate void EndTurnEffects();
        private EndTurnEffects endTurnEffectsDelegate;

        public void AddEndTurnEffect(EndTurnEffects method)
        {
            Debug.Log("End turn effect added");
            if (endTurnEffectsDelegate == null)
            {
                endTurnEffectsDelegate = method;
            }
            else
            {
                endTurnEffectsDelegate += method;
            }
        }

        public void RemoveEndTurnEffect(EndTurnEffects method)
        {
            endTurnEffectsDelegate -= method;
        }

        #endregion

        #region Guard Cards

        private Card _playerGuardCard;
        private Card _opponentGuardCard;

        public void SetGuardCard(Card guardCard)
        {
            if (guardCard.contender.role == Contender.Role.PLAYER)
            {
                _playerGuardCard = guardCard;
            }
            else
            {
                _opponentGuardCard = guardCard;
            }
        }

        public void RemoveGuardCard(Contender contender)
        {
            if (contender.role == Contender.Role.PLAYER)
                _playerGuardCard = null;
            else
                _opponentGuardCard = null;
        }

        #endregion

        private void DrawCards(int cardNumber)
        {
            board.DrawCards(cardNumber, _turn);
        }

        private void FillMana()
        {
            player.FillMana();
            opponent.FillMana();
        }

        public void SkipCombat()
        {
            _skipCombat = true;
        }

        public void AlternateWinCondition()
        {
            _alternateWinCondition = true;
            CheckEnd();
        }

        private void SetTurn(Turn turn)
        {
            _turn = turn;
            if (turn == Turn.OPPONENT)
                opponentAI.enabled = true;

            UIManager.Instance.CheckEndTurnButtonState(_turn);
        }

        public Contender GetContenderFromHand(Hand hand)
        {
            if (hand == board.playerHand) return player;
            else return opponent;
        }

        public Contender GetOtherContender(Contender contender)
        {
            if (contender.role == Contender.Role.PLAYER) return opponent;
            else return player;
        }
    }
}