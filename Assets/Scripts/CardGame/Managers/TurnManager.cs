using CardGame.AI;
using CardGame.Cards;
using CardGame.Cards.DataModel.Effects;
using CardGame.Level;
using CardGame.Utils;
using DG.Tweening;
using Santi.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
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


        // Skip Combat Effect
        public bool skipCombat;

        // Mirror Effect
        public bool playerMirror;
        public bool opponentMirror;

        // Steal Mana Effect
        public bool playerStealMana;
        public bool opponentStealMana;

        #region Turn flow 

        public void StartGame()
        {
            StartCoroutine(StartGameCoroutine());
        }

        private IEnumerator StartGameCoroutine()
        {
            // START GAME ANIMATION
            UIManager.Instance.TurnAnimation(turn);

            yield return new WaitUntil(() => continueFlow);

            // DRAW CARDS
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
            UIManager.Instance.TurnAnimation(turn);

            yield return new WaitUntil(() => continueFlow);

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
                StartCoroutine(ApplyEndTurnEffectsCoroutine(StartRound));
            }
        }

        private IEnumerator FinishRoundCoroutine()
        {
            // CLASH TURN ANIMATION
            UIManager.Instance.TurnAnimation(turn);
            RemoveStealMana();

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

        private void SetTurn(Turn turn)
        {
            _turn = turn;

            UIManager.Instance.CheckEndTurnButtonState(_turn);
        }

        public void StopFlow()
        {
            continueFlow = false;
        }

        public void ContinueFlow()
        {
            continueFlow = true;
        }

        private void DrawCards(int cardNumber)
        {
            StopFlow();
            board.DrawCards(cardNumber, _turn);
        }

        #endregion

        #region Clash

        private void Clash()
        {
            StartCoroutine(ClashCoroutine());
        }

        public bool continueFlow { private set; get; }
        public bool combat;

        private IEnumerator ClashCoroutine()
        {
            combat = false;

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
            if (IsHitManagedByEffect(source, targetObj))
                return;

            source.Hit(targetObj);
        }

        private bool IsHitManagedByEffect(Card source, object targetObj)
        {
            return targetObj is Card && (IsEffect(SubType.REBOUND, source, (Card)targetObj) || IsEffect(SubType.SPONGE, source, (Card)targetObj));
        }

        private bool IsEffect(SubType subType, Card source, Card target)
        {
            return source.HasEffect(subType) || target.HasEffect(subType);
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

        public void AlternateWinCondition()
        {
            CardGameManager.Instance.SetAlternateWinCondition();
            CheckEndMidTurn();
        }

        #endregion

        #region Effects

        public void SkipCombat()
        {
            skipCombat = true;
        }

        #region Play Argument Effects

        private List<Action> playArgumentEffectsActions = new List<Action>();

        public void AddPlayArgumentEffects(Action method)
        {
            playArgumentEffectsActions.Add(method);
        }

        public void RemovePlayArgumentEffect(Action method)
        {
            playArgumentEffectsActions.Remove(method);
        }

        public void ApplyPlayArgumentEffects()
        {
            foreach(Action action in playArgumentEffectsActions)
            {
                action();
            }
        }

        #endregion

        #region End Turn Effects

        private List<Action> endTurnEffectsActions = new List<Action>();

        public void AddEndTurnEffect(Action method)
        {
            Debug.Log("End turn effect added");
            endTurnEffectsActions.Add(method);
        }

        public void RemoveEndTurnEffect(Action method)
        {
            endTurnEffectsActions.Remove(method);
        }

        private IEnumerator ApplyEndTurnEffectsCoroutine(Action followUp)
        {
            foreach (Action endTurnEffect in endTurnEffectsActions)
            {
                StopFlow();
                endTurnEffect();
                Debug.Log(endTurnEffectsActions.IndexOf(endTurnEffect));
                yield return new WaitUntil(() => continueFlow);
            }

            followUp();
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

        #region Mirror

        public void SetMirror(Contender contender, bool state)
        {
            if (contender.isPlayer) playerMirror = state;
            else opponentMirror = state;
        }
        public bool IsMirror(Contender contender)
        {
            return (contender.isPlayer && playerMirror) || (!contender.isPlayer && opponentMirror);
        }

        #endregion

        #region Steal Mana

        public void SetStealMana(Contender contender)
        {
            if (contender.isPlayer) playerStealMana = true;
            else opponentStealMana = true;
        }

        public void RemoveStealMana()
        {
            playerStealMana = false;
            opponentStealMana = false;
        }

        public bool IsStealMana(Contender contender)
        {
            return (contender.isPlayer && playerStealMana) || (!contender.isPlayer && opponentStealMana);
        }

        #endregion

        #endregion
    }
}