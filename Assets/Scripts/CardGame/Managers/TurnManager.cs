using CardGame.AI;
using CardGame.Cards;
using CardGame.Cards.DataModel.Effects;
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

        public bool isPlayerTurn => _turn == Turn.PLAYER;

        private Turn _turn = Turn.START;
        public Turn turn { get { return _turn; } private set { _turn = value; } }

        public bool skipCombat;

        #region Turn flow 

        public void StartGame()
        {
            StartCoroutine(StartGameCoroutine());
        }

        private IEnumerator StartGameCoroutine()
        {
            // START GAME ANIMATION
            StopFlow();
            UIManager.Instance.TurnAnimation(turn);

            yield return new WaitUntil(() => continueFlow);

            // DRAW CARDS
            StopFlow();
            DrawCards(CardGameManager.Instance.settings.initialCardNumber);

            yield return new WaitUntil(() => continueFlow);

            StartRound();
        }

        private void StartRound()
        {
            StartCoroutine(StartRoundCoroutine());
        }

        private IEnumerator StartRoundCoroutine()
        {
            // UPDATE HEALTH AND MANA
            StopFlow();
            CardGameManager.Instance.FillMana();
            UIManager.Instance.UpdateUIStats(startRound: true);

            yield return new WaitUntil(() => continueFlow);

            ChangeTurn();
            StartTurn();
        }

        private void StartTurn()
        {
            StartCoroutine(StartTurnCoroutine());
        }

        private IEnumerator StartTurnCoroutine()
        {
            // START TURN ANIMATION
            StopFlow();
            UIManager.Instance.TurnAnimation(turn);

            yield return new WaitUntil(() => continueFlow);

            // DRAW CARD
            StopFlow();
            DrawCards(1);

            yield return new WaitUntil(() => continueFlow);

            if (turn == Turn.OPPONENT)
                CardGameManager.Instance.opponentAI.enabled = true;
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
            else
                StartCoroutine(FinishRoundCoroutine());
        }

        private void FinishRoundContinue()
        {
            if (!CardGameManager.Instance.CheckEnd())
            {
                // Apply end round effects
                if (endTurnEffectsDelegate != null)
                    endTurnEffectsDelegate.Invoke();

                StartRound();
            }
        }

        private IEnumerator FinishRoundCoroutine()
        {
            // CLASH TURN ANIMATION
            StopFlow();
            UIManager.Instance.TurnAnimation(turn);

            yield return new WaitUntil(() => continueFlow);

            Clash();
        }

        public void ChangeTurn()
        {
            switch (_turn)
            {
                case Turn.START:
                case Turn.CLASH:
                    SetTurn(Turn.OPPONENT);
                    break;

                case Turn.OPPONENT:
                    SetTurn(Turn.PLAYER);
                    CardGameManager.Instance.opponentAI.enabled = false;
                    CardGameManager.Instance.opponent.hand.CheckDiscarding();
                    StartTurn();
                    break;

                case Turn.PLAYER:
                    SetTurn(Turn.DISCARDING);
                    CardGameManager.Instance.player.hand.CheckDiscarding();
                    break;

                case Turn.DISCARDING:
                    SetTurn(Turn.CLASH);
                    FinishRound();
                    break;
            }

        }

        public void StopFlow()
        {
            continueFlow = false;
        }

        public void ContinueFlow()
        {
            continueFlow = true;
        }

        #endregion

        #region Clash

        private void Clash()
        {
            StartCoroutine(ClashCoroutine());
        }

        //public bool clashing;
        public bool continueFlow;

        private IEnumerator ClashCoroutine()
        {
            //clashing = true;
            ContinueFlow();

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
                yield return new WaitUntil(() => continueFlow);

                playerCard?.CheckDestroy();
                opponentCard?.CheckDestroy();
            }

            FinishRoundContinue();
        }

        public void ApplyCombatActions(Card source, object targetObj)
        {
            source.ApplyCombatEffects(targetObj);
            if ((targetObj is Card) && (source.HasEffect(SubType.REBOUND) || ((Card)targetObj).HasEffect(SubType.REBOUND)))
                return;

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
        private int numberEffects = 0;

        public void AddEndTurnEffect(EndTurnEffects method)
        {
            Debug.Log("End turn effect added");
            numberEffects++;
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
            numberEffects--;

            if (numberEffects == 0) endTurnEffectsDelegate = null;
        }

        #endregion

        #region Guard Cards

        private Card _playerGuardCard;
        private Card _opponentGuardCard;

        public void SetGuardCard(Card guardCard)
        {
            if (guardCard.contender.isPlayer)
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
            //if (turn == Turn.OPPONENT)
            //    CardGameManager.Instance.opponentAI.enabled = true;

            UIManager.Instance.CheckEndTurnButtonState(_turn);
        }
    }
}