using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Booble.CardGame.Cards;
using Booble.CardGame.Managers;

namespace Booble.CardGame.Level
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

        #region Discard Cards

        public void DiscardCards(int cardNumber)
        {
            busy = true;

            List<int> indexList = new List<int>();
            for (int i = 0; i < numCards; i++) indexList.Add(i);

            if (HasAlternateWinConditionCard())
            {
                indexList.Remove(GetAlternateWinConditionCardIndex());
            }

            int number = cardNumber <= indexList.Count ? cardNumber : indexList.Count;

            for (int i = 0; i < number; i++)
            {
                int index = indexList[Random.Range(0, indexList.Count)];

                Card card = cards[index].GetComponent<Card>();

                _listToDiscard.Add(card);
                indexList.Remove(index);
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

                //if (!wheelEffect) yield return new WaitUntil(() => card.destroyed);

                _listToDiscard.RemoveAt(0);
            }

            yield return new WaitUntil(() => cardsAtPosition);

            _listToDiscard.Clear();
            busy = false;
        }

        #endregion

        #region Steal Cards

        public Card StealCard()
        {
            System.Random random = new System.Random();
            int index = random.Next(0, numCards);
            GameObject cardObj = cards[index];

            RemoveCard(cardObj);
            Card card = cardObj.GetComponent<Card>();
            return card;
        }

        #endregion

        #region Check Discarding

        private int _discardingNumber;
        private List<Card> _discardingCards = new List<Card>();

        private void SetDiscardingNumber(int number)
        {
            _discardingNumber = number;
        }

        public void AddDiscarding(Card card)
        {
            if (_discardingCards.Count >= _discardingNumber) return;

            card.CardUI.ShowHighlight(true);
            _discardingCards.Add(card);

            if (_discardingCards.Count >= _discardingNumber)
            {
                UIManager.Instance.SetEndTurnButtonInteractable(true);
            }
        }

        public void SubstractDiscarding(Card card)
        {
            card.CardUI.ShowHighlight(false);
            _discardingCards.Remove(card);

            UIManager.Instance.SetEndTurnButtonInteractable(false);
        }

        public bool CheckStartDiscarding()
        {
            int cardsToDiscard = numCards - _handCapacity;

            if (cardsToDiscard > 0 && contender.isPlayer)
            {
                SetDiscardingNumber(cardsToDiscard);
                isDiscarding = true;
                ChangeScale(CardGameManager.Instance.settings.highlightScale);
                UIManager.Instance.SetEndTurnButtonInteractable(false);
                UIManager.Instance.SetEndTurnButtonText(TurnManager.Turn.DISCARDING);
            }

            return isDiscarding;
        }

        public void EndDiscarding()
        {
            StartCoroutine(EndDiscardingCoroutine());
        }

        private IEnumerator EndDiscardingCoroutine()
        {
            if (contender.isPlayer)
            {
                UIManager.Instance.SetEndTurnButtonInteractable(false);

                isDiscarding = false;
                ChangeScale(CardGameManager.Instance.settings.defaultScale);

                Card aux = _discardingCards[0];

                foreach (Card card in _discardingCards)
                {
                    card.DestroyCard();
                }

                yield return new WaitUntil(() => aux.destroyed);
                yield return new WaitUntil(() => cardsAtPosition);

                _discardingCards.Clear();

                TurnManager.Instance.ChangeTurn();
            }
            else
            {
                int number = numCards - _handCapacity;
                if (number > 0)
                {
                    DiscardCards(number);
                    yield return new WaitWhile(() => busy);
                }
                TurnManager.Instance.StartTurn();
            }
        }

        private void ChangeScale(float scale)
        {
            foreach (GameObject cardObj in cards)
            {
                cardObj.transform.DOScale(scale, 0.2f);
            }
        }

        #endregion

        public bool HasAlternateWinConditionCard()
        {
            foreach (GameObject cardObj in cards)
            {
                Card card = cardObj.GetComponent<Card>();
                if (card.IsAlternateWinConditionCard()) return true;
            }

            return false;
        }

        private int GetAlternateWinConditionCardIndex()
        {
            foreach (GameObject cardObj in cards)
            {
                Card card = cardObj.GetComponent<Card>();
                if (card.IsAlternateWinConditionCard()) return cards.IndexOf(cardObj);
            }

            return -1;
        }

        public Card GetCard(string name)
        {
            foreach (GameObject cardObj in cards)
            {
                Card card = cardObj.GetComponent<Card>();
                if (card.data.name == name)
                {
                    return card;
                }
            }
            return null;
        }
    }
}