using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CardGame.Managers;
using CardGame.Cards;

namespace CardGame.Level
{
    public class Hand : CardContainer
    {
        public void AddCard(Card card)
        {
            AddCard(card, transform);
        }

        public void DiscardCards(int cardNumber)
        {
            System.Random random = new System.Random();
            for (int i = 0; i < cardNumber; i++)
            {
                int index = random.Next(0, numCards);
                RemoveCard(cards[index]);
            }
        }
    }
}