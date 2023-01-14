using Booble.CardGame.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Booble.UI
{
    public class AlertButton : MyButton
    {
        [SerializeField] private AlertImage alertImage;

        public void ShowAlert(bool value)
        {
            alertImage.gameObject.SetActive(value);
        }

        public void SetImage(Sprite sprite)
        {
            _button.image.sprite = sprite;
        }
    }
}