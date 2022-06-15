using CardGame.Cards.DataModel;
using CardGame.Level;
using CardGame.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame
{
    public class Contender : MonoBehaviour
    {
        public enum Role
        {
            PLAYER, OPPONENT
        }

        public Role role;

        public CardsDataContainer deckCards;

        public int eloquence { private set; get; }
        public int currentMana { private set; get; }
        public int currentMaxMana { private set; get; }
        private int _maxMana;

        public Hand hand { private set; get; }
        public List<CardZone> cardZones { private set; get; }


        public void Initialize(Hand hand, List<CardZone> cardZone)
        {
            this.hand = hand;
            this.cardZones = cardZone;
        }

        public void InitializeStats(int initialEloquence, int initialManaCounter, int maxManaCounter)
        {
            eloquence = initialEloquence;
            currentMaxMana = initialManaCounter;
            currentMana = currentMaxMana;
            _maxMana = maxManaCounter;
        }

        public void FillMana()
        {
            if (currentMaxMana < _maxMana)
            {
                currentMaxMana++;
            }
            currentMana = currentMaxMana;
            //Update ui
        }

        public void IncreaseMaxMana(int mana)
        {
            _maxMana += mana;
        }

        public void MinusMana(int cost)
        {
            currentMana -= cost;
            //Update ui
        }

        public void RestoreLife(int life)
        {
            eloquence += life;
            //Update ui
        }

        public void ReceiveDamage(int strength)
        {
            eloquence -= strength;
            if (eloquence < 0) eloquence = 0;
            //Update ui
        }
    }
}