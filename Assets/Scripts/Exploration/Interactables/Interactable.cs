using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Booble.Interactables.Dialogues;
using Booble.Flags;
using Booble.Characters;
using Booble.UI;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

namespace Booble.Interactables
{
	public class Interactable : MonoBehaviour
	{
        public static bool BlockActions => MouseOverInteractable || InteractionOnGoing;
        public static bool InteractionOnGoing { get; private set; }
        public static bool MouseOverInteractable { get; private set; }
        public static bool CluesOpen { get; set; }
        
        private static Dialogue _returnDialogue;
        private static List<Option> _returnOptions;

        public static void ManualInteractionActivation()
        {
            InteractionOnGoing = true;
            UI.Cursor.Instance.ShowActionText(false);
        }
        
        public static void ReturnToDialogue()
        {
            DialogueManager.Instance.StartDialogue(_returnDialogue, _returnOptions, DialogueManager.Instance.HideBackOption);
            DialogueManager.Instance.OnEndDialogue.RemoveAllListeners();
            DialogueManager.Instance.OnEndDialogue.AddListener(EndInteraction);
            DialogueManager.Instance.DisplayLastSentence();
        }

        public static void EndInteraction()
        {
            InteractionOnGoing = false;
            MouseOverInteractable = false;
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null && hit.collider.GetComponent<Interactable>())
            {
                UI.Cursor.Instance.ShowActionText(true);
            }
        }

        [SerializeField] private bool _unclickable;
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

            if (EventSystem.current.IsPointerOverGameObject())
                return;
            
            if (InteractionOnGoing)
                return;

            MouseOverInteractable = true;
            _cursor.ShowActionText(true);
        }

        private void OnMouseExit()
        {
            _mouseOverThisInteractable = false;

            if (InteractionOnGoing)
                return;

            MouseOverInteractable = false;
            _cursor.ShowActionText(false);
        }

        private void OnMouseDown()
        {
            if(EventSystem.current.IsPointerOverGameObject())
                return;

            if (InteractionOnGoing)
                return;

            if (_xDistanceToPlayer <= _interactDistance)
            {
                if (CluesOpen)
                {
                    ClueUI.Instance.ToggleClues();
                }
                StartInteraction();
            }
            else
            {
                _player.SetDestination(transform.position.x, _interactDistance*0.8f);
            }
        }

        private void Update()
        {
            if (_unclickable)
                return;
            
            if (InteractionOnGoing || !_mouseOverThisInteractable)
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
        
        public void StartInteraction(bool hideBackOption = false)
        {
            InteractionOnGoing = true;

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
            _diagManager.StartDialogue(_dialogue, _options, hideBackOption);

            _returnDialogue = _dialogue;
            _returnOptions = _options;
            _diagManager.OnEndDialogue.RemoveAllListeners();
            _diagManager.OnEndDialogue.AddListener(() => EndInteraction());
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
