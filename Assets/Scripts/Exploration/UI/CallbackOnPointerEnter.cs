using System.Collections;
using System.Collections.Generic;
using Booble.Interactables;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Cursor = Booble.UI.Cursor;

public class CallbackOnPointerEnter : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private UnityEvent _callback;
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        _callback.Invoke();
    }
}
