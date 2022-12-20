using CardGame.Cards.DataModel;
using CardGame.Cards.DataModel.Effects;
using CardGame.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame.Cards
{
    public class CardStats : MonoBehaviour
    {
        public int manaCost => _card.data.cost;
        public int strength
        {
            private set { _card.data.strength = value; }
            get { return _card.data.strength; }
        }
        public int defense
        {
            private set { _card.data.defense = value; }
            get { return _card.data.defense; }
        }
        public int defaultStrength => _card.data.defaultStrength;
        public int defaultDefense => _card.data.defaultDefense;
        public CardType type => _card.data.type;

        private Card _card;

        public void Initialize(Card card)
        {
            _card = card;
        }

        public void SubstractMana()
        {
            _card.contender.SubstractMana(manaCost, continueFlow: false, useFreeMana: true);
        }

        public void ReceiveDamage(int strength)
        {
            defense -= strength;
            if (defense < 0) defense = 0;
        }

        public void BoostStats(int strengthBoost, int defenseBoost)
        {
            strength += strengthBoost;
            defense += defenseBoost;
        }

        public void DecreaseStats(int strengthDecrease, int defenseDecrease)
        {
            strength -= strengthDecrease;
            defense -= defenseDecrease;
            if (strength < 0) strength = 0;
        }

        public bool IsBoosted()
        {
            return strength > defaultStrength || defense > defaultDefense;
        }

        public int GetBoost()
        {
            int boost = 0;
            if (strength > defaultStrength) boost += strength - defaultStrength;
            if (defense > defaultDefense) boost += defense - defaultDefense;
            return boost;
        }

        public bool IsDamaged()
        {
            return defense < defaultDefense;
        }

        public int GetDamage()
        {
            return defaultDefense - defense;
        }

        public void ReturnToHand()
        {
            strength = defaultStrength;
            defense = defaultDefense;
        }

    }
}