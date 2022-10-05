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
            if(numCards >= TurnManager.Instance.settings.handCapacity)
            {
                DiscardFirst();
            }

            AddCard(card, transform);
        }

        public void DiscardCards(int cardNumber)
        {
            int number = (cardNumber <= numCards) ? cardNumber : numCards;
            System.Random random = new System.Random();
            for (int i = 0; i < number; i++)
            {
                int index = random.Next(0, numCards);
                cards[index].GetComponent<Card>().Destroy();
            }
        }

        private void DiscardFirst()
        {
            cards[0].GetComponent<Card>().Destroy();
        }

        public void DiscardAll()
        {
            for (int i = 0; i < numCards; i++)
            {
                cards[i].GetComponent<Card>().Destroy();
            }
        }
    }
}