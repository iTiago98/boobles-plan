using CardGame.Cards;
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

        public void InitializeDecks(List<CardsData> playerCards, List<CardsData> opponentCards)
        {
            playerDeck.Initialize(playerHand, playerCards);
            opponentDeck.Initialize(opponentHand, opponentCards);
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
            GetHand(contender).DiscardCards(cardNumber);
        }

        public List<Card> CardsOnTable(Contender contender)
        {
            List<CardZone> cardZone = GetCardZone(contender);
            List<Card> temp = new List<Card>();
            foreach (CardZone zone in cardZone)
            {
                Card card = zone.GetCard();
                if (card != null) temp.Add(card);
            }
            return temp;
        }

        public bool NumCardsOnTable(Contender contender)
        {
            List<CardZone> cardZone = GetCardZone(contender);
            foreach (CardZone zone in cardZone)
            {
                if (zone.numCards > 0) return true;
            }

            return false;
        }

        private Hand GetHand(Contender contender)
        {
            return (contender.role == Contender.Role.PLAYER) ? playerHand : opponentHand;
        }

        private List<CardZone> GetCardZone(Contender contender)
        {
            return (contender.role == Contender.Role.PLAYER) ? playerCardZone : opponentCardZone;
        }
    }
}