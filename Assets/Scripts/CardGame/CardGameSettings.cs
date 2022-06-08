using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CardGame
{
    [CreateAssetMenu(fileName = "CardGameSettings", menuName = "DataObjects/CardGameSettings")]
    public class CardGameSettings : ScriptableObject
    {
        public int initialCardNumber;
        public int initialEloquence;
        public int initialManaCounter;
        public int maxManaCounter;

        public float movePositionZ;
    }
}