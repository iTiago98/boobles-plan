using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Booble.UI
{
    public class AlertButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        
        [SerializeField] private GameObject _newCardAlert;

        public void ShowAlert(bool value)
        {
            _newCardAlert.SetActive(value);
        }

        public void SetImage(Sprite sprite)
        {
            button.image.sprite = sprite;
        }

        public void SetInteractable(bool value)
        {
            button.interactable = value;
        }
    }
}