using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Booble.Interactables.Events
{
    public class DialogueOption : MonoBehaviour
    {
        private List<DialogueEvent> _events;

        private void Awake()
        {
            _events = new List<DialogueEvent>();
            foreach (DialogueEvent dialogueEvent in GetComponents<DialogueEvent>())
            {
                _events.Add(dialogueEvent);
            }
        }

        public void OnSelect()
        {
            foreach (DialogueEvent dialogueEvent in _events)
            {
                dialogueEvent.Execute();
            }
        }
    }
}
