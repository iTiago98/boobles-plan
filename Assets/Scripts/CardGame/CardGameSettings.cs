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
        public int initialLife;
        public int initialManaCounter;
        public int maxManaCounter;
        public int handCapacity;

        [Header("Hover card")]
        public float hoverPosY;
        public float hoverScale;

        [Header("Move card with mouse")]
        public float movePositionZ;
        public float moveScale;

        [Header("Highlight")]
        public float highlightScale;

        [Header("Default")]
        public float defaultScale;

        [Header("Hit animation")]
        public float hitScale;
    }
}