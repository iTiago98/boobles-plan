using CardGame.Cards;
using CardGame.Cards.DataModel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardGame.Managers.TurnManager;

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

        public void DrawCards(int cardNumber, Turn turn)
        {
            switch (turn)
            {
                case Turn.PLAYER:
                    playerDeck.DrawCards(cardNumber);
                    break;
                case Turn.OPPONENT:
                    opponentDeck.DrawCards(cardNumber);
                    break;
                case Turn.START:
                    playerDeck.DrawCards(cardNumber);
                    opponentDeck.DrawCards(cardNumber);
                    break;
            }
        }

        public void DiscardCards(int cardNumber, Contender contender)
        {
            GetHand(contender).DiscardCards(cardNumber);
        }

        public bool EmptyCardZone(Contender contender)
        {
            List<CardZone> cardZone = GetCardZone(contender);
            foreach (CardZone zone in cardZone)
            {
                if (zone.numCards > 0) return false;
            }

            return true;
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

        public int NumCardsOnTable(Contender contender)
        {
            List<CardZone> cardZone = GetCardZone(contender);

            int num = 0;
            foreach (CardZone zone in cardZone)
            {
                if (zone.numCards > 0) num++;
            }
            return num;
        }

        public bool AreCardsOnTable(Contender contender)
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

        public void DestroyAll()
        {
            foreach (CardZone cardZone in playerCardZone) cardZone.GetCard()?.Destroy();
            foreach (CardZone cardZone in opponentCardZone) cardZone.GetCard()?.Destroy();
        }

        public int MaxCardNumber()
        {
            return playerCardZone.Count;
        }

        public void HighlightTargets(List<Card> possibleTargets)
        {
            List<CardZone> temp = new List<CardZone>();
            temp.AddRange(playerCardZone);
            temp.AddRange(opponentCardZone);

            foreach (CardZone cardZone in temp)
            {
                Card card = cardZone.GetCard();
                if (card != null)
                {
                    card.highlight.SetActive(possibleTargets.Contains(card));
                }
            }
        }
    }
}