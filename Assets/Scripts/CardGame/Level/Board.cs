using CardGame.Cards;
using CardGame.Cards.DataModel;
using CardGame.Cards.DataModel.Effects;
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

        public void WheelEffect()
        {
            StartCoroutine(WheelCoroutine());
        }

        private IEnumerator WheelCoroutine()
        {
            TurnManager.Instance.StopFlow();

            Contender player = CardGameManager.Instance.player;
            Contender opponent = CardGameManager.Instance.opponent;

            int playerNumCards = player.hand.numCards;
            int opponentNumCards = opponent.hand.numCards;

            player.hand.DiscardAll();
            opponent.hand.DiscardAll();

            yield return new WaitUntil(() => EmptyHands());

            Debug.Log(playerNumCards);
            Debug.Log(opponentNumCards);

            TurnManager.Instance.StopFlow();

            DrawCards(playerNumCards, Turn.PLAYER);
            DrawCards(opponentNumCards, Turn.OPPONENT);

            yield return new WaitUntil(() => TurnManager.Instance.continueFlow);
        }

        private bool EmptyHands()
        {
            return (playerHand.numCards == 0 || playerHand.HasAlternateWinConditionCard())
                && (opponentHand.numCards == 0 || opponentHand.HasAlternateWinConditionCard());
        }

        #region Getters

        public CardZone GetEmptyCardZone(Contender contender)
        {
            List<CardZone> cardZone = contender.cardZones;
            foreach (CardZone zone in cardZone)
            {
                if (zone.numCards == 0) return zone;
            }

            return null;
        }

        public List<Card> GetCardsOnTable(Contender contender)
        {
            List<CardZone> cardZone = contender.cardZones;
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
                if (cardZone.GetCard()?.name == card.name) return i;
            }

            return -1;
        }

        public Card GetOppositeCard(Card card)
        {
            int position = GetPositionFromCard(card);
            Contender otherContender = CardGameManager.Instance.GetOtherContender(card.contender);
            CardZone oppositeCardZone = otherContender.cardZones[position];
            return oppositeCardZone.GetCard();
        }

        #endregion

        #region Destroy

        public void DestroyCards(Contender contender)
        {
            foreach (CardZone zone in contender.cardZones) zone.GetCard()?.Destroy();
            Card fieldCard = contender.fieldCardZone.GetCard();
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
                    if(addFieldCard) cardZones.Add(otherContender.fieldCardZone);
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
                Card card = cardZone.GetCard();
                if (card != null)
                {
                    card.ShowHighlight(true);
                }
            }
        }

        public void HighlightZoneTargets(CardType type, Contender contender, bool show)
        {
            switch (type)
            {
                case CardType.ARGUMENT:
                    {
                        List<CardZone> cardZones = contender.cardZones;
                        foreach (CardZone zone in cardZones)
                        {
                            if (!show || zone.isEmpty) zone.ShowHighlight(show);
                        }
                        break;
                    }
                case CardType.FIELD:
                    {
                        CardZone zone = contender.fieldCardZone;
                        zone.ShowHighlight(show);
                        break;
                    }
            }
        }

        #endregion

    }
}