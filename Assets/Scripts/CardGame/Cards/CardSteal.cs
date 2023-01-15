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

        private CardsData _data;
        private int _index;

        private bool _selected;

        private SpriteRenderer _spriteRenderer;

        public void Initialize(CardsData data, int index)
        {
            _data = data;

            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteRenderer.sprite = data.sprite;
            SetColor(false);

            nameText.text = data.name;

            if (data.type == CardType.ARGUMENT)
            {
                strengthText.text = data.strength.ToString();
                defenseText.text = data.defense.ToString();
            }

            descText.text = data.GetDescriptionText();

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
            UIManager.Instance.ShowExtendedDescription(_data);
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
    }
}