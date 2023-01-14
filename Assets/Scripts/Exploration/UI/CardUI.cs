using Booble.CardGame.Cards.DataModel;
using Booble.CardGame.Cards.DataModel.Effects;
using Booble.Managers;
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

        [SerializeField] private AlertImage alertImage;

        private CardsData _data;
        private Image _imageComponent;
        private bool _front;

        public string GetName() => _data.name;

        public void Initialize(CardsData data, Sprite cardBack)
        {
            _data = data;

            _imageComponent = GetComponent<Image>();
            if (DeckManager.Instance.PlayerHasCard(data))
            {
                SetFront();
            }
            else
            {
                SetBack(cardBack);
            }
        }

        public void SetFront()
        {
            nameText.text = _data.name;
            descriptionText.text = _data.GetDescriptionText();

            if (_data.type == CardType.ARGUMENT)
            {
                strengthText.text = _data.strength.ToString();
                defenseText.text = _data.defense.ToString();
            }

            _imageComponent.sprite = _data.sprite;
            _imageComponent.color = Color.white;
            _front = true;
        }

        private void SetBack(Sprite cardBack)
        {
            nameText.text = "";
            descriptionText.text = "";

            if (_data.type == CardType.ARGUMENT)
            {
                strengthText.text = "";
                defenseText.text = "";
            }

            _imageComponent.sprite = cardBack;
            _imageComponent.color = Color.white;
            _front = false;
        }

        public void OnPointerEnter()
        {
            if (_front)
            {
                PauseMenu.Instance.ShowExtendedDescription(_data);
                if (alertImage.gameObject.activeSelf)
                {
                    ShowAlertImage(false);
                    DeckManager.Instance.RemoveNewCard(_data);
                }
            }
        }

        public void OnPointerExit()
        {
            if (_front) PauseMenu.Instance.HideExtendedDescription();
        }

        #region Alert

        public void ShowAlertImage(bool value)
        {
            alertImage.gameObject.SetActive(value);
        }

        #endregion
    }
}