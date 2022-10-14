using System.Collections;
using System.Collections.Generic;
using Booble.Interactables.Events;
using UnityEngine;

public class ThrowAgain : DialogueEvent
{
    [SerializeField] private NelaOffice1Animation _anim;


    public override void Execute()
    {
        _anim.ThrowAgain();
    }
}
