using CardGame.Cards;
using CardGame.Cards.DataModel;
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
        private Contender _contender;

        public void Initialize(Contender contender)
        {
            _contender = contender;
        }

        private void Update()
        {
            Play();
        }

        private void Play()
        {
            CardZone emptyCardZone = EmptyCardZone();
            if (_contender.currentMana > 0 && emptyCardZone)
            {
                foreach (GameObject cardObj in _contender.hand.cards)
                {
                    Card card = cardObj.GetComponent<Card>();

                    if (card.manaCost <= _contender.currentMana && card.type == CardType.ARGUMENT)
                    {
                        PlayCard(card, emptyCardZone);
                        break;
                    }
                }
            }
            else
            {
                SkipTurn();
            }
        }

        private void PlayCard(Card card, CardZone emptyCardZone)
        {
            card.RemoveFromContainer();
            emptyCardZone.AddCard(card);
            _contender.MinusMana(card.manaCost);
            TurnManager.Instance.UpdateUIStats();
        }

        private CardZone EmptyCardZone()
        {
            foreach (CardZone cardZone in _contender.cardZones)
            {
                if (cardZone.GetCard() == null) return cardZone;
            }

            return null;
        }

        private void SkipTurn()
        {
            TurnManager.Instance.FinishTurn();
        }
    }
}