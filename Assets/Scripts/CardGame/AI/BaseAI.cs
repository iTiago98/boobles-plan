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
    public abstract class BaseAI : MonoBehaviour
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
                            if (card.type == CardType.FIELD) return card;
                            else
                            {
                                int temp = card.strength + card.defense;
                                if (temp > bestStats)
                                {
                                    bestTarget = card;
                                    bestStats = temp;
                                }
                            }
                        }
                        break;

                    case SubType.DEAL_DAMAGE:
                        {
                            if (card.defense <= effect.intParameter1 && card.strength > bestStats)
                            {
                                bestTarget = card;
                                bestStats = card.strength;
                            }
                        }
                        break;

                    case SubType.RETURN_CARD:
                        {
                            int temp = card.strength + card.defense;
                            if (card.IsBoosted()) temp += card.GetBoost();
                            if (card.IsDamaged()) temp -= card.GetDamage();

                            if (temp > bestStats)
                            {
                                bestTarget = card;
                                bestStats = temp;
                            }
                        }
                        break;

                    case SubType.DUPLICATE_CARD:
                        {
                            int temp = card.strength + card.defense;
                            if (temp > bestStats)
                            {
                                bestTarget = card;
                                bestStats = temp;
                            }
                        }
                        break;

                    case SubType.SWAP_POSITION:
                        {
                            int position = Board.Instance.GetPositionFromCard(card);

                            Contender otherContender = CardGameManager.Instance.GetOtherContender(card.contender);
                            CardZone oppositeCardZone = Board.Instance.GetCardZoneFromPosition(position, otherContender);
                            Card oppositeCard = oppositeCardZone.GetCard();

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
                        break;

                    case SubType.STAT_BOOST:
                        //TODO
                        break;
                }
            }

            return bestTarget;
        }

        protected abstract bool IsGoodChoice(CardEffect effect);
    }
}