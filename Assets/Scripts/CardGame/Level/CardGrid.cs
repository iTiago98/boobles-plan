using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Booble.CardGame.Level
{
    public class CardGrid : CardContainer
    {
        [SerializeField] private int _cardsPerRow;
        [SerializeField] private float _cardSeparationY;

        protected override float GetXPosition(int index, float initialPosition)
        {
            return initialPosition + cardSeparationX * (index % _cardsPerRow);
        }

        protected override float GetYPosition(int index)
        {
            return -(index / _cardsPerRow * _cardSeparationY);
        }

        protected override float CalculateInitialPosition()
        {
            return -((float)_cardsPerRow - 1) / 2 * cardSeparationX;
        }
    }
}