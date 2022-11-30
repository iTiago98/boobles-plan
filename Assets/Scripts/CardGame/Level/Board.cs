using CardGame.Cards;
using CardGame.Cards.DataModel;
using CardGame.Managers;
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

        [Header("Background")]
        public SpriteRenderer background;
        public List<Sprite> backgroundList;

        [Header("Decks")]
        public Deck playerDeck;
        public Deck opponentDeck;

        [Header("Hands")]
        public Hand playerHand;
        public Hand opponentHand;

        [Header("Card Zones")]
        public List<CardZone> playerCardZone;
        public List<CardZone> opponentCardZone;

        public CardZone playerFieldCardZone;
        public CardZone opponentFieldCardZone;

        public Transform waitingSpot;

        private void Awake()
        {
            Instance = this;
        }

        public void InitializeDecks(List<CardsData> playerCards, List<CardsData> opponentCards)
        {
            playerDeck.Initialize(CardGameManager.Instance.player, playerHand, playerCards);
            opponentDeck.Initialize(CardGameManager.Instance.opponent, opponentHand, opponentCards);
        }

        public void InitializeBackground(Opponent_Name opponentName)
        {
            switch (opponentName)
            {
                case Opponent_Name.Tutorial:
                    background.sprite = backgroundList[0];
                    break;
                case Opponent_Name.Citriano:
                    background.sprite = backgroundList[1];
                    break;
                case Opponent_Name.PingPongBros:
                    background.sprite = backgroundList[2];
                    break;
                case Opponent_Name.Secretary:
                    background.sprite = backgroundList[3];
                    break;
                case Opponent_Name.Jefe:
                    background.sprite = backgroundList[4];
                    break;
            }
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

        public void DiscardAll(Contender contender)
        {
            GetHand(contender).DiscardAll();
        }

        #region Getters

        public CardZone GetEmptyCardZone(Contender contender)
        {
            List<CardZone> cardZone = GetCardZones(contender);
            foreach (CardZone zone in cardZone)
            {
                if (zone.numCards == 0) return zone;
            }

            return null;
        }

        public List<Card> GetCardsOnTable(Contender contender)
        {
            List<CardZone> cardZone = GetCardZones(contender);
            List<Card> temp = new List<Card>();
            foreach (CardZone zone in cardZone)
            {
                Card card = zone.GetCard();
                if (card != null) temp.Add(card);
            }
            return temp;
        }

        public bool AreCardsOnTable(Contender contender)
        {
            List<CardZone> cardZone = GetCardZones(contender);
            foreach (CardZone zone in cardZone)
            {
                if (zone.numCards > 0) return true;
            }

            return false;
        }

        public int NumCardsOnTable(Contender contender)
        {
            List<CardZone> cardZone = GetCardZones(contender);
            int num = 0;
            foreach (CardZone zone in cardZone)
            {
                if (zone.numCards > 0) num++;
            }
            return num;
        }

        public Hand GetHand(Contender contender)
        {
            return (contender.role == Contender.Role.PLAYER) ? playerHand : opponentHand;
        }

        public Deck GetDeck(Contender contender)
        {
            return (contender.role == Contender.Role.PLAYER) ? playerDeck : opponentDeck;
        }

        public List<CardZone> GetCardZones(Contender contender)
        {
            return (contender.role == Contender.Role.PLAYER) ? playerCardZone : opponentCardZone;
        }

        public int GetPositionFromCard(Card card)
        {
            List<CardZone> cardZones = GetCardZones(card.contender);

            for(int i = 0; i < cardZones.Count; i++)
            {
                CardZone cardZone = cardZones[i];
                if (cardZone.GetCard()?.name == card.name) return i;
            }

            return -1;
        }

        public CardZone GetCardZoneFromPosition(int position, Contender contender)
        {
            return GetCardZones(contender)[position];
        }

        public CardZone GetFieldCardZone(Contender contender)
        {
            return (contender.role == Contender.Role.PLAYER) ? playerFieldCardZone : opponentFieldCardZone;
        }

        #endregion

        #region Destroy

        public void DestroyCards(Contender contender)
        {
            foreach (CardZone zone in GetCardZones(contender)) zone.GetCard()?.Destroy();
            Card fieldCard = GetFieldCardZone(contender).GetCard();
            if (fieldCard != null) fieldCard.Destroy();
        }

        public void DestroyAll()
        {
            DestroyCards(CardGameManager.Instance.player);
            DestroyCards(CardGameManager.Instance.opponent);
        }

        #endregion

        #region Highlight

        public void HighlightTargets(List<Card> possibleTargets)
        {
            List<CardZone> temp = new List<CardZone>();
            temp.AddRange(playerCardZone);
            temp.Add(playerFieldCardZone);
            temp.AddRange(opponentCardZone);
            temp.Add(opponentFieldCardZone);

            foreach (CardZone cardZone in temp)
            {
                Card card = cardZone.GetCard();
                if (card != null)
                {
                    card.ShowHighlight(possibleTargets.Contains(card));
                }
            }
        }

        public void HighlightZoneTargets(CardType type, Contender contender, bool show)
        {
            switch (type)
            {
                case CardType.ARGUMENT:
                    {
                        List<CardZone> cardZones = GetCardZones(contender);
                        foreach (CardZone zone in cardZones)
                        {
                            if (!show || zone.isEmpty) zone.ShowHighlight(show);
                        }
                        break;
                    }
                case CardType.FIELD:
                    {
                        CardZone zone = GetFieldCardZone(contender);
                        zone.ShowHighlight(show);
                        break;
                    }
            }
        }

        #endregion

    }
}