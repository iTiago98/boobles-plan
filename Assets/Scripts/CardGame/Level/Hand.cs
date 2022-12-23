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
        public Contender contender { private set; get; }
        public bool isDiscarding { private set; get; }
        public bool busy { private set; get; }

        private List<Card> _listToDiscard = new List<Card>();

        private int _handCapacity;

        public void Initialize(Contender contender)
        {
            this.contender = contender;
            _handCapacity = CardGameManager.Instance.settings.handCapacity;
        }

        public void AddCard(Card card)
        {
            AddCard(card, transform);
        }

        public void DiscardCards(int cardNumber)
        {
            busy = true;

            int number = (cardNumber <= numCards) ? cardNumber : numCards;
            System.Random random = new System.Random();
            for (int i = 0; i < number; i++)
            {
                int index = random.Next(0, numCards);
                Card card = cards[index].GetComponent<Card>();

                if (_listToDiscard.Contains(card)
                    || card.IsAlternateWinConditionCard())
                {
                    i--;
                    continue;
                }

                _listToDiscard.Add(card);
            }

            StartCoroutine(DiscardCoroutine(wheelEffect: false));
        }

        public void DiscardAll()
        {
            busy = true;

            for (int i = 0; i < numCards; i++)
            {
                Card card = cards[i].GetComponent<Card>();
                if (card.IsAlternateWinConditionCard())
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
                card.DestroyCard();

                if (!wheelEffect) yield return new WaitUntil(() => card.destroyed);

                _listToDiscard.RemoveAt(0);
            }

            _listToDiscard.Clear();
            busy = false;
        }

        public Card StealCard()
        {
            System.Random random = new System.Random();
            int index = random.Next(0, numCards);
            GameObject cardObj = cards[index];

            RemoveCard(cardObj);
            Card card = cardObj.GetComponent<Card>();
            return card;
        }

        public bool HasAlternateWinConditionCard()
        {
            foreach (GameObject cardObj in cards)
            {
                Card card = cardObj.GetComponent<Card>();
                if (card.IsAlternateWinConditionCard()) return true;
            }

            return false;
        }

        private int _discardingNumber;

        private void SetDiscardingNumber(int number)
        {
            _discardingNumber = number;
        }

        public void SubstractDiscarding()
        {
            _discardingNumber--;
        }

        public bool CheckStartDiscarding()
        {
            SetDiscardingNumber(numCards);

            if (_discardingNumber > _handCapacity)
            {
                isDiscarding = true;
                ChangeScale(CardGameManager.Instance.settings.highlightScale);
            }

            return isDiscarding;
        }

        public void CheckDiscarding()
        {
            StartCoroutine(CheckDiscardingCoroutine());
        }

        private IEnumerator CheckDiscardingCoroutine()
        {
            if (contender.isPlayer)
            {
                if (_discardingNumber == _handCapacity && isDiscarding)
                {
                    isDiscarding = false;
                    ChangeScale(CardGameManager.Instance.settings.defaultScale);

                    yield return new WaitUntil(() => numCards == _handCapacity);

                    TurnManager.Instance.ChangeTurn();
                }
            }
            else
            {
                int number = numCards - _handCapacity;
                if (number > 0)
                {
                    DiscardCards(number);
                    yield return new WaitWhile(() => busy);
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