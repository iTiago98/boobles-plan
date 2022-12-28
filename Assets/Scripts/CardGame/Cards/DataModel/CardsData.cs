using Booble.CardGame.Cards.DataModel.Effects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Booble.CardGame.Cards.DataModel
{
    [Serializable]
    public class CardsData
    {
        public string name;
        public Sprite sprite;

        public int cost;
        public int strength;
        public int defense;

        public int defaultStrength { private set; get; }
        public int defaultDefense { private set; get; }

        public CardType type;

        public List<CardEffect> effects = new List<CardEffect>();

        public CardsData()
        {

        }

        public CardsData(CardsData data)
        {
            this.name = data.name;
            this.sprite = data.sprite;
            this.cost = data.cost;
            this.strength = data.strength;
            this.defense = data.defense;
            this.defaultStrength = data.strength;
            this.defaultDefense = data.defense;
            this.type = data.type;

            this.effects = new List<CardEffect>();
            foreach (CardEffect effect in data.effects)
            {
                this.effects.Add(effect);
            }
        }

        public void AddEffect(CardEffect effect)
        {
            effects.Add(effect);
        }
    }
}