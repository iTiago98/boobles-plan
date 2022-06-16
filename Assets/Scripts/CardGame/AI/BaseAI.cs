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
    public abstract class BaseAI : MonoBehaviour
    {
        protected Contender _contender;

        public void Initialize(Contender contender)
        {
            _contender = contender;
        }

        private void Update()
        {
            Play();
        }

        public abstract void Play();

        protected void PlayCard(Card card, CardZone emptyCardZone)
        {
            card.RemoveFromContainer();
            emptyCardZone.AddCard(card);
            _contender.MinusMana(card.manaCost);
            UIManager.Instance.UpdateUIStats();
        }

        protected CardZone RandomEmptyCardZone()
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

        protected void SkipTurn()
        {
            TurnManager.Instance.FinishTurn();
        }
    }
}