using CardGame.Cards.DataModel.Effects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame.Cards.DataModel
{
    public class CardsDataNoSer
    {
        public string name;
        public Sprite sprite;

        public int cost;
        public int strength;
        public int defense;

        public CardType type;

        public CardsDataNoSer()
        {

        }

        public CardsDataNoSer(CardsDataNoSer data)
        {
            this.name = data.name;
            this.sprite = data.sprite;
            this.cost = data.cost;
            this.strength = data.strength;
            this.defense = data.defense;
            this.type = data.type;
        }

        public CardsData ToCardsData()
        {
            return new CardsData
            {
                name = this.name,
                sprite = this.sprite,
                cost = this.cost,
                strength = this.strength,
                defense = this.defense,
                type = this.type
            };
        }
    }
}