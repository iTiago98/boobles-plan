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
    public class CardSteal : MonoBehaviour, IClickable
    {
        [SerializeField] private TextMeshPro nameText;
        [SerializeField] private TextMeshPro descText;
        [SerializeField] private TextMeshPro strengthText;
        [SerializeField] private TextMeshPro defenseText;

        [SerializeField] private Color _selectedColor;
        [SerializeField] private Color _selectedHoverColor;
        [SerializeField] private Color _unselectedColor;
        [SerializeField] private Color _unselectedHoverColor;


        private CardType _type;
        private List<CardEffect> _effects;
        private int _index;

        private bool _selected;

        private SpriteRenderer _spriteRenderer;

        public void Initialize(CardsData data, int index)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = data.sprite;
            SetColor(false);

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

        public void OnMouseLeftClickUp(MouseController mouseController)
        {
            if (_selected)
            {
                _selected = CardEffectsManager.Instance.RemoveStolenCard(_index);
            }
            else
            {
                _selected = CardEffectsManager.Instance.AddStolenCard(_index);
            }

            SetColor(true);
        }

        public void OnMouseHoverEnter()
        {
            UIManager.Instance.ShowExtendedDescription(NameToString(), TypeToString(), DescriptionToString());
            SetColor(true);
        }

        public void OnMouseHoverExit()
        {
            UIManager.Instance.HideExtendedDescription();
            SetColor(false);
        }

        private void SetColor(bool enter)
        {
            _spriteRenderer.color = GetColor(enter);
        }

        private Color GetColor(bool enter)
        {
            if(enter) {
                if (_selected) return _selectedHoverColor;
                else return _unselectedHoverColor; 
            }
            else
            {
                if (_selected) return _selectedColor;
                else return _unselectedColor;
            }
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