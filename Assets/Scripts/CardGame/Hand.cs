using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Hand : CardContainer, IClickable
{
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
            AddCard(card);
            mouseController.holding = null;
        }
    }

    public void AddCard(Card card)
    {
        base.AddCard(card, transform);
    }

    void IClickable.OnMouseHoverEnter()
    {
    }

    void IClickable.OnMouseHoverExit()
    {
    }

    void IClickable.OnMouseRightClick()
    {
    }
}
