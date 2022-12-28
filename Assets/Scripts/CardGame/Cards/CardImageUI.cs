using Booble.CardGame.Cards.DataModel;
using Booble.CardGame.Cards.DataModel.Effects;
using Booble.CardGame.Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Booble.CardGame.Cards
{
    public class CardImageUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descText;
        [SerializeField] private TextMeshProUGUI strengthText;
        [SerializeField] private TextMeshProUGUI defenseText;

        private CardType _type;
        private List<CardEffect> _effects;
        private int _index;

        private bool _selected;

        public bool clickable { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public void Initialize(CardsData data, int index)
        {
            GetComponent<Image>().sprite = data.sprite;

            nameText.text = data.name;

            _type = data.type;
            if (_type == CardType.ARGUMENT)
            {
                strengthText.text = data.strength.ToString();
                defenseText.text = data.defense.ToString();
            }

            _effects = data.effects;
            descText.text = GetDescriptionText();

            _index = index;
        }

        public void OnMouseLeftClickUp()
        {
            if (_selected)
            {
                GetComponent<Image>().color = Color.white;
                UIManager.Instance.RemoveStolenCard(_index);
            }
            else
            {
                GetComponent<Image>().color = Color.grey;
                UIManager.Instance.AddStolenCard(_index);
            }

            _selected = !_selected;
        }

        public void OnMouseHoverEnter()
        {
            UIManager.Instance.ShowExtendedDescription(NameToString(), TypeToString(), DescriptionToString());
        }

        public void OnMouseHoverExit()
        {
            UIManager.Instance.HideExtendedDescription();
        }

        private string GetDescriptionText()
        {
            string temp = "";

            foreach (CardEffect effect in _effects)
            {
                temp += effect.ToString() + "\n";
            }

            return temp;
        }

        public string NameToString()
        {
            return name.ToUpper();
        }

        public string TypeToString()
        {
            return "TIPO: " + _type.ToString();
        }

        public string DescriptionToString()
        {
            string s = "";
            foreach (CardEffect effect in _effects)
            {
                s += effect.ToStringExtended(_type) + "\n";
            }

            return s;
        }
    }
}