using Booble.CardGame;
using Booble.CardGame.Cards;
using Booble.CardGame.Level;
using Booble.CardGame.Managers;
using UnityEngine;

namespace Booble.CardGame.Level
{
    public class CardZone : CardContainer, IClickable
    {
        [SerializeField] private bool _isFieldZone;

        private SpriteRenderer _highlightSprite;
        private bool _isHighlighted => _highlightSprite.enabled;

        private void Start()
        {
            _highlightSprite = GetComponent<SpriteRenderer>();
        }

        #region IClickable methods
        public void OnMouseLeftClickDown(MouseController mouseController)
        {

        }

        public void OnMouseLeftClickUp(MouseController mouseController)
        {
            Card card = mouseController.holdingCard;

            if (card != null && _isHighlighted)
            {
                mouseController.SetHolding(null);
                card.Play(this);
                Board.Instance.RemoveCardZonesHighlight(card);
                UIManager.Instance.HidePlayButtons();
            }
        }

        public void OnMouseHoverEnter() { }

        public void OnMouseHoverExit() { }

        #endregion

        public override void AddCard(GameObject cardObj)
        {
            base.AddCard(cardObj);
            GetComponent<BoxCollider>().enabled = false;
        }

        public override void RemoveCard(GameObject card)
        {
            base.RemoveCard(card);
            GetComponent<BoxCollider>().enabled = true;
        }

        public Card GetCard()
        {
            return (cards.Count > 0) ? cards[0].GetComponent<Card>() : null;
        }

        public void ShowHighlight(bool show)
        {
            _highlightSprite.enabled = show;
        }

    }
}