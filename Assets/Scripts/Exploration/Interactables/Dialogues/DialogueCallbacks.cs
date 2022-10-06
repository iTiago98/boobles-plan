using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueCallbacks : MonoBehaviour
{
    public void OnQuecaInterview(string opponentName)
    {
        DeckManager.Instance.SetOpponentCards(opponentName);
        SceneLoader.Instance.LoadInterviewScene();
    }
}
