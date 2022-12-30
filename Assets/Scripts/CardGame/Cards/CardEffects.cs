using Booble.CardGame.Cards.DataModel.Effects;
using Booble.CardGame.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Booble.CardGame.Cards
{
    public class CardEffects : MonoBehaviour
    {
        public List<CardEffect> effectsList { private set { _card.data.effects = value; } get { return _card.data.effects; } }
        public CardEffect firstEffect => effectsList[0];
        public bool hasEnterEffects => _enterEffects.Count > 0;
        public bool hasCombatEffects => _combatEffects.Count > 0;

        private Card _card;

        private List<CardEffect> _enterEffects = new List<CardEffect>();
        private List<CardEffect> _combatEffects = new List<CardEffect>();
        private List<CardEffect> _permanentEffects = new List<CardEffect>();
        private List<CardEffect> _destroyEffects = new List<CardEffect>();

        [HideInInspector] public bool applyingEffects;

        public void Initialize(Card card)
        {
            _card = card;
        }

        public bool AddEffect(CardEffect effect)
        {
            if (!HasEffect(effect.subType))
            {
                effectsList.Add(effect);
                CheckEffect(effect);
                return true;
            }

            return false;
        }

        public bool HasEffect(SubType subType)
        {
            foreach (CardEffect effect in effectsList)
            {
                if (effect.subType == subType) return true;
            }

            return false;
        }

        private bool IsManagedCombatEffect(SubType type)
        {
            return type == SubType.LIFELINK || type == SubType.REBOUND || type == SubType.TRAMPLE || type == SubType.SPONGE;
        }

        private bool HasManagedCombatEffect()
        {
            foreach(CardEffect effect in _combatEffects)
            {
                if (IsManagedCombatEffect(effect.subType)) return true;
            }

            return false;
        }

        #region Apply

        public void ApplyFirstEffect(object target)
        {
            ApplyEffect(firstEffect, target);
        }

        public void ApplyEffect(CardEffect effect, object target)
        {
            if (effect.IsAppliable(_card))
                effect.Apply(_card, target);
            else
                effect.SetEffectApplied();
        }

        public void ApplyEnterEffects()
        {
            ApplyEffects(_enterEffects, null);
        }

        public void ApplyCombatEffects(object target)
        {
            ApplyEffects(_combatEffects, target);
        }

        public void ApplyDestroyEffects()
        {
            ApplyEffects(_destroyEffects, null);
        }

        private void ApplyEffects(List<CardEffect> effects, object target)
        {
            if (effects.Count > 0)
            {
                applyingEffects = true;
                StartCoroutine(ApplyEffectsCoroutine(effects, target));
            }
        }
        private IEnumerator ApplyEffectsCoroutine(List<CardEffect> effects, object target)
        {

            foreach (CardEffect effect in effects)
            {
                if (effect.IsAppliable(_card))
                {
                    _card.CardUI.ShowEffectAnimation();
                    yield return new WaitWhile(() => _card.CardUI.IsPlayingAnimation);

                    ApplyEffect(effect, target);
                    yield return new WaitUntil(() => effect.effectApplied);
                }
            }

            applyingEffects = false;
        }

        #endregion

        #region Permanent Effects

        private void ApplyPermanentEffect()
        {
            StartCoroutine(ApplyPermanentEffectCoroutine());
        }

        private IEnumerator ApplyPermanentEffectCoroutine()
        {
            if (firstEffect.IsAppliable(_card))
            {
                _card.CardUI.ShowEffectAnimation();

                yield return new WaitWhile(() => _card.CardUI.IsPlayingAnimation);
            }

            ApplyFirstEffect(null);
        }

        #endregion

        #region Check

        public void CheckEffects()
        {
            foreach (CardEffect effect in effectsList)
            {
                CheckEffect(effect);
            }
        }

        private void CheckEffect(CardEffect effect)
        {
            switch (effect.applyTime)
            {
                case ApplyTime.ENTER:
                    _enterEffects.Add(effect); break;

                case ApplyTime.COMBAT:
                    if (!IsManagedCombatEffect(effect.subType) || !HasManagedCombatEffect()) _combatEffects.Add(effect);
                    break;

                case ApplyTime.DESTROY:
                    _destroyEffects.Add(effect); break;

                case ApplyTime.PERMANENT:
                    ApplyEffect(effect, null);
                    _permanentEffects.Add(effect);
                    break;

                case ApplyTime.END:
                case ApplyTime.DRAW_CARD:
                case ApplyTime.PLAY_ARGUMENT:
                    Action action = new Action(ApplyPermanentEffect);
                    CardEffectsManager.Instance.AddPermanentEffect(action, _card, effect.applyTime);
                    _permanentEffects.Add(effect);
                    break;
            }
        }

        public void CheckRemoveEffects()
        {
            Contender contender = _card.contender;
            if (_permanentEffects.Count > 0 && !_card.IsInHand)
            {
                foreach (CardEffect effect in _permanentEffects)
                {
                    switch (effect.applyTime)
                    {
                        case ApplyTime.END:
                        case ApplyTime.DRAW_CARD:
                        case ApplyTime.PLAY_ARGUMENT:
                            CardEffectsManager.Instance.RemovePermanentEffect(_card, effect.applyTime); break;

                        case ApplyTime.PERMANENT:
                            if (effect.subType == SubType.GUARD) TurnManager.Instance.RemoveGuardCard(contender);
                            else if (effect.subType == SubType.MIRROR) TurnManager.Instance.SetMirror(contender, false);
                            break;
                    }
                }
            }
        }

        #endregion
    }
}