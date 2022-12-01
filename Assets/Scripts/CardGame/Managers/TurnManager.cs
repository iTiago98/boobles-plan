using CardGame.AI;
using CardGame.Cards;
using CardGame.Level;
using CardGame.Utils;
using DG.Tweening;
using Santi.Utils;
using System;
using System.Collections;
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
            UIManager.Instance.UpdateUIStats(startRound: true);
        }

        public void StartRoundContinue()
        {
            SetTurn(Turn.OPPONENT);
            StartTurn();
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
            if (skipCombat)
            {
                skipCombat = false;
                FinishRoundContinue();
            }
            else Clash();
        }

        private void FinishRoundContinue()
        {
            // END ROUND ANIMATION

            if (!CardGameManager.Instance.CheckEnd())
            {
                // Apply end round effects
                endTurnEffectsDelegate?.Invoke();

                StartRound();
            }
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
            StartCoroutine(ClashCoroutine());
        }

        private IEnumerator ClashCoroutine()
        {
            bool combat = false;

            for (int index = 0; index < 4; index++)
            {
                combat = true;
                Sequence sequence = DOTween.Sequence();

                Card playerCard = board.playerCardZone[index].GetCard();
                Card opponentCard = board.opponentCardZone[index].GetCard();

                object playerTarget = (_opponentGuardCard != null) ? _opponentGuardCard
                    : (opponentCard != null) ? opponentCard
                    : CardGameManager.Instance.opponent;

                object opponentTarget = (_playerGuardCard != null) ? _playerGuardCard
                    : (playerCard != null) ? playerCard
                    : CardGameManager.Instance.player;

                if ((playerCard != null && playerCard.strength > 0) || (opponentCard != null && opponentCard.strength > 0))
                {
                    if (playerCard != null)
                        sequence.Join(playerCard.HitSequence(playerTarget));

                    if (opponentCard != null)
                        sequence.Join(opponentCard.HitSequence(opponentTarget));
                }

                sequence.AppendCallback(() => combat = false);
                sequence.Play();

                yield return new WaitWhile(() => combat);
            }

            FinishRoundContinue();
        }

        public void ApplyCombatActions(Card source, object targetObj)
        {
            source.ApplyCombatEffects(targetObj);
            source.Hit(targetObj);
        }

        #endregion

        #region Interview End

        public void CheckEndMidTurn()
        {
            if (CardGameManager.Instance.player.life <= 0
                || CardGameManager.Instance.opponent.life <= 0
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