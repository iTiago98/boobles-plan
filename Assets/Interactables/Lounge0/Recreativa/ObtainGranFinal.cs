using Booble.Interactables.Events;
using Booble.Managers;
using UnityEngine;

public class ObtainGranFinal : DialogueEvent
{
    public override void Execute()
    {
        Debug.Log("ADD2");
        DeckManager.Instance.AddGranFinal();
    }
}
