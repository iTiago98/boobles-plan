using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardZone : CardContainer, IClickable
{
    public Transform cardsPosition;

    private bool _clickable;

    bool IClickable.clickable { get => _clickable; set => _clickable = value; }
    GameObject IClickable.gameObject { get => gameObject; set => Debug.Log(""); }

    public void OnMouseLeftClickDown(MouseController mouseController)
    {

    }

    public void OnMouseLeftClickUp(MouseController mouseController)
    {
        Card card = mouseController.holding;
        if (card != null)
        {
            // If there's already a card 
            if (numCards != 0)
            {
                // Send card back to previous container
                card.OnMouseLeftClickUp(mouseController);
            }
            else
            {
                // If the player has enough mana
                if (card.manaCost <= TurnManager.Instance.currentPlayer.currentMana)
                {
                    mouseController.holding = null;
                    AddCard(card);
                }
                else
                {
                    // Animate mana counter to show not enough mana
                    Debug.Log("Not enough mana");
                    card.OnMouseLeftClickUp(mouseController);
                }
            }
        }
    }

    public void AddCard(Card card)
    {
        base.AddCard(card, cardsPosition);

        TurnManager.Instance.currentPlayer.MinusMana(card.manaCost);
        TurnManager.Instance.UpdateUI();
    }

    public Card GetCard()
    {
        return (cards.Count > 0) ? cards[0].GetComponent<Card>() : null;
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
}
