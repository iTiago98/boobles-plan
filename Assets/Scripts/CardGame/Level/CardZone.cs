using Booble.CardGame;
using Booble.CardGame.Cards;
using Booble.CardGame.Level;
using Booble.CardGame.Managers;
using UnityEngine;

namespace Booble.CardGame.Level
{
    public class CardZone : CardContainer, IClickable
    {
        //public Transform cardsPosition;

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

        public void OnMouseHoverEnter()
        {
            if (isNotEmpty) GetCard().ShowExtendedDescription();
        }

        public void OnMouseHoverExit()
        {
            if (isNotEmpty) GetCard().HideExtendedDescription();
        }

        #endregion

        public void AddCard(Card card)
        {
            AddCard(card, transform);
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