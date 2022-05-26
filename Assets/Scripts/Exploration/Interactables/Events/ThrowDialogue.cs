using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Booble.Interactables.Dialogues;
using static Booble.Interactables.Interactable;

namespace Booble.Interactables.Events
{
	public class ThrowDialogue : MonoBehaviour
	{
        [SerializeField]
        private enum EndType { Nothing, Return, Close }
        
        [SerializeField] private Dialogue _dialogue;
        [SerializeField] private List<AnimatorIdentifier> _animatorIdentifiers;
        [SerializeField] private EndType _onEnd;

		private DialogueManager _diagManager;

        protected virtual void Awake()
        {
            _diagManager = DialogueManager.Instance;
        }

        public void StartInteraction()
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
            }
        }

        protected virtual void OnDialogueEnd() { }
	}
}
