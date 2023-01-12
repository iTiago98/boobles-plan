using Booble.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Booble.UI
{
    public class PauseButton : MonoBehaviour
    {
        [SerializeField] private GameObject _newCardAlert;

        public void ShowPauseButton(bool value)
        {
            _pauseButton.SetActive(value);
        }

        public void ShowPauseButtonAlert(bool value)
        {
            _newCardAlert.SetActive(value);
        }

        public void OnPauseButtonClick()
        {
            GameManager.Instance.SwitchPauseMenu();
        }
    }
}