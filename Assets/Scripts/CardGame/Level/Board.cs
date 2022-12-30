using Booble.CardGame.Cards;
using Booble.CardGame.Cards.DataModel;
using Booble.CardGame.Cards.DataModel.Effects;
using Booble.CardGame.Managers;
using System.Collections.Generic;
using UnityEngine;
using static Booble.CardGame.Managers.TurnManager;

namespace Booble.CardGame.Level
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

        public void InitializeBackground(Sprite backgroundSprite)
        {
            background.sprite = backgroundSprite;
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
                case Turn.INTERVIEW_START:
                    playerDeck.DrawCards(cardNumber);
                    opponentDeck.DrawCards(cardNumber);
                    break;
            }
        }

        public bool EmptyHands()
        {
            return (playerHand.numCards == 0 || playerHand.HasAlternateWinConditionCard())
                && (opponentHand.numCards == 0 || opponentHand.HasAlternateWinConditionCard());
        }

        #region Getters

        public CardZone GetEmptyCardZone(Contender contender)
        {
            int tries = 10;

            List<CardZone> cardZones = contender.cardZones;
            for (int i = 0; i < tries; i++)
            {
                int index = Random.Range(0, cardZones.Count);
                CardZone cardZone = cardZones[index];
                if (cardZone.isEmpty) return cardZone;
            }

            foreach (CardZone cardZone in cardZones)
            {
                if (cardZone.isEmpty) return cardZone;
            }

            return null;
        }

        public List<Card> GetCardsOnTable(Contender contender)
        {
            List<CardZone> cardZone = contender.cardZones;
            List<Card> temp = new List<Card>();
            foreach (CardZone zone in cardZone)
            {
                if (zone.isNotEmpty)
                    temp.Add(zone.GetCard());
            }
            return temp;
        }

        public bool AreCardsOnTable(Contender contender)
        {
            return NumCardsOnTable(contender) > 0;
        }

        public int NumCardsOnTable(Contender contender)
        {
            List<CardZone> cardZone = contender.cardZones;
            int num = 0;
            foreach (CardZone zone in cardZone)
            {
                if (zone.numCards > 0) num++;
            }
            return num;
        }

        public int GetPositionFromCard(Card card)
        {
            List<CardZone> cardZones = card.contender.cardZones;

            for (int i = 0; i < cardZones.Count; i++)
            {
                CardZone cardZone = cardZones[i];
                if (cardZone.GetCard() == card) return i;
            }

            return -1;
        }

        public Card GetOppositeCard(Card card)
        {
            int position = GetPositionFromCard(card);
            if (position == -1) return null;

            Contender otherContender = CardGameManager.Instance.GetOtherContender(card.contender);
            CardZone oppositeCardZone = otherContender.cardZones[position];
            return oppositeCardZone.GetCard();
        }

        #endregion

        public bool HasCard(Contender contender, string name)
        {
            foreach (CardZone cardZone in contender.cardZones)
            {
                if (cardZone.isNotEmpty)
                {
                    if (cardZone.GetCard().name.Equals(name)) return true;
                }
            }

            return false;
        }

        #region Highlight

        public void RemoveTargetsHighlight()
        {
            HighlightTargets(new List<Card>());
        }

        public void HighlightTargets(List<Card> possibleTargets)
        {
            List<CardZone> temp = new List<CardZone>();
            temp.AddRange(playerCardZone);
            temp.Add(playerFieldCardZone);
            temp.AddRange(opponentCardZone);
            temp.Add(opponentFieldCardZone);

            foreach (CardZone cardZone in temp)
            {
                if (cardZone.isNotEmpty)
                {
                    Card card = cardZone.GetCard();
                    card.ShowHighlight(possibleTargets.Contains(card));
                }
            }
        }

        public void HighlightMultipleTargets(Contender contender, SubType subType, Target targetType)
        {
            List<CardZone> cardZones = new List<CardZone>();
            bool addFieldCard = subType == SubType.DESTROY_CARD || subType == SubType.RETURN_CARD;

            switch (targetType)
            {
                case Target.AALLY:
                    cardZones.AddRange(contender.cardZones);
                    if (addFieldCard) cardZones.Add(contender.fieldCardZone);
                    break;
                case Target.AENEMY:
                    Contender otherContender = CardGameManager.Instance.GetOtherContender(contender);
                    cardZones.AddRange(otherContender.cardZones);
                    if (addFieldCard) cardZones.Add(otherContender.fieldCardZone);
                    break;
                case Target.ACARD:
                    cardZones.AddRange(playerCardZone);
                    cardZones.AddRange(opponentCardZone);

                    if (addFieldCard)
                    {
                        cardZones.Add(playerFieldCardZone);
                        cardZones.Add(opponentFieldCardZone);
                    }

                    break;
            }

            foreach (CardZone cardZone in cardZones)
            {
                if (cardZone.isNotEmpty)
                {
                    Card card = cardZone.GetCard();
                    card.ShowHighlight(true);
                }
            }
        }

        public void RemoveCardZonesHighlight(Card card)
        {
            HighlightCardZones(card, false);
        }

        public void HighlightCardZones(Card card, bool show)
        {
            switch (card.Stats.type)
            {
                case CardType.ARGUMENT:
                    {
                        List<CardZone> cardZones = card.contender.cardZones;
                        foreach (CardZone zone in cardZones)
                        {
                            if (!show || zone.isEmpty) zone.ShowHighlight(show);
                        }
                        break;
                    }
                case CardType.FIELD:
                    {
                        CardZone zone = card.contender.fieldCardZone;
                        zone.ShowHighlight(show);
                        break;
                    }
            }
        }

        #endregion

    }
}