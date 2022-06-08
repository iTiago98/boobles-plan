using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame.Cards.DataModel
{
    [CreateAssetMenu(fileName = "CardsData", menuName = "DataObjects/CardsData")]
    public class CardsDataContainer : ScriptableObject
    {
        public string resourcesPath;
        public List<CardsData> cards;

    }
}