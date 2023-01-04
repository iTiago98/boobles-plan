using Booble.CardGame.Cards;
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

            public bool cardDestroy;
            public bool targetDestroy;

            public bool hasCardWithStrength => card != null && card.Stats.strength > 0;
            public bool playingAnimation => card != null && card.CardUI.IsPlayingAnimation;
            public bool applyingEffects => card != null && card.Effects.applyingEffects || targetIsGuardCard && targetCard.Effects.applyingEffects;

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
                    //if (targetCard) targetCard.CardUI.SetPlayingAnimation();
                }
            }

            public void GetCardDestroy()
            {
                cardDestroy = card && card.CheckDestroy();
                targetDestroy = targetCard && targetCard.CheckDestroy();
            }

            public bool GetCardDestroying()
            {
                return cardDestroy && !card.destroyed || targetDestroy && !targetCard.destroyed;
            }
        }

        public bool combat { private set; get; }

        private bool _combatActionsApplied;
        public void SetCombatActionsApplied()
        {
            _combatActionsApplied = true;
            _combatActions = true;
        }

        private bool _hitSequence, _combatActions;

        private bool _firstHit, _playerFirstHit, _secondHit;

        private ClashInfo _playerClashInfo, _opponentClashInfo;

        private Card _playerCard, _opponentCard;

        public void Clash()
        {
            StartCoroutine(ClashCoroutine());
        }

        private IEnumerator ClashCoroutine()
        {
            Contender player = CardGameManager.Instance.player;
            Contender opponent = CardGameManager.Instance.opponent;

            _playerClashInfo = new ClashInfo();
            _opponentClashInfo = new ClashInfo();

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

                _playerClashInfo.GetCardDestroy();
                _opponentClashInfo.GetCardDestroy();

                yield return new WaitWhile(DestroyingCards);

                combat = false;

                if (!_firstHit) index++;
            }

            TurnManager.Instance.ChangeTurn();
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

            if (!_firstHit)
            {
                _secondHit = false;

                if (_playerClashInfo.targetIsGuardCard)
                {
                    sequence.Join(_playerClashInfo.GetHitSequence(_combatActionsApplied));

                    SetFirstHit(_opponentClashInfo, true);
                    SetCombatCards(_playerClashInfo.card, _playerClashInfo.targetCard);
                }
                else if (_opponentClashInfo.targetIsGuardCard)
                {
                    sequence.Join(_opponentClashInfo.GetHitSequence(_combatActionsApplied));

                    SetFirstHit(_playerClashInfo, false);
                    SetCombatCards(_opponentClashInfo.targetCard, _opponentClashInfo.card);
                }
                else
                {
                    if (_playerClashInfo.card)
                    {
                        sequence.Join(_playerClashInfo.GetHitSequence(_combatActionsApplied));
                    }
                    if (_opponentClashInfo.card)
                    {
                        sequence.Join(_opponentClashInfo.GetHitSequence(_combatActionsApplied));
                    }

                    SetCombatCards(_playerClashInfo.card, _opponentClashInfo.card);
                }
            }
            else
            {
                _firstHit = false;
                _secondHit = true;

                if (_playerFirstHit)
                {
                    if (_opponentClashInfo.card)
                    {
                        sequence.Join(_opponentClashInfo.GetHitSequence(_combatActionsApplied));

                        SetCombatCards(_opponentClashInfo.targetCard, _opponentClashInfo.card);
                    }
                }
                else if (_playerClashInfo.card)
                {
                    sequence.Join(_playerClashInfo.GetHitSequence(_combatActionsApplied));

                    SetCombatCards(_playerClashInfo.card, _playerClashInfo.targetCard);
                }
            }

            sequence.AppendCallback(() => _hitSequence = false);

            return sequence;
        }

        private void SetFirstHit(ClashInfo clashInfo, bool playerFirstHit)
        {
            if (clashInfo.card)
            {
                _firstHit = true;
                _playerFirstHit = playerFirstHit;
            }
        }

        private void SetCombatCards(Card player, Card opponent)
        {
            _playerCard = player;
            _opponentCard = opponent;
        }

        private bool DestroyingCards()
        {
            return _playerClashInfo.GetCardDestroying() || _opponentClashInfo.GetCardDestroying();
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

            ApplyCombatEffects();

            yield return new WaitWhile(ApplyingEffects);

            _combatActions = false;
        }

        private void GetEffectValues()
        {
            if (_playerCard && _opponentCard)
            {
                if (_playerCard.Effects.hasManagedCombatEffects) _playerCard.Effects.GetEffectValues(_opponentCard);
                if (_opponentCard.Effects.hasManagedCombatEffects) _opponentCard.Effects.GetEffectValues(_playerCard);
            }
        }

        private void Hit()
        {
            if ((_firstHit && _playerFirstHit) || (_secondHit && !_playerFirstHit))
            {
                object target = _opponentCard ? _opponentCard : _playerClashInfo.target;
                _playerCard.Hit(target);
                //_opponentCard.CardUI.SetPlayingAnimation();
            }
            else if ((_firstHit && !_playerFirstHit) || (_secondHit && _playerFirstHit))
            {
                object target = _playerCard ? _playerCard : _opponentClashInfo.target;
                _opponentCard.Hit(target);
                //_playerCard.CardUI.SetPlayingAnimation();
            }
            else
            {
                _playerClashInfo.Hit();
                _opponentClashInfo.Hit();
            }
        }

        private void ApplyCombatEffects()
        {
            bool playerCardHasCombatEffects = _playerCard && _playerCard.Effects.hasCombatEffects;
            bool opponentCardHasCombatEffects = _opponentCard && _opponentCard.Effects.hasCombatEffects;

            if (playerCardHasCombatEffects)
            {
                bool singleHit = (_opponentCard != _opponentClashInfo.card) || (_playerFirstHit && _firstHit) || (!_playerFirstHit && _secondHit);
                object target = _opponentCard ? _opponentCard : _playerClashInfo.target;
                _playerCard.Effects.ApplyCombatEffects(target, singleHit);
                //_playerCard.Effects.SetApplyingEffects();
            }
            if (opponentCardHasCombatEffects)
            {
                bool singleHit = (_playerCard != _playerClashInfo.card) || (_playerFirstHit && _secondHit) || (!_playerFirstHit && _firstHit);
                object target = _playerCard ? _playerCard : _opponentClashInfo.target;
                _opponentCard.Effects.ApplyCombatEffects(target, singleHit);
                //_opponentCard.Effects.SetApplyingEffects();
            }
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