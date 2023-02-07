using Booble.CardGame.Cards;
using Booble.CardGame.Cards.DataModel.Effects;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Booble.CardGame.Managers
{
    public class ClashManager : MonoBehaviour
    {
        private class ClashInfo
        {
            public Card card;
            public object target;
            public Card targetCard;
            public bool targetIsGuardCard;

            public bool hasCardWithStrength => card != null && card.Stats.strength > 0;

            public Sequence GetHitSequence(bool combatActionsApplied)
            {
                return card.HitSequence(target, combatActionsApplied);
            }

            public void GetTarget(Contender contender, Card oppositeCard)
            {
                target = null;
                targetCard = null;
                targetIsGuardCard = false;

                if (card == null) return;

                if (contender.hasGuardCards)
                {
                    targetCard = contender.GetGuardCard();
                    targetIsGuardCard = targetCard != oppositeCard;
                    target = targetCard;
                }
                else if (oppositeCard != null)
                {
                    targetCard = oppositeCard;
                    target = targetCard;
                }
                else
                {
                    targetCard = null;
                    target = contender;
                }
            }

            public void Hit()
            {
                if (card != null)
                {
                    card.Hit(target);
                }
            }
        }

        public bool clash { private set; get; }
        public bool combat { private set; get; }

        private bool _combatActionsApplied;
        public void SetCombatActionsApplied()
        {
            _combatActionsApplied = true;
            _combatActions = true;
        }

        private bool _hitSequence, _combatActions;

        private ClashInfo _playerClashInfo, _opponentClashInfo;

        private Card _playerCard, _opponentCard;
        private bool _playerCardDestroy, _opponentCardDestroy;

        List<Card> cardsToClash = new List<Card>();
        HitType hitType;

        private enum HitType
        {
            NONE, PLAYER, OPPONENT, BOTH
        }

        public void Clash(bool changeTurn = true)
        {
            StartCoroutine(ClashCoroutine(changeTurn));
        }

        private IEnumerator ClashCoroutine(bool changeTurn)
        {
            Contender player = CardGameManager.Instance.player;
            Contender opponent = CardGameManager.Instance.opponent;

            _playerClashInfo = new ClashInfo();
            _opponentClashInfo = new ClashInfo();

            clash = true;
            _hitSequence = false;

            int index = 0;
            while (index < 4)
            {
                _playerClashInfo.card = player.cardZones[index].GetCard();
                _opponentClashInfo.card = opponent.cardZones[index].GetCard();

                if (!_playerClashInfo.hasCardWithStrength && !_opponentClashInfo.hasCardWithStrength)
                {
                    index++;
                    continue;
                }

                combat = true;
                _hitSequence = true;

                GetTargets();

                Sequence sequence = GetHitSequence();
                sequence.Play();

                yield return new WaitWhile(() => _hitSequence);
                if (_combatActionsApplied) yield return new WaitWhile(() => _combatActions);

                yield return new WaitUntil(() => TurnManager.Instance.continueFlow);

                _playerCardDestroy = _playerCard && _playerCard.CheckDestroy();
                _opponentCardDestroy = _opponentCard && _opponentCard.CheckDestroy();

                yield return new WaitWhile(DestroyingCards);

                yield return new WaitUntil(() => TurnManager.Instance.continueFlow);

                combat = false;

                if (cardsToClash.Count == 0) index++;
            }

            clash = false;
            if (changeTurn) TurnManager.Instance.ChangeTurn();
        }

        private void GetTargets()
        {
            Contender player = CardGameManager.Instance.player;
            Contender opponent = CardGameManager.Instance.opponent;

            _playerClashInfo.GetTarget(opponent, _opponentClashInfo.card);
            _opponentClashInfo.GetTarget(player, _playerClashInfo.card);
        }

        private Sequence GetHitSequence()
        {
            Sequence sequence = DOTween.Sequence();

            _combatActionsApplied = false;

            if (cardsToClash.Count == 0)
            {
                hitType = HitType.NONE;

                if (_playerClashInfo.targetIsGuardCard)
                    AddCombatCard(_playerClashInfo.card);

                if (_opponentClashInfo.targetIsGuardCard)
                    AddCombatCard(_opponentClashInfo.card);

                if (_playerClashInfo.card && !cardsToClash.Contains(_playerClashInfo.card))
                    AddCombatCard(_playerClashInfo.card);

                if (_opponentClashInfo.card && !cardsToClash.Contains(_opponentClashInfo.card))
                    AddCombatCard(_opponentClashInfo.card);
            }

            Card firstCard = cardsToClash[0];

            ClashInfo clashInfo = (firstCard.IsPlayerCard) ? _playerClashInfo : _opponentClashInfo;
            ClashInfo oppositeClashInfo = (firstCard.IsPlayerCard) ? _opponentClashInfo : _playerClashInfo;

            sequence.Join(clashInfo.GetHitSequence(_combatActionsApplied));

            if (clashInfo.targetIsGuardCard)
            {
                SetHitType(firstCard.IsPlayerCard);
                SetCombatCards(clashInfo.card, clashInfo.targetCard);
            }
            else
            {
                if (cardsToClash.Count > 1)
                {
                    Card secondCard = cardsToClash[1];
                    sequence.Join(oppositeClashInfo.GetHitSequence(_combatActionsApplied));
                    SetCombatCards(firstCard, secondCard);
                    SetHitType(HitType.BOTH);
                    cardsToClash.RemoveAt(1);
                }
                else
                {
                    SetCombatCards(firstCard, oppositeClashInfo.card);
                    SetHitType(firstCard.IsPlayerCard);
                }
            }

            cardsToClash.RemoveAt(0);

            sequence.AppendCallback(() => _hitSequence = false);

            return sequence;
        }

        private void AddCombatCard(Card card)
        {
            if (card.Stats.strength > 0)
            {
                cardsToClash.Add(card);
            }
        }

        private void SetHitType(bool playerHit)
        {
            if (playerHit) SetHitType(HitType.PLAYER);
            else SetHitType(HitType.OPPONENT);
        }

        private void SetHitType(HitType hitType) { this.hitType = hitType; }

        private void SetCombatCards(Card card1, Card card2)
        {
            if (card1 && card1.IsPlayerCard)
            {
                _playerCard = card1;
                _opponentCard = card2;
            }
            else
            {
                _playerCard = card2;
                _opponentCard = card1;
            }
        }

        private bool DestroyingCards()
        {
            return (_playerCardDestroy && !_playerCard.destroyed) || (_opponentCardDestroy && !_opponentCard.destroyed);
        }

        public void ApplyCombatActions()
        {
            StartCoroutine(ApplyCombatActionsCoroutine());
        }

        private IEnumerator ApplyCombatActionsCoroutine()
        {
            GetEffectValues();

            Hit();

            yield return new WaitWhile(IsPlayingAnimation);

            _applyingCombatEffects = true;
            StartCoroutine(ApplyCombatEffects());

            yield return new WaitWhile(() => _applyingCombatEffects);

            _combatActions = false;
        }

        private void GetEffectValues()
        {
            if (_playerCard && _opponentCard)
            {
                _playerCard.Effects.GetEffectValues(_opponentCard);
                _opponentCard.Effects.GetEffectValues(_playerCard);

                if (hitType == HitType.PLAYER) _playerCard.Effects.SetSingleHit(_opponentCard);
                else if (hitType == HitType.OPPONENT) _opponentCard.Effects.SetSingleHit(_playerCard);
            }
        }

        private void Hit()
        {
            switch (hitType)
            {
                case HitType.PLAYER:
                    {
                        object target = _opponentCard ? _opponentCard : _playerClashInfo.target;
                        _playerCard.Hit(target);
                    }
                    break;
                case HitType.OPPONENT:
                    {
                        object target = _playerCard ? _playerCard : _opponentClashInfo.target;
                        _opponentCard.Hit(target);
                    }
                    break;
                case HitType.BOTH:
                    {
                        _playerClashInfo.Hit();
                        _opponentClashInfo.Hit();
                    }
                    break;
            }
        }

        private bool _applyingCombatEffects;

        private IEnumerator ApplyCombatEffects()
        {
            if (_playerCard && _playerCard.Effects.hasCombatEffects)
                ApplyCombatEffects(_playerCard);

            if (_opponentCard && _opponentCard.Effects.hasCombatEffects)
                ApplyCombatEffects(_opponentCard);

            yield return new WaitWhile(ApplyingEffects);

            if (_playerCard && _playerCard.Effects.HasEffect(SubType.STEAL_CARD))
            {
                _playerCard.Effects.ApplyStealEffect(_opponentCard);
            }
            else if (_opponentCard && _opponentCard.Effects.HasEffect(SubType.STEAL_CARD))
            {
                _opponentCard.Effects.ApplyStealEffect(_playerCard);
            }

            _applyingCombatEffects = false;
        }

        private void ApplyCombatEffects(Card card)
        {
            bool isPlayer = card.IsPlayerCard;

            Card oppositeCard = isPlayer ? _opponentCard : _playerCard;
            ClashInfo clashInfo = isPlayer ? _playerClashInfo : _opponentClashInfo;

            object target = oppositeCard ? oppositeCard : clashInfo.target;
            card.Effects.ApplyCombatEffects(target);
        }

        private bool IsPlayingAnimation()
        {
            return (_playerCard && _playerCard.CardUI.IsPlayingAnimation) || (_opponentCard && _opponentCard.CardUI.IsPlayingAnimation);
        }

        private bool ApplyingEffects()
        {
            return (_playerCard && _playerCard.Effects.applyingEffects) || (_opponentCard && _opponentCard.Effects.applyingEffects);
        }
    }
}