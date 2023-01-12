using Booble.CardGame.Managers;
using Booble.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Booble.CardGame.UI
{
    public class LoseMenu : MonoBehaviour
    {
        public void OnRetryButtonClick()
        {
            SceneLoader.Instance.ReloadInterview();
        }

        public void OnReturnToMenuButtonClick()
        {
            SceneLoader.Instance.UnloadInterviewScene();
            if (CardGameManager.Instance.playingStoryMode) SceneLoader.Instance.LoadMainMenuScene();
        }
    }
}