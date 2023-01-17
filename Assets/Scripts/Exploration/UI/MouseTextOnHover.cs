using System.Collections;
using System.Collections.Generic;
using Booble.Interactables;
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
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null && hit.collider.GetComponent<Interactable>())
        {
            Cursor.Instance.ShowActionText(true);
        }
        else
        {
            Cursor.Instance.ShowActionText(false);
        }
    }
}
