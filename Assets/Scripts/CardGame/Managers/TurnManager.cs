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
            INTERVIEW_START, ROUND_START, PLAYER, OPPONENT, DISCARDING, CLASH, ROUND_END, INTERVIEW_END
        }

        public bool isPlayerTurn => _turn == Turn.PLAYER;

        private Turn _turn = Turn.INTERVIEW_START;
        public Turn turn { get { return _turn; } private set { _turn = value; } }

        #region Turn flow 

        public void StartGame()
        {
            StartCoroutine(StartGameCoroutine());
        }

        private IEnumerator StartGameCoroutine()
        {
            UIManager.Instance.TurnAnimation(turn);

            yield return new WaitUntil(() => continueFlow);

            DrawCards(CardGameManager.Instance.settings.initialCardNumber);

            yield return new WaitUntil(() => continueFlow);

            ChangeTurn();
        }

        private void StartRound()
        {
            StartCoroutine(StartRoundCoroutine());
        }

        private IEnumerator StartRoundCoroutine()
        {
            UIManager.Instance.TurnAnimation(turn);

            yield return new WaitUntil(() => continueFlow);

            StopFlow();
            CardGameManager.Instance.FillMana();
            UIManager.Instance.UpdateUIStats(startRound: true);

            yield return new WaitUntil(() => continueFlow);

            ChangeTurn();
        }

        private void StartTurn()
        {
            StartCoroutine(StartTurnCoroutine());
        }

        private IEnumerator StartTurnCoroutine()
        {
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

        private void StartClash()
        {
            if (skipCombat)
            {
                skipCombat = false;
                ChangeTurn();
            }
            else
                StartCoroutine(StartClashCoroutine());
        }

        private IEnumerator StartClashCoroutine()
        {
            UIManager.Instance.TurnAnimation(turn);

            yield return new WaitUntil(() => continueFlow);

            Clash();
        }

        private void FinishRound()
        {
            if (!CardGameManager.Instance.CheckEnd())
            {
                StartCoroutine(FinishRoundCoroutine());
            }
        }

        private IEnumerator FinishRoundCoroutine()
        {
            UIManager.Instance.TurnAnimation(turn);

            yield return new WaitUntil(() => continueFlow);

            StartCoroutine(ApplyEndTurnEffectsCoroutine());
        }

        public void ChangeTurn()
        {
            switch (_turn)
            {
                case Turn.INTERVIEW_START:
                    SetTurn(Turn.ROUND_START);
                    StartRound();
                    break;

                case Turn.ROUND_START:
                    SetTurn(Turn.OPPONENT);
                    StartTurn();
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
                    StartClash();
                    break;

                case Turn.CLASH:
                    SetTurn(Turn.ROUND_END);
                    FinishRound();
                    break;

                case Turn.ROUND_END:
                    SetTurn(Turn.ROUND_START);
                    StartRound();
                    break;

                case Turn.INTERVIEW_END: break;

            }
        }

        private void SetTurn(Turn turn)
        {
            _turn = turn;
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
            Board.Instance.DrawCards(cardNumber, _turn);
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
            Contender player = CardGameManager.Instance.player;
            Contender opponent = CardGameManager.Instance.opponent;

            for (int index = 0; index < 4; index++)
            {
                combat = true;

                Card playerCard = player.cardZones[index].GetCard();
                Card opponentCard = opponent.cardZones[index].GetCard();

                if (playerCard == null && opponentCard == null) continue;

                object playerTarget = GetTarget(opponent, _opponentGuardCard, opponentCard);
                object opponentTarget = GetTarget(player, _playerGuardCard, playerCard);

                Sequence sequence = DOTween.Sequence();

                if (playerCard) sequence.Join(playerCard.HitSequence(playerTarget));
                if (opponentCard) sequence.Join(opponentCard.HitSequence(opponentTarget));

                sequence.AppendCallback(() => combat = false);
                sequence.Play();

                yield return new WaitWhile(() => combat);
                yield return new WaitUntil(() => continueFlow);

                yield return new WaitWhile(() => (playerCard && playerCard.CardUI.IsPlayingAnimation)
                    || (opponentCard && opponentCard.CardUI.IsPlayingAnimation));

                bool playerCardDestroy = playerCard && playerCard.CheckDestroy();
                bool opponentCardDestroy = opponentCard && opponentCard.CheckDestroy();

                yield return new WaitWhile(() => (playerCardDestroy && playerCard != null) || (opponentCardDestroy && opponentCard != null));
            }

            ChangeTurn();
        }

        public void ApplyCombatActions(Card source, object targetObj)
        {
            source.Effects.ApplyCombatEffects(targetObj);
            if (IsHitManagedByEffect(source, targetObj))
                return;

            source.Hit(targetObj);
        }

        private object GetTarget(Contender contender, Card guardCard, Card card)
        {
            return (guardCard != null) ? guardCard
                : (card != null) ? card
                : contender;
        }

        private bool IsHitManagedByEffect(Card source, object targetObj)
        {
            return targetObj is Card && (IsEffect(SubType.REBOUND, source, (Card)targetObj) || IsEffect(SubType.SPONGE, source, (Card)targetObj));
        }

        private bool IsEffect(SubType subType, Card source, Card target)
        {
            return source.Effects.HasEffect(subType) || target.Effects.HasEffect(subType);
        }

        #endregion

        #region Interview End

        public void CheckEndMidTurn()
        {
            if (CardGameManager.Instance.CheckEnd())
            {
                StopFlow();
            }
        }

        public void AlternateWinCondition()
        {
            CardGameManager.Instance.SetAlternateWinCondition();
            CheckEndMidTurn();
        }

        #endregion

        #region Effects

        private bool skipCombat;

        public void SkipCombat()
        {
            skipCombat = true;
        }

        public bool GetSkipCombat()
        {
            return skipCombat;
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
            foreach (Action action in playArgumentEffectsActions)
            {
                action();
            }
        }

        #endregion

        #region End Turn Effects

        private List<string> _endTurnEffectsNames = new List<string>();
        private List<Action> _endTurnEffectsActions = new List<Action>();
        private List<int> _effectsToRemove = new List<int>();

        public void AddEndTurnEffect(Action method, string name)
        {
            Debug.Log("End turn effect added");
            _endTurnEffectsActions.Add(method);
            _endTurnEffectsNames.Add(name);
        }

        public void RemoveEndTurnEffect(Action method)
        {
            if (turn == Turn.ROUND_END)
            {
                _effectsToRemove.Add(_endTurnEffectsActions.IndexOf(method));
            }
            else
                _endTurnEffectsActions.Remove(method);
        }

        private IEnumerator ApplyEndTurnEffectsCoroutine()
        {
            for (int i = 0; i < _endTurnEffectsActions.Count; i++)
            {
                if (_effectsToRemove.Contains(i))
                {
                    Debug.Log(_endTurnEffectsNames[i] + " to be removed");
                    continue;
                }

                StopFlow();
                Action endTurnEffect = _endTurnEffectsActions[i];
                endTurnEffect();
                Debug.Log(_endTurnEffectsNames[i] + " applied");
                yield return new WaitUntil(() => continueFlow);
            }

            for (int i = _effectsToRemove.Count - 1; i >= 0; i--)
            {
                _endTurnEffectsActions.RemoveAt(i);
                _endTurnEffectsNames.RemoveAt(i);
            }

            _effectsToRemove.Clear();

            ChangeTurn();
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
            if (contender.isPlayer)
                _playerGuardCard = null;
            else
                _opponentGuardCard = null;
        }

        #endregion

        #region Mirror

        private bool _playerMirror;
        private bool _opponentMirror;

        public void SetMirror(Contender contender, bool state)
        {
            if (contender.isPlayer) _playerMirror = state;
            else _opponentMirror = state;
        }
        public bool IsMirror(Contender contender)
        {
            return (contender.isPlayer && _playerMirror) || (!contender.isPlayer && _opponentMirror);
        }

        #endregion

        #endregion
    }
}