using Booble.CardGame.Cards.DataModel;
using Booble.CardGame.Cards.DataModel.Effects;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Booble.UI
{
    public class ExtendedDescriptionPanel : MonoBehaviour
    {
        [Header("Card type images")]
        [SerializeField] private Image _typeImage;
        [SerializeField] private Sprite _argumentTypeSprite;
        [SerializeField] private Sprite _actionTypeSprite;
        [SerializeField] private Sprite _fieldTypeSprite;

        [SerializeField] private float _posYEffect;
        [SerializeField] private float _posYNoEffect;

        [Header("Text fields")]
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _typeText;
        [SerializeField] private TextMeshProUGUI _descText;

        [Header("Panels")]
        [SerializeField] private GameObject _descPanel;

        //public void SetText(string name, string type, string desc)
        //{
        //    _nameText.text = name;
        //    _typeText.text = type;
        //    _descText.text = desc;
        //}

        public void Show(CardsData data)
        {
            _nameText.text = data.GetNameText();
            _typeText.text = data.GetTypeText();

            float posY;
            if (data.effects.Count > 0)
            {
                _descPanel.SetActive(true);
                _descText.text = data.GetExtendedDescriptionText();
                posY = _posYEffect;
            }
            else
            {
                _descPanel.SetActive(false);
                posY = _posYNoEffect;
            }

            _typeImage.rectTransform.anchoredPosition = new Vector2(_typeImage.rectTransform.anchoredPosition.x, posY);

            switch (data.type)
            {
                case CardType.ARGUMENT:
                    _typeImage.sprite = _argumentTypeSprite;
                    break;
                case CardType.ACTION:
                    _typeImage.sprite = _actionTypeSprite;
                    break;
                case CardType.FIELD:
                    _typeImage.sprite = _fieldTypeSprite;
                    break;
            }

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            _nameText.text = "";
            _typeText.text = "";
            _descText.text = "";
            gameObject.SetActive(false);
        }
    }
}