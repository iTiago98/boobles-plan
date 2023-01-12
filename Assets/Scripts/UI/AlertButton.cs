using Booble.CardGame.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Booble.UI
{
    public class AlertButton : MyButton
    {
        [SerializeField] private GameObject _newCardAlert;

        public void ShowAlert(bool value)
        {
            _newCardAlert.SetActive(value);
        }

        public void SetImage(Sprite sprite)
        {
            _button.image.sprite = sprite;
        }
    }
}