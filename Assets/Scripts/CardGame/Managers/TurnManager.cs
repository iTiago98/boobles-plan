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
            START, PLAYER, OPPONENT, DISCARDING, CLASH
        }

        public Board board;

        #region Tweens

        public Sequence finishRoundSequence;

        #endregion

        public bool isPlayerTurn => _turn == Turn.PLAYER;

        private Turn _turn = Turn.START;
        public Turn turn { get { return _turn; } private set { _turn = value; } }

        public bool skipCombat;

        #region Turn flow 

        public void StartGame()
        {
            // START GAME ANIMATION

            DrawCards(CardGameManager.Instance.settings.initialCardNumber);
            UIManager.Instance.CheckEndTurnButtonState(_turn);

            StartRound();
        }

        private void StartRound()
        {
            // START ROUND ANIMATION 

            CardGameManager.Instance.FillMana();
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
                if (!CardGameManager.Instance.CheckEnd())
                {
                    // Apply end round effects
                    endTurnEffectsDelegate?.Invoke();

                    StartRound();
                }
            });

            if (skipCombat) skipCombat = false;
            else Clash();

            // END ROUND ANIMATION

            finishRoundSequence.Play();
        }

        public void ChangeTurn()
        {
            switch (_turn)
            {
                case Turn.OPPONENT:
                    SetTurn(Turn.PLAYER);
                    CardGameManager.Instance.opponentAI.enabled = false;
                    Board.Instance.GetHand(CardGameManager.Instance.opponent).CheckDiscarding();
                    StartTurn();
                    break;

                case Turn.PLAYER:
                    SetTurn(Turn.DISCARDING);
                    Board.Instance.GetHand(CardGameManager.Instance.player).CheckDiscarding();
                    break;

                case Turn.DISCARDING:
                    SetTurn(Turn.CLASH);
                    FinishRound();
                    break;
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
                object playerTarget = (_opponentGuardCard != null) ? _opponentGuardCard : CardGameManager.Instance.opponent;
                object opponentTarget = (_playerGuardCard != null) ? _playerGuardCard : CardGameManager.Instance.player;

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

        #region Interview End

        public void CheckEndMidTurn()
        {
            if (CardGameManager.Instance.player.eloquence <= 0
                || CardGameManager.Instance.opponent.eloquence <= 0
                || CardGameManager.Instance.alternateWinCondition)
            {
                if (turn == Turn.CLASH)
                {
                    finishRoundSequence.Kill();
                }

                SkipCombat();
                FinishRound();
            }
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

        public void SkipCombat()
        {
            skipCombat = true;
        }

        public void AlternateWinCondition()
        {
            CardGameManager.Instance.SetAlternateWinCondition();
            CheckEndMidTurn();
        }

        private void SetTurn(Turn turn)
        {
            _turn = turn;
            if (turn == Turn.OPPONENT)
                CardGameManager.Instance.opponentAI.enabled = true;

            UIManager.Instance.CheckEndTurnButtonState(_turn);
        }
    }
}