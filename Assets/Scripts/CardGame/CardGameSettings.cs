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
        [Header("Stats")]
        public int initialCardNumber;
        public int initialEloquence;
        public int initialManaCounter;
        public int maxManaCounter;
        public int handCapacity;

        [Header("Hover card")]
        public float hoverPosY;
        public float hoverScale;

        [Header("Move card with mouse")]
        public float movePositionZ;
        public float defaultScale;
        public float moveScale;

        [Header("Hit animation")]
        public float hitScale;
    }
}