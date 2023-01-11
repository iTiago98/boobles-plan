using Booble.CardGame.Cards.DataModel;
using Booble.CardGame.Cards.DataModel.Effects;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Booble.UI
{
    public class CardUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI strengthText;
        [SerializeField] private TextMeshProUGUI defenseText;

        private CardsData _data;
        private bool _initialized;

        public void Initialize(CardsData data)
        {
            _data = data;
            _initialized = true;

            nameText.text = data.name;
            descriptionText.text = GetDescriptionText(data);

            if (data.type == CardType.ARGUMENT)
            {
                strengthText.text = data.strength.ToString();
                defenseText.text = data.defense.ToString();
            }

            GetComponent<Image>().sprite = data.sprite;
        }

        private string GetDescriptionText(CardsData data)
        {
            string temp = "";

            foreach (CardEffect effect in data.effects)
            {
                temp += effect.ToString() + "\n";
            }

            return temp;
        }

        public void OnPointerEnter()
        {
            if(_initialized) PauseMenu.Instance.ShowExtendedDescription(_data);
        }

        public void OnPointerExit()
        {
            if (_initialized) PauseMenu.Instance.HideExtendedDescription();
        }
    }
}