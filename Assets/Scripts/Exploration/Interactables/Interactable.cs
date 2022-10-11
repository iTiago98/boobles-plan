using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Booble.Interactables.Dialogues;
using Booble.Flags;
using Booble.Characters;
using UnityEngine.EventSystems;

namespace Booble.Interactables
{
	public class Interactable : MonoBehaviour
	{
        public static bool BlockActions => _mouseOverInteractable || _interactionOnGoing  || CluesOpen;
        public static bool CluesOpen { get; set; }
        
        private static bool _interactionOnGoing;
        private static Dialogue _returnDialogue;
        private static List<Option> _returnOptions;
        private static bool _mouseOverInteractable;

        public static void ManualInteractionActivation()
        {
            _interactionOnGoing = true;
            UI.Cursor.Instance.ShowActionText(false);
        }
        
        public static void ReturnToDialogue()
        {
            DialogueManager.Instance.StartDialogue(_returnDialogue, _returnOptions);
            DialogueManager.Instance.OnEndDialogue.RemoveAllListeners();
            DialogueManager.Instance.OnEndDialogue.AddListener(EndInteraction);
            DialogueManager.Instance.DisplayLastSentence();
        }

        public static void EndInteraction()
        {
            _interactionOnGoing = false;
            _mouseOverInteractable = false;
        }

        [SerializeField] private float _interactDistance;
        [SerializeField] private List<ClickDialogue> _clickDialogues;
        [SerializeField] private List<Option> _options;
        
        private Dialogue _dialogue;

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
            if(EventSystem.current.IsPointerOverGameObject())
                return;
            
            if(CluesOpen)
                return;
            
            if (_interactionOnGoing)
                return;

            if (_xDistanceToPlayer <= _interactDistance)
            {
                _interactionOnGoing = true;

                _player.StopMovement();
                _cursor.ShowActionText(false);

                bool found = false;
                int i = 0;
                while(!found && i < _clickDialogues.Count)
                {
                    ClickDialogue clickDialogue = _clickDialogues[i];
                    if(clickDialogue.FlagsSatisfied)
                    {
                        found = true;
                        _dialogue = clickDialogue.Dialogue;
                        clickDialogue.SetFlags();
                    }
                    i++;
                }
                if(!found)
                {
                    Debug.LogError("No Click Dialogue with a satisfied Flag List!");
                }
                _diagManager.StartDialogue(_dialogue, _options);

                _returnDialogue = _dialogue;
                _returnOptions = _options;
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
    }

    [System.Serializable]
    public class ClickDialogue
    {
        public Dialogue Dialogue => _dialogue;
        public bool FlagsSatisfied => FlagManager.Instance.FlagsSatisfied(_trueFlags, _falseFlags);

        [SerializeField] private Dialogue _dialogue;
        [SerializeField] private List<Flag.Reference> _trueFlags;
        [SerializeField] private List<Flag.Reference> _falseFlags;
        [SerializeField] private List<Flag.Reference> _setFlags;

        public void SetFlags()
        {
            foreach(Flag.Reference flag in _setFlags)
            {
                FlagManager.Instance.SetFlag(flag);
            }
        }
    }

}
