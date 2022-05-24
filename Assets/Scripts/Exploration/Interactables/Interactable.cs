using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Booble.Interactables.Dialogues;

namespace Booble.Interactables
{
	public class Interactable : MonoBehaviour
	{
        public static bool BlockActions => _mouseOverInteractable || _interactionOnGoing;

        private static Dialogue _returnDialogue;
        private static List<DialogueManager.Option> _returnOptions;
        private static bool _mouseOverInteractable;
        private static bool _interactionOnGoing;

        public static void ReturnToDialogue()
        {
            DialogueManager.Instance.StartDialogue(_returnDialogue, _returnOptions);
            DialogueManager.Instance.OnEndDialogue.RemoveAllListeners();
            DialogueManager.Instance.OnEndDialogue.AddListener(() => { EndInteraction(); });
            DialogueManager.Instance.DisplayLastSentence();
        }

        public static void EndInteraction()
        {
            _interactionOnGoing = false;
            _mouseOverInteractable = false;
        }

        [SerializeField] private float _interactDistance;
        [SerializeField] private Dialogue _dialogue;
        [SerializeField] private List<DialogueManager.Option> _options;

        private Player.Controller _player;
        private UI.Cursor _cursor;
        private DialogueManager _diagManager;

        private bool _mouseOverThisInteractable;

        private float _xDistanceToPlayer => Mathf.Abs(_player.transform.position.x - transform.position.x);

        private void Awake()
        {
            _player = Player.Controller.Instance;
            _cursor = UI.Cursor.Instance;
            _diagManager = DialogueManager.Instance;
        }

        private void OnMouseEnter()
        {
            _mouseOverThisInteractable = true;

            if (_interactionOnGoing)
                return;

            _mouseOverInteractable = true;
            _cursor.ShowActionText(true);
        }

        private void OnMouseExit()
        {
            _mouseOverThisInteractable = false;

            if (_interactionOnGoing)
                return;

            _mouseOverInteractable = false;
            _cursor.ShowActionText(false);
        }

        private void OnMouseDown()
        {
            if (_interactionOnGoing)
                return;

            if (_xDistanceToPlayer <= _interactDistance)
            {
                _interactionOnGoing = true;
                _returnDialogue = _dialogue;
                _returnOptions = _options;

                _player.StopMovement();
                _cursor.ShowActionText(false);
                _diagManager.StartDialogue(_dialogue, _options);
                _diagManager.OnEndDialogue.RemoveAllListeners();
                _diagManager.OnEndDialogue.AddListener(() => { EndInteraction(); });
            }
            else
            {
                _player.SetDestination(transform.position.x, _interactDistance*0.8f);
            }
        }

        private void Update()
        {
            if (_interactionOnGoing || !_mouseOverThisInteractable)
                return;

            if(_xDistanceToPlayer <= _interactDistance)
            {
                _cursor.SetInteractText();
            }
            else
            {
                _cursor.SetApproachText();
            }
        }

        public void ChangeDialogue(Dialogue dialogue)
        {
            _dialogue = dialogue;
        }

        public void CloseOptionsMenu()
        {
            Interactable.EndInteraction();
        }
    }
}
