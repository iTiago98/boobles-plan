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
        [SerializeField] private Image _image;

        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _typeText;
        [SerializeField] private TextMeshProUGUI _descText;

        public void SetText(string name, string type, string desc)
        {
            _nameText.text = name;
            _typeText.text = type;
            _descText.text = desc;
        }

        public void Show(CardsData data)
        {
            _nameText.text = data.GetNameText();
            _typeText.text = data.GetTypeText();
            _descText.text = data.GetExtendedDescriptionText();
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