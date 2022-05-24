using System;
using System.Collections.Generic;
using UnityEngine;

namespace DataModel
{
    [Serializable]
    public class CardsData
    {
        public string name;
        public Sprite sprite;

        public int cost;
        public int strength;
        public int defense;

        public CardType type;

        public List<CardEffect> effects = new List<CardEffect>();
    } 
}