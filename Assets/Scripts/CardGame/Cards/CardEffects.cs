using Booble.CardGame.Cards.DataModel.Effects;
using Booble.CardGame.Level;
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
        public bool hasDestroyEffects => _destroyEffects.Count > 0;

        private Card _card;

        private List<CardEffect> _enterEffects = new List<CardEffect>();
        private List<CardEffect> _combatEffects = new List<CardEffect>();
        private List<CardEffect> _permanentEffects = new List<CardEffect>();
        private List<CardEffect> _destroyEffects = new List<CardEffect>();

        public bool applyingEffects { private set; get; }
        public void SetApplyingEffects() => applyingEffects = true;

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

        #region Combat Effects

        public struct StoredValues
        {
            public int lifeValue;
            public int reboundValue;
            public int trampleValue;
            public int spongeValue;
        }

        private StoredValues storedValues;

        private bool _zeroStoredValues => storedValues.lifeValue == 0 && storedValues.reboundValue == 0
            && storedValues.trampleValue == 0 && storedValues.spongeValue == 0;

        public void GetEffectValues(Card target)
        {
            storedValues = new StoredValues();
            Card source = _card;

            CardEffectsManager.Instance.GetEffectValues(source, target, ref storedValues.lifeValue, ref storedValues.reboundValue,
                ref storedValues.trampleValue, ref storedValues.spongeValue);
        }

        public void ApplyEffectValues(Card target, bool receiveSingleHit = false)
        {
            if (storedValues.lifeValue > 0) _card.contender.RestoreLife(storedValues.lifeValue);

            if (storedValues.reboundValue > 0) target.ReceiveDamage(storedValues.reboundValue);

            if (storedValues.trampleValue > 0) target.contender.ReceiveDamage(storedValues.trampleValue);

            if (storedValues.spongeValue > 0) _card.BoostStats(storedValues.spongeValue, 0);
        }

        public bool hasManagedCombatEffects { private set; get; }
        public bool hasAppliableManagedCombatEffects { private set; get; }
        public bool singleHit { private set; get; }

        private void SetSingleHit(bool value, object target)
        {
            if (value)
            {
                storedValues.reboundValue = 0;
                storedValues.spongeValue = 0;

                ((Card)target).Effects.SetReceiveSingleHit();
            }

            singleHit = value;
        }

        public void SetReceiveSingleHit()
        {
            storedValues.lifeValue = 0;
            storedValues.trampleValue = 0;
        }

        #endregion

        #region Apply

        public void ApplyFirstEffect(object target)
        {
            ApplyEffect(firstEffect, target);
        }

        public void ApplyEffect(CardEffect effect, object target)
        {
            if (effect.IsAppliable(_card, target))
                effect.Apply(_card, target);
            else
                effect.SetEffectApplied();
        }

        public void ApplyEnterEffects()
        {
            ApplyEffects(_enterEffects, null);
        }

        public void ApplyCombatEffects(object target, bool singleHit = false)
        {
            SetSingleHit(singleHit, target);

            hasAppliableManagedCombatEffects = false;
            ApplyEffects(_combatEffects, target);
        }

        public void ApplyDestroyEffects()
        {
            ApplyEffects(_destroyEffects, null);
        }

        private void ApplyEffects(List<CardEffect> effects, object target)
        {
            SetApplyingEffects();
            StartCoroutine(ApplyEffectsCoroutine(effects, target));
        }

        private IEnumerator ApplyEffectsCoroutine(List<CardEffect> effects, object target)
        {
            foreach (CardEffect effect in effects)
            {
                if (effect.IsAppliable(_card, target))
                {
                    if (effect.IsManagedCombatEffect())
                    {
                        if (target is Card && _zeroStoredValues) continue;
                        hasAppliableManagedCombatEffects = true;
                    }

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
            _enterEffects.Clear();
            _combatEffects.Clear();
            _permanentEffects.Clear();
            _destroyEffects.Clear();

            hasManagedCombatEffects = false;

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
                    bool isManagedCombatEffect = effect.IsManagedCombatEffect();
                    if (!isManagedCombatEffect || !hasManagedCombatEffects)
                    {
                        _combatEffects.Add(effect);
                        if (isManagedCombatEffect) hasManagedCombatEffects = true;
                    }
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

        public void CheckRemoveEffects(Contender contender)
        {
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
                            if (effect.subType == SubType.GUARD) contender.RemoveGuardCard(_card);
                            else if (effect.subType == SubType.MIRROR) contender.RemoveMirrorCard();
                            break;
                    }
                }
            }
        }

        #endregion
    }
}