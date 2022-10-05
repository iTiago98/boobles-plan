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

        private Hand _hand;
        private List<CardsData> _deckCards;

        public delegate void DrawCardEffects();
        private DrawCardEffects drawCardEffectsDelegate;

        public void Initialize(Hand hand, List<CardsData> deckCards)
        {
            _hand = hand;
            CopyCardsList(deckCards);
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
                card.Initialize(_hand, data, cardRevealed: false);
                _hand.AddCard(card);

                // Apply end round effects
                drawCardEffectsDelegate?.Invoke();
            }
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
    }
}