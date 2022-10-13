using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueCallbacks : MonoBehaviour
{
    public void OnQuecaInterview()
    {
        SceneLoader.Instance.LoadNelaOffice0();
    }
}
