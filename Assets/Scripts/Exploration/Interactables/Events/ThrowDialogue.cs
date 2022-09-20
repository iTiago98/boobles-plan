using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Booble.Interactables.Dialogues;
using static Booble.Interactables.Interactable;

namespace Booble.Interactables.Events
{
	public class ThrowDialogue : DialogueEvent
	{
        [SerializeField]
        private enum EndType { Nothing, Return, Close, Callback }
        
        [SerializeField] private Dialogue _dialogue;
        [SerializeField] private List<AnimatorIdentifier> _animatorIdentifiers;
        [SerializeField] private EndType _onEnd;
        [SerializeField] private UnityEvent _callbackEvent;

		private DialogueManager _diagManager;

        protected virtual void Awake()
        {
            _diagManager = DialogueManager.Instance;
        }

        public override void Execute()
        {
            _diagManager.StartDialogue(_dialogue, _animatorIdentifiers);
            _diagManager.OnEndDialogue.RemoveAllListeners();
            _diagManager.OnEndDialogue.AddListener(() => OnDialogueEnd());
            switch(_onEnd)
            {
                case EndType.Return:
                    _diagManager.OnEndDialogue.AddListener(() => Interactable.ReturnToDialogue());
                    break;
                case EndType.Close:
                    _diagManager.OnEndDialogue.AddListener(() => Interactable.EndInteraction());
                    break;
                case EndType.Callback:
                    _diagManager.OnEndDialogue.AddListener(() => _callbackEvent.Invoke());
                    break;
            }
        }

        protected virtual void OnDialogueEnd() { }
	}
}
