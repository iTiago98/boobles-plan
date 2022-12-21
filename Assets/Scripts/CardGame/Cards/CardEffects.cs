using CardGame.Cards.DataModel.Effects;
using CardGame.Level;
using CardGame.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame.Cards
{
    public class CardEffects : MonoBehaviour
    {
        public List<CardEffect> effectsList { private set { _card.data.effects = value; } get { return _card.data.effects; } }
        public CardEffect firstEffect => effectsList[0];
        public bool hasEffect => effectsList.Count > 0;

        private Action _playArgumentEffect;
        private List<Action> _endTurnEffects;
        private Deck.DrawCardEffects _drawCardEffect;

        private Card _card;

        private void Start()
        {
            _endTurnEffects = new List<Action>();
        }

        public void Initialize(Card card)
        {
            _card = card;
        }

        public void AddEffect(CardEffect effect)
        {
            if(!HasEffect(effect.subType))
            {
                effectsList.Add(effect);
                _card.CardUI.UpdateDescriptionText();
            }
        }

        public bool HasEffect(SubType subType)
        {
            foreach(CardEffect effect in effectsList)
            {
                if(effect.subType == subType) return true;
            }

            return false;
        }

        #region Apply

        public void ApplyEffect()
        {
            ApplyEffect(firstEffect);
        }

        public void ApplyEffect(object target)
        {
            ApplyEffect(firstEffect, target);
        }

        private void ApplyEffect(CardEffect effect)
        {
            ApplyEffect(effect, null);
        }

        public void ApplyEffect(CardEffect effect, object target)
        {
            if (effect.IsAppliable()) effect.Apply(_card, target);
        }

        #region Combat

        public void ApplyCombatEffects(object target)
        {
            foreach (CardEffect effect in effectsList)
            {
                if (effect.applyTime == ApplyTime.COMBAT)
                {
                    ApplyEffect(effect, target);
                }
            }
        }

        #endregion

        #region End Turn

        public void ApplyEndTurnEffect()
        {
            StartCoroutine(ApplyEndTurnEffectCoroutine());
        }

        private IEnumerator ApplyEndTurnEffectCoroutine()
        {
            //GameObject applyEffectParticles = UIManager.Instance.ShowParticlesEffectApply(transform);

            //yield return new WaitUntil(() => applyEffectParticles == null);

            ApplyEffect(firstEffect);

            yield return null;
        }

        #endregion

        #endregion

        public void CheckEffect()
        {
            if (hasEffect)
            {
                foreach (CardEffect effect in effectsList)
                {
                    if (effect.applyTime == ApplyTime.ENTER)
                    {
                        ApplyEffect(effect);
                    }
                    else if (effect.applyTime == ApplyTime.END)
                    {
                        Action action = new Action(ApplyEndTurnEffect);
                        _endTurnEffects.Add(action);
                        TurnManager.Instance.AddEndTurnEffect(action, _card);
                    }
                    else if (effect.applyTime == ApplyTime.DRAW_CARD)
                    {
                        _drawCardEffect = new Deck.DrawCardEffects(ApplyEffect);
                        _card.contender.deck.AddDrawCardEffects(_drawCardEffect);
                    }
                    else if (effect.applyTime == ApplyTime.PLAY_ARGUMENT)
                    {
                        _playArgumentEffect = new Action(ApplyEffect);
                        TurnManager.Instance.AddPlayArgumentEffects(_playArgumentEffect);
                    }
                }
            }
        }

        public void CheckDestroyEffects()
        {
            if (hasEffect)
            {
                foreach (CardEffect effect in effectsList)
                {
                    if (effect.applyTime == ApplyTime.DESTROY && !_card.IsInHand)
                    {
                        ApplyEffect(effect);
                    }
                }
            }
        }

        public void CheckDelegateEffects()
        {
            Contender contender = _card.contender;
            if (hasEffect)
            {
                foreach (CardEffect effect in effectsList)
                {
                    // End Turn Effects
                    if (effect.applyTime == ApplyTime.END && _endTurnEffects?.Count > 0)
                    {
                        TurnManager.Instance.RemoveEndTurnEffect(_endTurnEffects[_endTurnEffects.Count - 1]);
                    }
                    // Draw Card Effects
                    else if (effect.applyTime == ApplyTime.DRAW_CARD && _drawCardEffect != null)
                    {
                        contender.deck.RemoveDrawCardEffect(_drawCardEffect);
                    }
                    // Play Argument Effects
                    else if (effect.applyTime == ApplyTime.PLAY_ARGUMENT && _playArgumentEffect != null)
                    {
                        TurnManager.Instance.RemovePlayArgumentEffect(_playArgumentEffect);
                    }

                    // Guard Card
                    if (effect.subType == SubType.GUARD)
                    {
                        TurnManager.Instance.RemoveGuardCard(contender);
                    }
                    else if (effect.subType == SubType.MIRROR)
                    {
                        TurnManager.Instance.SetMirror(contender, false);
                    }
                }
            }
        }

        public void ManageEffects()
        {
            if (hasEffect)
            {
                foreach (CardEffect effect in effectsList)
                {
                    if (effect.subType == SubType.GUARD) TurnManager.Instance.SetGuardCard(_card);
                    else if (effect.subType == SubType.MIRROR) TurnManager.Instance.SetMirror(_card.contender, true);
                }
            }
        }

    }

}