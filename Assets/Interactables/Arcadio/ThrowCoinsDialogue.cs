using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Booble.Interactables.Dialogues;
using static Booble.Interactables.Interactable;

namespace Booble.Interactables.Events
{
	public class ThrowCoinsDialogue : MonoBehaviour
	{
        [SerializeField]
        private enum EndType { Nothing, Return, Close, Callback }
        
        [SerializeField] private Dialogue _dialogue1;
        [SerializeField] private Dialogue _dialogue2;
        [SerializeField] private Dialogue _dialogue3;
        [SerializeField] private ArcadioContinues _arcCont;
        [SerializeField] private List<AnimatorIdentifier> _animatorIdentifiers;
        [SerializeField] private EndType _onEnd;
        [SerializeField] private UnityEvent _callbackEvent;

		private DialogueManager _diagManager;
        private Dialogue _dialogue;

        protected virtual void Awake()
        {
            _diagManager = DialogueManager.Instance;
        }

        public void StartInteraction()
        {
            switch(_arcCont.CoinCount)
            {
                case 1:
                    _dialogue = _dialogue1;
                    break;
                case 2:
                    _dialogue = _dialogue2;
                    break;
                case 3:
                    _dialogue = _dialogue3;
                    break;
            }

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
                    _diagManager.OnEndDialogue.AddListener(() => Interactable.EndInteraction());
                    _diagManager.OnEndDialogue.AddListener(() => _callbackEvent.Invoke());
                    break;
            }
        }

        protected virtual void OnDialogueEnd() { }
	}
}
