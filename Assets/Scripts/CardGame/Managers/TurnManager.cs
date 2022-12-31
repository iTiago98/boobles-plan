using Booble.CardGame.Cards;
using Booble.CardGame.Cards.DataModel.Effects;
using Booble.CardGame.Level;
using DG.Tweening;
using Santi.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Booble.CardGame.Managers
{
    public class TurnManager : Singleton<TurnManager>
    {

        public enum Turn
        {
            INTERVIEW_START, ROUND_START, PLAYER, OPPONENT, DISCARDING, CLASH, ROUND_END, INTERVIEW_END
        }

        public bool IsPlayerTurn => _turn == Turn.PLAYER;

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

            yield return new WaitUntil(() => GetContinueFlow());

            DrawCards(CardGameManager.Instance.settings.initialCardNumber);

            yield return new WaitUntil(() => GetContinueFlow());

            ChangeTurn();
        }

        private void StartRound()
        {
            StartCoroutine(StartRoundCoroutine());
        }

        private IEnumerator StartRoundCoroutine()
        {
            UIManager.Instance.TurnAnimation(turn);

            yield return new WaitUntil(() => GetContinueFlow());

            StopFlow();
            CardGameManager.Instance.FillMana();
            UIManager.Instance.UpdateUIStats(startRound: true);

            yield return new WaitUntil(() => UIManager.Instance.statsUpdated);

            ChangeTurn();
        }

        private void StartTurn()
        {
            StartCoroutine(StartTurnCoroutine());
        }

        private IEnumerator StartTurnCoroutine()
        {
            UIManager.Instance.TurnAnimation(turn);

            yield return new WaitUntil(() => GetContinueFlow());

            DrawCards(1);

            yield return new WaitUntil(() => GetContinueFlow());

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

            yield return new WaitUntil(() => GetContinueFlow());

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

            yield return new WaitUntil(() => GetContinueFlow());

            CardEffectsManager.Instance.ApplyEndTurnEffects();
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
                    break;

                case Turn.PLAYER:
                    SetTurn(Turn.DISCARDING);
                    if (!CardGameManager.Instance.player.hand.CheckStartDiscarding())
                    {
                        ChangeTurn();
                    }
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

        public bool GetContinueFlow()
        {
            return continueFlow && !CardGameManager.Instance.gamePaused;
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
        private bool _combatActions;

        private IEnumerator ClashCoroutine()
        {
            Contender player = CardGameManager.Instance.player;
            Contender opponent = CardGameManager.Instance.opponent;

            bool hitSequence = false;

            for (int index = 0; index < 4; index++)
            {
                Card playerCard = player.cardZones[index].GetCard();
                Card opponentCard = opponent.cardZones[index].GetCard();

                if ((playerCard == null || playerCard.Stats.strength == 0)
                    && (opponentCard == null || opponentCard.Stats.strength == 0)) continue;

                combat = true;

                object playerTarget = GetTarget(opponent, _opponentGuardCard, opponentCard);
                object opponentTarget = GetTarget(player, _playerGuardCard, playerCard);

                Sequence sequence = DOTween.Sequence();

                bool applyCombatActions = true;
                if (playerCard)
                {
                    sequence.Join(playerCard.HitSequence(playerTarget, applyCombatActions));
                    applyCombatActions = false;
                }
                if (opponentCard)
                    sequence.Join(opponentCard.HitSequence(opponentTarget, applyCombatActions));

                if (!applyCombatActions) _combatActions = true;

                sequence.AppendCallback(() => hitSequence = false);

                hitSequence = true;
                sequence.Play();

                yield return new WaitWhile(() => hitSequence);

                if (!applyCombatActions) yield return new WaitWhile(() => _combatActions);

                bool playerCardDestroy = playerCard && playerCard.CheckDestroy();
                bool opponentCardDestroy = opponentCard && opponentCard.CheckDestroy();

                yield return new WaitWhile(() => (playerCardDestroy && playerCard != null) || (opponentCardDestroy && opponentCard != null));

                combat = false;
            }

            ChangeTurn();
        }

        public void ApplyCombatActions(Card source, object targetObj)
        {
            StartCoroutine(ApplyCombatActionsCoroutine(source, targetObj));
        }

        private IEnumerator ApplyCombatActionsCoroutine(Card source, object targetObj)
        {
            Card targetCard = null;
            if (targetObj is Card) targetCard = (Card)targetObj;

            if (targetCard != null)
            {
                if (source.Effects.hasManagedCombatEffects) source.Effects.GetEffectValues();
                if (targetCard.Effects.hasManagedCombatEffects) targetCard.Effects.GetEffectValues();
            }

            source.Hit(targetObj);
            targetCard?.Hit(source);

            yield return new WaitWhile(() => source.CardUI.IsPlayingAnimation);
            if (targetCard != null) yield return new WaitWhile(() => targetCard.CardUI.IsPlayingAnimation);

            if (source.Effects.hasCombatEffects) source.Effects.ApplyCombatEffects(targetObj);
            if (targetCard != null && targetCard.Effects.hasCombatEffects) targetCard?.Effects.ApplyCombatEffects(source);

            yield return new WaitWhile(() => source.Effects.applyingEffects);
            if (targetCard != null) yield return new WaitWhile(() => targetCard.Effects.applyingEffects);

            _combatActions = false;
        }

        private object GetTarget(Contender contender, Card guardCard, Card card)
        {
            return (guardCard != null) ? guardCard
                : (card != null) ? card
                : contender;
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