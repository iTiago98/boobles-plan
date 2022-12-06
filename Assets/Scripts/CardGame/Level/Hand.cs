using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CardGame.Managers;
using CardGame.Cards;
using CardGame.Cards.DataModel;
using CardGame.Cards.DataModel.Effects;

namespace CardGame.Level
{
    public class Hand : CardContainer
    {
        public Contender contender;

        public bool isDiscarding;

        private List<Card> _listToDiscard = new List<Card>();

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
                Card card = cards[index].GetComponent<Card>();
                if (card.type == CardType.ACTION && card.hasEffect && card.effect.type == EffectType.ALTERNATE_WIN_CONDITION)
                    continue;

                _listToDiscard.Add(cards[index].GetComponent<Card>());
            }

            StartCoroutine(DiscardCoroutine(wheelEffect: false));
        }

        public void DiscardAll()
        {
            for (int i = 0; i < numCards; i++)
            {
                Card card = cards[i].GetComponent<Card>();
                if (card.type == CardType.ACTION && card.hasEffect && card.effect.type == EffectType.ALTERNATE_WIN_CONDITION)
                    continue;

                _listToDiscard.Add(card);
            }

            StartCoroutine(DiscardCoroutine(wheelEffect: true));
        }

        private IEnumerator DiscardCoroutine(bool wheelEffect)
        {
            int loops = _listToDiscard.Count;
            for (int i = 0; i < loops; i++)
            {
                Card card = _listToDiscard[0];
                card.Destroy();

                if (!wheelEffect) yield return new WaitUntil(() => card == null);
                _listToDiscard.RemoveAt(0);
            }

            _listToDiscard.Clear();
            TurnManager.Instance.ContinueFlow();
        }

        public bool HasAlternateWinConditionCard()
        {
            foreach(GameObject cardObj in cards)
            {
                Card card = cardObj.GetComponent<Card>();
                if (card.hasEffect && card.effect.type == EffectType.ALTERNATE_WIN_CONDITION) return true;
            }

            return false;
        }

        public void CheckDiscarding()
        {
            CheckDiscarding(numCards);
        }
        public void CheckDiscarding(int numCards)
        {
            int handCapacity = CardGameManager.Instance.settings.handCapacity;
            if (numCards > handCapacity)
            {
                if (contender.isPlayer)
                {
                    isDiscarding = true;
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
                    TurnManager.Instance.ChangeTurn();
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