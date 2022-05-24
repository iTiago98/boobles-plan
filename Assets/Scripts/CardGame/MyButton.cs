using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyButton : MonoBehaviour
{
    public Text text;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void SetText(string s)
    {
        text.text = s;
    }

    public void SetInteractable(bool b)
    {
        _button.interactable = b;
    }
}
