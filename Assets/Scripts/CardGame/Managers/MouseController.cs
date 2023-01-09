using Booble.CardGame.Cards;
using Booble.CardGame.Cards.DataModel.Effects;
using Booble.CardGame.Level;
using Santi.Utils;
using System.Collections;
using UnityEngine;

namespace Booble.CardGame.Managers
{
    public class MouseController : Singleton<MouseController>
    {

        #region Layer Masks

        [Header("Layer Masks")]
        [SerializeField] private LayerMask selectingLayerMask;
        [SerializeField] private LayerMask targetLayerMask;
        [SerializeField] private LayerMask stealingLayerMask;

        private LayerMask _currentMask;

        #endregion

        public Card holdingCard { private set; get; }

        private Card _effectCard;
        public bool IsHoldingCard => holdingCard != null;
        public bool IsApplyingEffect => _effectCard != null;

        private IClickable _hovering;

        #region Tutorial

        private string _tutorialCardName;
        public void SetTutorialCard(string name)
        {
            _tutorialCardName = name;
        }
        public void RemoveTutorialCard() => _tutorialCardName = "";
        public string GetTutorialCard() => _tutorialCardName;

        #endregion

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
                || !targetCard.CardUI.IsHighlighted)
                return;

            _effectCard.ContinueAction(targetCard);
        }

        public void ResetApplyingEffect()
        {
            holdingCard = null;
            _effectCard = null;
            SetMask(selectingLayerMask);
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
                case Target.ENEMY:
                case Target.CARD:
                    layerMask = targetLayerMask;
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