using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Booble.CardGame.Cards.DataModel
{
    [CreateAssetMenu(fileName = "CardsDataContainer", menuName = "DataObjects/CardsDataContainer")]
    public class CardsDataContainer : ScriptableObject
    {
        public string resourcesPath;
        public List<CardsData> cards;
    }
}