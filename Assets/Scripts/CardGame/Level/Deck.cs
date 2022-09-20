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
        private Hand _hand;
        private List<CardsData> _deckCards;

        public void Initialize(Hand hand, List<CardsData> deckCards)
        {
            _hand = hand;
            CopyCardsList(deckCards);
        }

        public void DrawCards(int numCards)
        {
            for (int i = 0; i < numCards; i++)
            {
                if (_deckCards.Count == 0)
                {
                    TurnManager.Instance.GetContenderFromHand(_hand).ReceiveDamage(1);
                }
                else
                {
                    // Instantiate card
                    GameObject cardObj = Instantiate(cardPrefab, transform.position, cardPrefab.transform.rotation, _hand.transform);

                    // Take data from scriptable
                    int index = Random.Range(0, _deckCards.Count);
                    CardsData data = _deckCards[index];
                    _deckCards.RemoveAt(index);

                    // Add card to hand
                    Card card = cardObj.GetComponent<Card>();
                    card.Initialize(_hand, data);
                    _hand.AddCard(card);
                }
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
    }
}