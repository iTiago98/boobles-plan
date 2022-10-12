using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Booble.Interactables.Events
{
    public class DialogueOption : MonoBehaviour
    {
        public UnityEvent DoOnSelect { get; set; }
        
        private List<DialogueEvent> _events;

        private void Awake()
        {
            DoOnSelect = new UnityEvent();
            _events = new List<DialogueEvent>();
            foreach (DialogueEvent dialogueEvent in GetComponents<DialogueEvent>())
            {
                _events.Add(dialogueEvent);
            }
        }

        public void OnSelect()
        {
            DoOnSelect.Invoke();
            foreach (DialogueEvent dialogueEvent in _events)
            {
                dialogueEvent.Execute();
            }
        }
    }
}
