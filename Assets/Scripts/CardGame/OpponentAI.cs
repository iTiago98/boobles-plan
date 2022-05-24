using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentAI : MonoBehaviour
{
    private Contender _contender;

    public void Initialize(Contender contender)
    {
        _contender = contender;
    }

    private void Update()
    {
        Play();

    }

    private void Play()
    {
        CardZone emptyCardZone = EmptyCardZone();
        if (_contender.currentMana > 0 && emptyCardZone)
        {
            foreach (GameObject cardObj in _contender.hand.cards)
            {
                Card card = cardObj.GetComponent<Card>();

                if (card.manaCost <= _contender.currentMana)
                {
                    PlayCard(card, emptyCardZone);
                    break;
                }
            }
        }
        else
        {
            SkipTurn();
        }
    }

    private void PlayCard(Card card, CardZone emptyCardZone)
    {
        card.RemoveFromContainer();
        emptyCardZone.AddCard(card);
    }

    private CardZone EmptyCardZone()
    {
        foreach (CardZone cardZone in _contender.cardZones)
        {
            if (cardZone.GetCard() == null) return cardZone;
        }

        return null;
    }

    private void SkipTurn()
    {
        TurnManager.Instance.FinishTurn();
    }
}
