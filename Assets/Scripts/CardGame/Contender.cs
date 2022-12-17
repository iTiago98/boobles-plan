using CardGame.AI;
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
        #region Role 

        public enum Role
        {
            PLAYER, OPPONENT
        }

        public Role role;
        public bool isPlayer => role == Role.PLAYER;

        #endregion

        #region Stats

        public int life { private set; get; }
        public int currentMana { private set; get; }
        public int currentMaxMana { private set; get; }
        public int extraMana { private set; get; }
        public bool freeMana { private set; get; }
        private int _maxMana;

        #endregion

        [SerializeField] private CardsDataContainer _deckCards;
        [SerializeField] private Sprite _cardBack;

        [Header("Opponent settings")]
        [SerializeField] private Sprite _interviewBackground;
        [SerializeField] private InterviewDialogue _interviewDialogue;
        [SerializeField] private OpponentAI _aIScript;

        public Hand hand { private set; get; }
        public Deck deck { private set; get; }
        public List<CardZone> cardZones { private set; get; }
        public CardZone fieldCardZone { private set; get; }

        [HideInInspector]
        public int stolenCards;

        public void Initialize(Hand hand, Deck deck, List<CardZone> cardZones, CardZone fieldCardZone)
        {
            this.hand = hand;
            hand.contender = this;
            this.deck = deck;
            this.cardZones = cardZones;
            this.fieldCardZone = fieldCardZone;
        }

        public void InitializeStats(int initialEloquence, int initialManaCounter, int maxManaCounter)
        {
            life = initialEloquence;
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
        }

        public void IncreaseMaxMana(int mana)
        {
            _maxMana += mana;
            extraMana += mana;
            UIManager.Instance.UpdateMaxMana(this, _maxMana);
            currentMaxMana += mana;
        }

        public void SubstractMana(int cost, bool continueFlow)
        {
            currentMana -= cost;
            if (currentMana < 0) currentMana = 0;
            UIManager.Instance.UpdateUIStats(continueFlow);
        }

        public void RestoreLife(int life, bool continueFlow)
        {
            this.life += life;
            UIManager.Instance.UpdateUIStats(continueFlow);
        }

        public void ReceiveDamage(int strength)
        {
            life -= strength;
            //if (eloquence < 0) eloquence = 0;
            UIManager.Instance.UpdateUIStats();
            TurnManager.Instance.CheckEndMidTurn();
        }

        public void SetFreeMana(bool state)
        {
            freeMana = state;
            // Show in UI
        }

        public CardsDataContainer GetDeckCards() { return _deckCards; }
        public Sprite GetCardBack() { return _cardBack; }
        public InterviewDialogue GetInterviewDialogue() { return _interviewDialogue; }
        public Sprite GetInterviewBackground() { return _interviewBackground; }
        public OpponentAI GetAIScript() { return _aIScript; }
    }
}