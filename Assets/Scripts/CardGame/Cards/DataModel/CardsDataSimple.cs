using CardGame.Cards.DataModel.Effects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame.Cards.DataModel
{
    [Serializable]
    public class CardsDataSimple
    {
        public string name;
        public Sprite sprite;
        public int strength;
        public int defense;
        public int manaCost;
    }
}