using CardGame.Cards;
using CardGame.Managers;
using UnityEngine;

namespace CardGame.Level
{
    public class CardZone : CardContainer, IClickable
    {
        public Transform cardsPosition;

        [SerializeField] private bool _isFieldZone;

        private SpriteRenderer _highlightSprite;
        private bool _isHighlighted => _highlightSprite.enabled;
        private bool _clickable;

        bool IClickable.clickable { get => _clickable; set => _clickable = value; }
        GameObject IClickable.gameObject { get => gameObject; set => Debug.Log(""); }

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
                Board.Instance.HighlightZoneTargets(card.type, card.contender, show: false);
                UIManager.Instance.HidePlayButtons();
                UIManager.Instance.SetEndTurnButtonInteractable(true);
            }
        }

        public void OnMouseHoverEnter()
        {
            if (!isEmpty) GetCard().ShowExtendedDescription();
        }

        public void OnMouseHoverExit()
        {
            if (!isEmpty) GetCard().HideExtendedDescription();
        }

        //public void OnMouseRightClick()
        //{
        //}

        #endregion

        public void AddCard(Card card)
        {
            AddCard(card, cardsPosition);
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