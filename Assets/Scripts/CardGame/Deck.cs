using DataModel;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    private Hand _hand;
    private List<CardsData> _deckCards;

    public void Initialize(Hand hand, List<CardsData> deckCards)
    {
        _hand = hand;
        _deckCards = new List<CardsData>(deckCards);
    }

    public void DrawCards(int numCards)
    {
        for (int i = 0; i < numCards; i++)
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
