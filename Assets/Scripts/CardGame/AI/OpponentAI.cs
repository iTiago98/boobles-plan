using CardGame.Cards;
using CardGame.Cards.DataModel;
using CardGame.Cards.DataModel.Effects;
using CardGame.Level;
using CardGame.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame.AI
{
    public class OpponentAI : MonoBehaviour
    {
        protected Contender _contender;

        [SerializeField] private float _waitTime;
        private float _timer;

        public void Initialize(Contender contender)
        {
            _contender = contender;
        }

        private void Update()
        {
            if (_contender == null) return;

            _timer += Time.deltaTime;
            if (_timer > _waitTime)
            {
                _timer = 0;
                Play();
            }
        }

        private void Play()
        {
            if (_contender.currentMana > 0 || _contender.freeMana)
            {
                List<Card> playableCards = new List<Card>();
                CardZone emptyCardZone = GetRandomEmptyCardZone();
                CardZone fieldCardZone = GetFieldCardZone();

                foreach (GameObject cardObj in _contender.hand.cards)
                {
                    Card card = cardObj.GetComponent<Card>();

                    if (card.manaCost > _contender.currentMana && !_contender.freeMana) continue;

                    switch (card.type)
                    {
                        case CardType.ARGUMENT:
                            if (emptyCardZone) playableCards.Add(card);
                            break;
                        case CardType.ACTION:
                            if (card.hasEffect && card.effect.IsAppliable() && IsGoodChoice(card.effect))
                                playableCards.Add(card);
                            break;
                        case CardType.FIELD:
                            if (fieldCardZone) playableCards.Add(card);
                            break;
                    }
                }

                if (playableCards.Count == 0) SkipTurn();
                else
                {
                    int index = new System.Random().Next(0, playableCards.Count);
                    Card card = playableCards[index];
                    CardZone cardZone = null;

                    switch (card.type)
                    {
                        case CardType.ARGUMENT:
                            cardZone = emptyCardZone;
                            break;
                        case CardType.ACTION:
                            break;
                        case CardType.FIELD:
                            cardZone = fieldCardZone;
                            break;
                    }

                    PlayCard(card, cardZone);
                }
            }
            else
            {
                SkipTurn();
            }
        }

        private void PlayCard(Card card, CardZone cardZone)
        {
            card.RemoveFromContainer(); // Remove from hand
            card.Play(cardZone);
        }

        private CardZone GetRandomEmptyCardZone()
        {
            List<CardZone> cardZones = _contender.cardZones;
            int tries = 10;
            System.Random random = new System.Random();
            for (int i = 0; i < tries; i++)
            {
                int index = random.Next(0, cardZones.Count);
                if (cardZones[index].GetCard() == null) return cardZones[index];
            }

            foreach (CardZone cardZone in cardZones)
            {
                if (cardZone.GetCard() == null) return cardZone;
            }

            return null;
        }

        private CardZone GetFieldCardZone()
        {
            return (_contender.fieldCardZone.isEmpty) ? _contender.fieldCardZone : null;
        }

        private void SkipTurn()
        {
            TurnManager.Instance.FinishTurn();
        }

        public Card GetBestTarget(CardEffect effect, List<Card> possibleTargets)
        {
            Card bestTarget = possibleTargets[new System.Random().Next(0, possibleTargets.Count)];

            int bestStats = 0;
            foreach (Card card in possibleTargets)
            {
                switch (effect.subType)
                {
                    case SubType.DESTROY_CARD:
                        {
                            if (card.type == CardType.FIELD)
                                return card;
                            else
                                GetBestStats(card, ref bestTarget, ref bestStats);
                        }
                        break;

                    case SubType.DEAL_DAMAGE:
                        {
                            Card oppositeCard = Board.Instance.GetOppositeCard(card);

                            if (card != null)
                            {
                                int temp = GetStats(card);
                                int damage = effect.intParameter1;
                                if (oppositeCard != null) damage += oppositeCard.strength;

                                if ((card.defense <= damage) && (temp > bestStats))
                                {
                                    bestStats = temp;
                                    bestTarget = card;
                                };
                            }
                        }
                        break;

                    case SubType.RETURN_CARD:
                        {
                            int temp = 0;
                            if (card.IsBoosted()) temp += card.GetBoost();
                            if (card.IsDamaged()) temp -= card.GetDamage();

                            GetBestStats(card, temp, ref bestTarget, ref bestStats);
                        }
                        break;

                    case SubType.DUPLICATE_CARD:
                        GetBestStats(card, ref bestTarget, ref bestStats); break;

                    case SubType.SWAP_POSITION:
                        GetSwapPositionTarget(card, ref bestTarget, ref bestStats); break;

                    case SubType.STAT_BOOST:
                        {
                            Card oppositeCard = Board.Instance.GetOppositeCard(card);

                            if (oppositeCard != null)
                            {
                                bool savedCard = card.defense <= oppositeCard.strength && (card.defense + effect.intParameter2) > oppositeCard.strength;
                                bool killedCard = card.strength < oppositeCard.defense && (card.strength + effect.intParameter1) >= oppositeCard.defense;

                                int cardStats = GetStats(card);
                                int oppositeCardStats = GetStats(oppositeCard);

                                if (killedCard && oppositeCardStats > bestStats)
                                {
                                    bestTarget = card;
                                    bestStats = oppositeCardStats;
                                }
                                else if (savedCard && cardStats > bestStats)
                                {
                                    bestTarget = card;
                                    bestStats = cardStats;
                                }
                            }
                        }
                        break;
                }
            }

            return bestTarget;
        }

        private bool IsGoodChoice(CardEffect effect)
        {
            Contender player = CardGameManager.Instance.player;
            Contender opponent = CardGameManager.Instance.opponent;

            List<CardZone> playerCardZones = Board.Instance.GetCardZones(player);
            List<CardZone> opponentCardZones = Board.Instance.GetCardZones(opponent);

            switch (effect.subType)
            {
                case SubType.RESTORE_LIFE:
                    return true;

                case SubType.DESTROY_CARD:
                    if (effect.targetType == Target.CARD)
                    {
                        if (Board.Instance.GetFieldCardZone(player).GetCard() != null) return true;

                        return Board.Instance.NumCardsOnTable(player) > Board.Instance.NumCardsOnTable(opponent);
                    }
                    else if (effect.targetType == Target.AENEMY)
                    {
                        return Board.Instance.NumCardsOnTable(player) > Board.Instance.NumCardsOnTable(opponent) 
                            && Board.Instance.NumCardsOnTable(player) > 2;
                    }
                    else if (effect.targetType == Target.ACARD)
                    {
                        return GetStatsSummary(player, playerCardZones, opponent, opponentCardZones) > 0;
                    }
                    break;

                case SubType.DEAL_DAMAGE:
                    if (effect.targetType == Target.ENEMY)
                    {
                        foreach (CardZone cardZone in playerCardZones)
                        {
                            Card playerCard = cardZone.GetCard();

                            if (playerCard != null)
                            {
                                Card oppositeCard = Board.Instance.GetOppositeCard(playerCard);
                                int temp = effect.intParameter1;
                                if (oppositeCard != null) temp += oppositeCard.strength;
                                if (playerCard.strength <= temp) return true;
                            }
                        }
                        return false;
                    }
                    else if (effect.targetType == Target.PLAYER) return true;
                    break;

                case SubType.DECREASE_MANA:
                    return (Board.Instance.NumCardsOnTable(opponent) - Board.Instance.NumCardsOnTable(player)) >= 2;

                case SubType.STAT_BOOST:
                    int statsSummary = GetStatsSummary(player, playerCardZones, opponent, opponentCardZones);
                    if (effect.targetType == Target.ALLY) return statsSummary > 0;
                    else if (effect.targetType == Target.AALLY)
                        return (statsSummary < 0) && Board.Instance.NumCardsOnTable(opponent) >= 2;
                    break;

                case SubType.DUPLICATE_CARD:
                    int bestStats = 0;
                    Card bestTarget = null;
                    foreach(CardZone cardZone in opponentCardZones)
                    {
                        Card card = cardZone.GetCard();
                        if (card != null) GetBestStats(card, ref bestTarget, ref bestStats);
                    }

                    return bestStats > 5;

                case SubType.SWAP_POSITION:
                    return Board.Instance.NumCardsOnTable(player) < playerCardZones.Count
                        && Board.Instance.NumCardsOnTable(opponent) < opponentCardZones.Count;

                case SubType.DRAW_CARD:
                    return Board.Instance.GetHand(opponent).numCards <= 3;

                case SubType.DISCARD_CARD:
                    return player.hand.numCards >= effect.intParameter1;

                case SubType.RETURN_CARD:
                    return Board.Instance.NumCardsOnTable(player) > Board.Instance.NumCardsOnTable(opponent);

                case SubType.SKIP_COMBAT:
                    return Board.Instance.NumCardsOnTable(player) > Board.Instance.NumCardsOnTable(opponent);

                default:
                    return true;
            }

            return true;
        }

        private int GetStatsSummary(Contender player, List<CardZone> playerCardZones, Contender opponent, List<CardZone> opponentCardZones)
        {
            int temp = 0;
            foreach (CardZone cardZone in playerCardZones) temp += GetStats(cardZone.GetCard());
            if (Board.Instance.GetFieldCardZone(player).GetCard() != null) temp += 5;

            foreach (CardZone cardZone in opponentCardZones) temp -= GetStats(cardZone.GetCard());
            if (Board.Instance.GetFieldCardZone(opponent).GetCard() != null) temp -= 5;

            return temp;
        }

        private int GetStats(Card card)
        {
            if (card != null) return card.strength + card.defense;
            else return 0;
        }

        private void GetSwapPositionTarget(Card card, ref Card bestTarget, ref int bestStats)
        {
            Card oppositeCard = Board.Instance.GetOppositeCard(card);

            int temp = card.strength;
            if (temp > bestStats)
            {
                if ((card.contender.isPlayer && oppositeCard == null)
                    || (!card.contender.isPlayer && oppositeCard != null))
                {
                    bestTarget = card;
                    bestStats = temp;
                }
            }
        }

        private void GetBestStats(Card card, ref Card bestTarget, ref int bestStats)
        {
            GetBestStats(card, 0, ref bestTarget, ref bestStats);
        }

        private void GetBestStats(Card card, int baseStats, ref Card bestTarget, ref int bestStats)
        {
            int temp = baseStats + GetStats(card);
            if (temp > bestStats)
            {
                bestTarget = card;
                bestStats = temp;
            }
        }
    }
}