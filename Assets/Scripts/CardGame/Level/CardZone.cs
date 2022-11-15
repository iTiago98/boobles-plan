using CardGame.Cards;
using CardGame.Cards.DataModel;
using CardGame.Managers;
using UnityEngine;
using DG.Tweening;

namespace CardGame.Level
{
    public class CardZone : CardContainer, IClickable
    {
        public Transform cardsPosition;

        [SerializeField] private bool isFieldZone;
        private bool _isEmpty => numCards == 0;
        private bool _clickable;

        bool IClickable.clickable { get => _clickable; set => _clickable = value; }
        GameObject IClickable.gameObject { get => gameObject; set => Debug.Log(""); }

        #region IClickable methods
        public void OnMouseLeftClickDown(MouseController mouseController)
        {

        }

        public void OnMouseLeftClickUp(MouseController mouseController)
        {
            Card card = mouseController.holdingCard;
            if (card != null)
            {
                // If the player has enough mana
                if (EnoughMana(card))
                {
                    switch (card.type)
                    {
                        case CardType.ARGUMENT: CheckArgument(card, mouseController); break;
                        case CardType.ACTION: CheckAction(card, mouseController); break;
                        case CardType.FIELD: CheckField(card, mouseController); break;
                    }
                }
                else
                {
                    // Animate mana counter to show not enough mana
                    Debug.Log("Not enough mana");
                    card.OnMouseLeftClickUp(mouseController);
                }
            }
        }

        public void OnMouseHoverEnter()
        {
        }

        public void OnMouseHoverExit()
        {
        }

        public void OnMouseRightClick()
        {
        }

        #endregion

        private void CheckArgument(Card card, MouseController mouseController)
        {
            if (isEmpty && !isFieldZone)
            {
                mouseController.SetHolding(null);
                card.Play(this);
            }
            else
            {
                // Send card back to previous container
                card.OnMouseLeftClickUp(mouseController);
            }
        }

        private void CheckAction(Card card, MouseController mouseController)
        {
            // If effect not appliable
            if (card.hasEffect && !card.effect.IsAppliable())
            {
                //Debug.Log("Effect not appliable");
                card.OnMouseLeftClickUp(mouseController);
                return;
            }

            card.Play(null);
        }

        private void CheckField(Card card, MouseController mouseController)
        {
            if (!isEmpty)
            {
                GetCard().Destroy();
            }

            if (isFieldZone)
            {
                mouseController.SetHolding(null);
                card.Play(this);
            }
            else
            {
                // Send card back to previous container
                card.OnMouseLeftClickUp(mouseController);
            }
        }

        public void AddCard(Card card)
        {
            AddCard(card, cardsPosition);
        }

        public Card GetCard()
        {
            return (cards.Count > 0) ? cards[0].GetComponent<Card>() : null;
        }

        private bool EnoughMana(Card card)
        {
            Contender contender = CardGameManager.Instance.currentPlayer;

            return contender.freeMana || card.manaCost <= contender.currentMana;
        }
    }
}