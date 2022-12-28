using System;
using UnityEngine;

namespace Booble.CardGame.Cards.DataModel
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