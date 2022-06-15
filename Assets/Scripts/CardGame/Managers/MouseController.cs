using CardGame.Cards;
using CardGame.Cards.DataModel;
using CardGame.Cards.DataModel.Effects;
using System;
using UnityEngine;

namespace CardGame.Managers
{
    public class MouseController : MonoBehaviour
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
        public Card holding = null;

        //private Card _applyingEffect;

        private IClickable _hovering;
        //private IClickable _closeUp;

        private void Start()
        {
            _currentMask = selectingLayerMask;
            //_applyingEffect = null;
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
                if (/*!IsApplyingEffect() && */clickableObject != null && TurnManager.Instance.isPlayerTurn)
                {
                    clickableObject.OnMouseLeftClickDown(this);
                }
            }
            else if (leftClickUp)
            {
                /*if (IsApplyingEffect())
                {
                    ApplyEffect(clickableObject);
                }
                else
                {*/
                    if (clickableObject != null)
                    {
                        clickableObject.OnMouseLeftClickUp(this);
                    }
                    else if (holding != null)
                    {
                        holding.OnMouseLeftClickUp(this);
                    }
                //}
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

        /*private void ApplyEffect(IClickable clickableObject)
        {
            //_applyingEffect.effect.Apply((Card)clickableObject);
            if(_applyingEffect.type == CardType.ACTION) _applyingEffect.Destroy();
            _applyingEffect = null;
            CheckMask(null);
            TurnManager.Instance.SetEndButtonInteractable(true);
        }*/

        /*private bool IsApplyingEffect()
        {
            return _applyingEffect != null;
        }*/

        public void SetHolding(Card card)
        {
            CheckMask(card);
            holding = card;
        }

        public void CheckMask(Card card)
        {
            if (card == null)
            {
               /* if (_currentMask == holdingLayerMask)
                {
                    // From holding to applying effect to ally, enemy or no target
                    if (holding.hasEffect)
                    {
                        _applyingEffect = holding;
                        SetMask(GetTargetMask(holding.effect));
                    }
                    // From holding to selecting
                    else
                    {
                        SetMask(selectingLayerMask);
                    }
                }
                else
                {*/
                    SetMask(selectingLayerMask);
                //}
            }
            else
            {
                // From selecting to holding card
                if (_currentMask == selectingLayerMask)
                {
                    SetMask(holdingLayerMask);
                }
            }
        }

        /*private LayerMask GetTargetMask(CardEffect effect)
        {
            switch (effect.GetTarget())
            {
                case "": return selectingLayerMask;
                default: return selectingLayerMask;
            }

            return noTargetLayerMask;
        }*/

        private void SetMask(LayerMask layerMask)
        {
            _currentMask = layerMask;
        }
    }
}