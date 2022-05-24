using DataModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public Deck playerDeck;
    public Deck opponentDeck;

    public Hand playerHand;
    public Hand opponentHand;

    public List<CardZone> playerCardZone;
    public List<CardZone> opponentCardZone;

    public void DrawCards(int cardNumber)
    {
        playerDeck.DrawCards(cardNumber);
        opponentDeck.DrawCards(cardNumber);
    }

    public void InitializeDecks(List<CardsData> playerCards, List<CardsData> opponentCards)
    {
        playerDeck.Initialize(playerHand, playerCards);
        opponentDeck.Initialize(opponentHand, opponentCards);
    }
}
