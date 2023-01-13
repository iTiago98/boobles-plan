using Booble.CardGame.Managers;
using Booble.Flags;
using Booble.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Booble.CardGame.UI
{
    public class LoseMenu : MonoBehaviour
    {
        [SerializeField] private Button _returnToExplorationButton;

        private void Start()
        {
            if (CardGameManager.Instance.playingStoryMode)
            {
                _returnToExplorationButton.gameObject.SetActive(true);

                if (DeckManager.Instance.GetOpponentName() == Opponent_Name.Boss)
                {
                    _returnToExplorationButton.interactable = false;
                }
            }
            else
            {
                _returnToExplorationButton.gameObject.SetActive(false);
            }
        }

        public void OnRetryButtonClick()
        {
            SceneLoader.Instance.ReloadInterview();
        }

        public void OnReturnToExplorationButtonClick()
        {
            Opponent_Name opponentName = DeckManager.Instance.GetOpponentName();
            string scene = "";

            switch (opponentName)
            {
                case Opponent_Name.Tutorial:
                    scene = Scenes.LOUNGE_0; break;

                case Opponent_Name.Citriano:
                    scene = Scenes.LOWER_HALL_1; break;

                case Opponent_Name.PPBros:
                    scene = Scenes.LOWER_HALL_2; break;

                case Opponent_Name.Secretary:
                    break;

                case Opponent_Name.Boss:
                    break;
            }

            SceneLoader.Instance.UnloadInterviewAndLoadScene(scene);
        }

        public void OnReturnToMenuButtonClick()
        {
            if (CardGameManager.Instance.playingStoryMode)
            {
                SceneLoader.Instance.UnloadInterviewAndLoadScene(Scenes.MAIN_MENU);
            }
            else
            {
                SceneLoader.Instance.UnloadInterviewScene();
            }
        }
    }
}