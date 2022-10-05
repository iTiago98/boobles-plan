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

        public abstract void Play();

        protected void PlayCard(Card card, CardZone cardZone)
        {
            card.RemoveFromContainer(); // Remove from hand
            card.Play(cardZone);
        }

        //protected void PlayArgument(Card card, CardZone emptyCardZone)
        //{
        //    card.RemoveFromContainer();
        //    card.Play(emptyCardZone);
        //}

        //protected void PlayAction(Card card)
        //{
        //    card.RemoveFromContainer();
        //    card.Play(null);
        //}

        //protected void PlayField(Card card, CardZone fieldCardZone)
        //{
        //    card.RemoveFromContainer();
        //    card.Play(fieldCardZone);
        //}

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

        protected CardZone FieldCardZone()
        {
            return (_contender.fieldCardZone.isEmpty) ? _contender.fieldCardZone : null;
        }

        protected void SkipTurn()
        {
            TurnManager.Instance.FinishTurn();
        }
    }
}