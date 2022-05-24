using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataModel
{
    [CreateAssetMenu(fileName = "CardsData", menuName = "DataObjects/CardsData")]
    public class CardsDataContainer : ScriptableObject
    {
        public string resourcesPath;
        public List<CardsData> cards;

    }
}