using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Booble.Interactables.Dialogues;
using Booble.Flags;
using Booble.Characters;

namespace Booble.Interactables
{
	public class Interactable : MonoBehaviour
	{
        public static bool BlockActions => _mouseOverInteractable || _interactionOnGoing;
        public static bool _interactionOnGoing;

        private static Dialogue _returnDialogue;
        private static List<DialogueManager.Option> _returnOptions;
        private static List<AnimatorIdentifier> _returnAnimIdentifiers;
        private static bool _mouseOverInteractable;

        public static void ManualInteractionActivation()
        {
            _interactionOnGoing = true;
            UI.Cursor.Instance.ShowActionText(false);
        }
        
        public static void ReturnToDialogue()
        {
            DialogueManager.Instance.StartDialogue(_returnDialogue, _returnOptions, _returnAnimIdentifiers);
            DialogueManager.Instance.OnEndDialogue.RemoveAllListeners();
            DialogueManager.Instance.OnEndDialogue.AddListener(() => EndInteraction());
            DialogueManager.Instance.DisplayLastSentence();
            
        }

        public static void EndInteraction()
        {
            _interactionOnGoing = false;
            _mouseOverInteractable = false;
        }

        [System.Serializable]
        public class AnimatorIdentifier
        {
            public CharacterList.Name Identifier => _identifier;
            public Animator Animator => _animator;

            [SerializeField] private string _name;
            [SerializeField] private CharacterList.Name _identifier;
            [SerializeField] private Animator _animator;
        }

        public Dialogue ContinueDialogue
        {
            get { return _continueDialogue; }
            set { _continueDialogue = value; }
        }

        [SerializeField] private float _interactDistance;
        [SerializeField] private Dialogue _introDialogue;
        [SerializeField] private Dialogue _continueDialogue;
        [SerializeField] private Flag.Reference _introFlag;
        [SerializeField] private List<DialogueManager.Option> _options;
        [SerializeField] private List<AnimatorIdentifier> _animatorIdentifiers;

        private Dialogue _dialogue;
        private bool _isIntroFlagSet;

        private Player.Controller _player;
        private UI.Cursor _cursor;
        private DialogueManager _diagManager;
        private FlagManager _flagManager;

        private bool _mouseOverThisInteractable;

        private float _xDistanceToPlayer => Mathf.Abs(_player.transform.position.x - transform.position.x);

        private void Awake()
        {
            _player = Player.Controller.Instance;
            _cursor = UI.Cursor.Instance;
            _diagManager = DialogueManager.Instance;
            _flagManager = FlagManager.Instance;
        }

        private void Start()
        {
            if (_flagManager.GetFlag(_introFlag))
            {
                _dialogue = _continueDialogue;
                _isIntroFlagSet = true;
            }
            else
            {
                _dialogue = _introDialogue;
            }
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

                _player.StopMovement();
                _cursor.ShowActionText(false);
                _diagManager.StartDialogue(_dialogue, _options, _animatorIdentifiers);

                _returnOptions = _options;
                _returnAnimIdentifiers = _animatorIdentifiers;
                if (!_isIntroFlagSet)
                {
                    _flagManager.SetFlag(_introFlag);
                    _isIntroFlagSet = true;
                    _dialogue = _continueDialogue;
                }
                
                _returnDialogue = _dialogue;
                _diagManager.OnEndDialogue.RemoveAllListeners();
                _diagManager.OnEndDialogue.AddListener(() => EndInteraction());
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
