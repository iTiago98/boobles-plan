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
            Card card = mouseController.holding;
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
            if (_isEmpty)
            {
                // Add card to container
                mouseController.SetHolding(null);
                AddCard(card);

                TurnManager.Instance.currentPlayer.MinusMana(card.manaCost);
                UIManager.Instance.UpdateUIStats();
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
            //if (card.hasEffect && !card.effect.IsAppliable())
            //{
                card.OnMouseLeftClickUp(mouseController);
            //}

            //TurnManager.Instance.currentPlayer.MinusMana(card.manaCost);
            //TurnManager.Instance.UpdateUIStats();

            // Move card to board waiting spot
            /*Transform dest = Board.Instance.waitingSpot;
            card.SetMoveWithMouse(false);
            card.transform.DOMove(dest.position, 1f);

            mouseController.SetHolding(null);*/

            // If the effect has no target, we apply the effect and destroy the card
            //if (card.hasEffect && card.effect.GetTarget() == "")
            //{
            //    card.effect.Apply(null);
            //    mouseController.CheckMask(null);
            //    card.Destroy();
            //}
            //else
            //{
            //    TurnManager.Instance.SetEndButtonInteractable(false);
            //}
        }

        private void CheckField(Card card, MouseController mouseController)
        {

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
            return card.manaCost <= TurnManager.Instance.currentPlayer.currentMana;
        }
    }
}