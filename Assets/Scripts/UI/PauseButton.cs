using Booble.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Booble.UI
{
    public class PauseButton : AlertButton
    {
        public void OnPauseButtonClick()
        {
            GameManager.Instance.SwitchPauseMenu();
        }
    }
}