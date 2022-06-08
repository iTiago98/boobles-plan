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

        //[Header("Close Up")]
        //public Transform closeUpPosition;
        //public float closeUpScaleMultiplier;
        //public float closeUpTime;

        public Hand hand { private set; get; }
        public List<CardZone> cardZones { private set; get; }

        public void Initialize(Hand hand, List<CardZone> cardZone)
        {
            this.hand = hand;
            this.cardZones = cardZone;
        }

        public void InitializeStats(int initialEloquence, int initialManaCounter)
        {
            eloquence = initialEloquence;
            currentMaxMana = initialManaCounter;
            currentMana = currentMaxMana;
        }

        public void FillMana()
        {
            if (currentMaxMana < TurnManager.Instance.settings.maxManaCounter)
            {
                currentMaxMana++;
            }
            currentMana = currentMaxMana;
        }

        public void MinusMana(int cost)
        {
            currentMana -= cost;
        }

        public void Hit(int strength)
        {
            eloquence -= strength;
        }
    }
}