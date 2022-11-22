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
        public Contender contender;

        public bool isDiscarding;

        public void AddCard(Card card)
        {
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

        public void DiscardAll()
        {
            for (int i = 0; i < numCards; i++)
            {
                cards[i].GetComponent<Card>().Destroy();
            }
        }

        public void CheckDiscarding()
        {
            CheckDiscarding(numCards);
        }
        public void CheckDiscarding(int numCards)
        {
            int handCapacity = CardGameManager.Instance.settings.handCapacity;
            if (numCards >= handCapacity)
            {
                if (contender.isPlayer)
                {
                    isDiscarding = true;
                    MouseController.Instance.SetDiscarding();
                    ChangeScale(CardGameManager.Instance.settings.highlightScale);
                }
                else
                {
                    DiscardCards(numCards - handCapacity);
                }
            }
            else
            {
                if (contender.isPlayer)
                {
                    isDiscarding = false;
                    ChangeScale(CardGameManager.Instance.settings.defaultScale);
                    TurnManager.Instance.ContinueClash();
                }
            }
        }

        private void ChangeScale(float scale)
        {
            foreach (GameObject cardObj in cards)
            {
                cardObj.transform.DOScale(scale, 0.2f);
            }
        }
    }
}