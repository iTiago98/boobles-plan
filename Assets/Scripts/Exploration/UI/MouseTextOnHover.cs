using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Cursor = Booble.UI.Cursor;

public class MouseTextOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Cursor.CursorText _text; 
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_text == Cursor.CursorText.None)
        {
            Cursor.Instance.ShowActionText(false);
            return;
        }
        
        Cursor.Instance.ShowActionText(true);
        Cursor.Instance.SetText(_text);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Cursor.Instance.ShowActionText(false);
    }
}
