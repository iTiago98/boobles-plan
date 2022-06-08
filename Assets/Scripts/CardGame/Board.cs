using CardGame.Cards.DataModel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame.Level
{
    public class Board : MonoBehaviour
    {
        public static Board Instance { private set; get; }

        public Deck playerDeck;
        public Deck opponentDeck;

        public Hand playerHand;
        public Hand opponentHand;

        public List<CardZone> playerCardZone;
        public List<CardZone> opponentCardZone;

        public Transform waitingSpot;


        private void Awake()
        {
            Instance = this;
        }

        public void DrawCards(int cardNumber, Contender contender)
        {
            switch (contender?.role)
            {
                case Contender.Role.PLAYER:
                    playerDeck.DrawCards(cardNumber);
                    break;
                case Contender.Role.OPPONENT:
                    opponentDeck.DrawCards(cardNumber);
                    break;
                default:
                    playerDeck.DrawCards(cardNumber);
                    opponentDeck.DrawCards(cardNumber);
                    break;
            }
        }

        public void DiscardCards(int cardNumber, Contender contender)
        {
            switch (contender.role)
            {
                case Contender.Role.PLAYER:
                    opponentHand.DiscardCards(cardNumber);
                    break;
                case Contender.Role.OPPONENT:
                    playerHand.DiscardCards(cardNumber);
                    break;
            }
        }

        public void InitializeDecks(List<CardsData> playerCards, List<CardsData> opponentCards)
        {
            playerDeck.Initialize(playerHand, playerCards);
            opponentDeck.Initialize(opponentHand, opponentCards);
        }

        public bool CardsOnTable(Contender contender)
        {
            switch (contender.role)
            {
                case Contender.Role.PLAYER:
                    foreach (CardZone zone in playerCardZone)
                    {
                        if (zone.numCards > 0) return true;
                    }
                    break;
                case Contender.Role.OPPONENT:
                    foreach (CardZone zone in opponentCardZone)
                    {
                        if (zone.numCards > 0) return true;
                    }
                    break;
            }

            return false;
        }
    }
}