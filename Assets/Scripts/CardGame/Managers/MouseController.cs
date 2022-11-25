using CardGame.Cards;
using CardGame.Cards.DataModel;
using CardGame.Cards.DataModel.Effects;
using CardGame.Level;
using Santi.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame.Managers
{
    public class MouseController : Singleton<MouseController>
    {

        #region Layer Masks

        [Header("Layer Masks")]
        public LayerMask selectingLayerMask;
        public LayerMask holdingLayerMask;

        public LayerMask noTargetLayerMask;
        public LayerMask allyLayerMask;
        public LayerMask enemyLayerMask;

        private LayerMask _currentMask;

        #endregion

        [HideInInspector]
        public Card holdingCard = null;

        private Card _effectCard;
        public bool IsApplyingEffect => _effectCard != null;

        private IClickable _hovering;
        //private IClickable _closeUp;

        private void Start()
        {
            _currentMask = selectingLayerMask;
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
            bool leftClickDown = Input.GetMouseButtonDown(0);
            bool leftClickUp = Input.GetMouseButtonUp(0);
            //bool rightClick = Input.GetMouseButtonDown(1) || Input.GetMouseButtonUp(1);

            //if ((leftClickDown || rightClick) && (_closeUp != null) && (_closeUp != clickableObject))
            //{
            //    if (!_closeUp.clickable) return;

            //    _closeUp.OnMouseRightClick();
            //    _closeUp = null;
            //}

            if (leftClickDown)
            {
                if (!IsApplyingEffect && clickableObject != null && TurnManager.Instance.isPlayerTurn)
                {
                    clickableObject.OnMouseLeftClickDown(this);
                }
            }
            else if (leftClickUp)
            {
                if (IsApplyingEffect)
                {
                    ApplyEffectToTarget(clickableObject);
                }
                else
                {
                    if (clickableObject != null && TurnManager.Instance.isPlayerTurn)
                    {
                        clickableObject.OnMouseLeftClickUp(this);
                    }
                    else if (holdingCard != null)
                    {
                        holdingCard.OnMouseLeftClickUp(this);
                    }
                }
            }
            //else if (rightClick)
            //{
            //    if (clickableObject != null)
            //    {
            //        clickableObject.OnMouseRightClick();
            //        _closeUp = (_closeUp == clickableObject) ? null : clickableObject;
            //    }
            //}
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
            if (targetCard.IsInHand) return;

            _effectCard.contender.SubstractMana(_effectCard.manaCost);
            _effectCard.effect.Apply(_effectCard, targetCard);
            if (_effectCard.type == CardType.ACTION) _effectCard.Destroy();
            ResetApplyingEffect();
        }

        public void ResetApplyingEffect()
        {
            _effectCard = null;
            SetMask(selectingLayerMask);
            UIManager.Instance.SetEndTurnButtonInteractable(true);
            UIManager.Instance.HidePlayButtons();
            Board.Instance.HighlightTargets(new List<Card>());
        }

        #endregion

        #region Layers 

        public void SetHolding(Card card)
        {
            CheckMask(card);
            holdingCard = card;
        }

        public void CheckMask(Card card)
        {
            if (card == null)
            {
                if (_currentMask == holdingLayerMask)
                {
                    // From holding card to selecting
                    //Debug.Log("selectingMask");
                    SetMask(selectingLayerMask);
                }
            }
            else if (_currentMask == selectingLayerMask)
            {
                // From selecting to holding card
                //Debug.Log("holdingMask");
                SetMask(holdingLayerMask);
            }
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