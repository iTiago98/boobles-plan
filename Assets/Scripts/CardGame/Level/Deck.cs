using CardGame.Cards;
using CardGame.Cards.DataModel;
using CardGame.Managers;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame.Level
{
    public class Deck : MonoBehaviour
    {
        public GameObject cardPrefab;
        public CardsData emptyDeckCardData;

        private int _maxCardNumber;
        private Contender _contender;
        private Hand _hand;
        private List<CardsData> _deckCards;

        public delegate void DrawCardEffects();
        private DrawCardEffects drawCardEffectsDelegate;

        public void Initialize(Contender contender, Hand hand, List<CardsData> deckCards)
        {
            _contender = contender;
            _hand = hand;
            CopyCardsList(deckCards);
            UIManager.Instance.ShowRemainingCards(contender);
            UpdateRemainingCards();
        }

        public void DrawCards(int numCards)
        {
            for (int i = 0; i < numCards; i++)
            {
                GameObject cardObj;
                CardsData data;

                if (_deckCards.Count == 0)
                {
                    // Instantiate card
                    cardObj = Instantiate(cardPrefab, transform.position, cardPrefab.transform.rotation, _hand.transform);

                    // Take data
                    data = new CardsData(emptyDeckCardData);
                }
                else
                {
                    // Instantiate card
                    cardObj = Instantiate(cardPrefab, transform.position, cardPrefab.transform.rotation, _hand.transform);

                    // Take data from scriptable
                    int index = Random.Range(0, _deckCards.Count);
                    data = _deckCards[index];
                    _deckCards.RemoveAt(index);
                }

                // Add card to hand
                Card card = cardObj.GetComponent<Card>();
                card.Initialize(_contender, data, cardRevealed: false);
                _hand.AddCard(card);

                // Apply end round effects
                drawCardEffectsDelegate?.Invoke();
            }

            CheckCardNumber();
        }

        public void DiscardCards(int numCards)
        {
            for (int i = 0; i < numCards; i++)
            {
                if (_deckCards.Count > 0)
                {
                    Vector3 position = transform.position + new Vector3(0, 0, -0.1f);
                    GameObject cardObj = Instantiate(cardPrefab, position, cardPrefab.transform.rotation);

                    // Take data from scriptable
                    int index = Random.Range(0, _deckCards.Count);
                    CardsData data = _deckCards[index];
                    _deckCards.RemoveAt(index);

                    Card card = cardObj.GetComponent<Card>();
                    card.Initialize(null, data, cardRevealed: true);
                    card.Destroy();
                }
            }

            CheckCardNumber();
        }

        private void CopyCardsList(List<CardsData> cards)
        {
            _deckCards = new List<CardsData>();
            foreach (CardsData data in cards)
            {
                CardsData temp = new CardsData();
                temp.name = data.name;
                temp.sprite = data.sprite;
                temp.cost = data.cost;
                temp.strength = data.strength;
                temp.defense = data.defense;
                temp.type = data.type;
                temp.effects = data.effects;

                _deckCards.Add(temp);
            }

            _maxCardNumber = _deckCards.Count;
        }

        public void AddDrawCardEffects(DrawCardEffects method)
        {
            if (drawCardEffectsDelegate == null)
            {
                drawCardEffectsDelegate = method;
            }
            else
            {
                drawCardEffectsDelegate += method;
            }
        }

        public void RemoveDrawCardEffect(DrawCardEffects method)
        {
            drawCardEffectsDelegate -= method;
        }

        private void CheckCardNumber()
        {
            if (_deckCards.Count <= 0)
            {
                GetComponent<SpriteRenderer>().sprite = null;
            }

            UpdateRemainingCards();
        }

        private void UpdateRemainingCards()
        {
            UIManager.Instance.UpdateRemainingCards(_deckCards.Count, _maxCardNumber, _contender);
        }
    }
}