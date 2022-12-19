using CardGame.Cards;
using CardGame.Cards.DataModel;
using CardGame.Cards.DataModel.Effects;
using CardGame.Level;
using Santi.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame.Managers
{
    public class MouseController : Singleton<MouseController>
    {

        #region Layer Masks

        [Header("Layer Masks")]
        public LayerMask selectingLayerMask;

        public LayerMask noTargetLayerMask;
        public LayerMask allyLayerMask;
        public LayerMask enemyLayerMask;

        public LayerMask stealingLayerMask;

        private LayerMask _currentMask;

        #endregion

        [HideInInspector]
        public Card holdingCard = null;

        private Card _effectCard;
        public bool IsHoldingCard => holdingCard != null;
        public bool IsApplyingEffect => _effectCard != null;

        private IClickable _hovering;

        private void Start()
        {
            SetMask(selectingLayerMask);
        }

        void Update()
        {
            IClickable clickableObject = GetRaycastObject();

            CheckHovering(clickableObject);
            CheckClick(clickableObject);
        }

        private IClickable GetRaycastObject()
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            var rayResult = Physics.Raycast(ray, out hit, Mathf.Infinity, _currentMask);

            IClickable clickableObject = null;
            if (rayResult)
            {
                GameObject obj = hit.collider.gameObject;
                clickableObject = obj.GetComponent<IClickable>();
            }

            return clickableObject;
        }

        #region Hovering 

        private void CheckHovering(IClickable clickableObject)
        {
            if (clickableObject != _hovering)
            {
                HoverExit();
                if (clickableObject != null)
                {
                    clickableObject.OnMouseHoverEnter();
                    _hovering = clickableObject;
                }
            }
        }

        private void HoverExit()
        {
            if (_hovering != null)
            {
                _hovering.OnMouseHoverExit();
                _hovering = null;
            }
        }

        #endregion

        #region Click

        private void CheckClick(IClickable clickableObject)
        {
            bool leftClickUp = Input.GetMouseButtonUp(0);

            if (leftClickUp)
            {
                if (IsApplyingEffect)
                {
                    ApplyEffectToTarget(clickableObject);
                }
                else
                {
                    if (clickableObject != null)
                    {
                        clickableObject.OnMouseLeftClickUp(this);
                    }
                    else if (holdingCard != null)
                    {
                        holdingCard.OnMouseLeftClickUp(this);
                    }
                }
            }
        }

        #endregion

        #region Effects 

        public void SetApplyingEffect(Card card)
        {
            _effectCard = card;
            SetMask(GetTargetMask(card.effect));
        }

        private void ApplyEffectToTarget(IClickable clickableObject)
        {
            Card targetCard = (Card)clickableObject;
            if (targetCard == null
                || targetCard.IsInHand
                || !targetCard.isHighlighted)
                return;

            StartCoroutine(ApplyEffectCoroutine(targetCard));
        }

        private IEnumerator ApplyEffectCoroutine(Card targetCard)
        {
            _effectCard.Stats.SubstractMana();
            _effectCard.Effects.ApplyEffect(targetCard);

            UIManager.Instance.HidePlayButtons();
            Board.Instance.HighlightTargets(new List<Card>());

            yield return new WaitUntil(() => TurnManager.Instance.continueFlow);

            _effectCard.DestroyCard();

            yield return new WaitUntil(() => _effectCard == null);

            ResetApplyingEffect();
        }

        public void ResetApplyingEffect()
        {
            _effectCard = null;
            SetMask(selectingLayerMask);
            UIManager.Instance.SetEndTurnButtonInteractable(true);
        }

        #endregion

        #region Layers 

        public void SetHolding(Card card)
        {
            holdingCard = card;
        }

        public void SetStealing()
        {
            SetMask(stealingLayerMask);
        }

        public void SetSelecting()
        {
            SetMask(selectingLayerMask);
        }

        private LayerMask GetTargetMask(CardEffect effect)
        {
            LayerMask layerMask;
            switch (effect.targetType)
            {
                case Target.NONE:
                    layerMask = selectingLayerMask;
                    break;

                case Target.ALLY:
                    layerMask = allyLayerMask;
                    break;

                case Target.ENEMY:
                    layerMask = enemyLayerMask;
                    break;

                case Target.CARD:
                    layerMask = allyLayerMask | enemyLayerMask;
                    break;

                default:
                    layerMask = selectingLayerMask;
                    break;
            }
            return layerMask;
        }

        private void SetMask(LayerMask layerMask)
        {
            _currentMask = layerMask;
        }

        #endregion


    }
}