using System.Collections;
using System.Collections.Generic;
using Booble.Interactables.Events;
using CardGame.Level;
using UnityEngine;

public class ObtainGranFinal : DialogueEvent
{
    public override void Execute()
    {
        Debug.Log("ADD2");
        DeckManager.Instance.AddGranFinal();
    }
}
