using System.Collections;
using System.Collections.Generic;
using Booble.Interactables.Events;
using UnityEngine;
using UnityEngine.Events;

public class Callback : DialogueEvent
{
    [SerializeField] private UnityEvent _callback;
    
    public override void Execute()
    {
        _callback.Invoke();
    }
}
