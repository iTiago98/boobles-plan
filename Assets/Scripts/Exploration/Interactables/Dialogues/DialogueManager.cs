using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Santi.Utils;
using Booble.Flags;

namespace Booble.Interactables.Dialogues
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        [System.Serializable]
        public class Option
        {
            public string Text => _optionText;
            public UnityEvent OnSelect => _onSelectOption;

            [TextArea]
            [SerializeField] private string _optionText;
            [SerializeField] private List<Flag.Reference> _trueFlags;
            [SerializeField] private List<Flag.Reference> _falseFlags;
            [SerializeField] private UnityEvent _onSelectOption;

            public bool FlagsSatisfied()
            {
                return FlagManager.Instance.FlagsSatisfied(_trueFlags, _falseFlags);
            }
        }

        public UnityEvent OnEndDialogue { get; set; }

        [SerializeField] private KeyCode _nextKey;
        [SerializeField] private float _characterDelay;
        [SerializeField] private AudioSource _characterSoundAS;
        [SerializeField] private GameObject _dialogueBox;
        [SerializeField] private TextMeshProUGUI _dialogueText;
        [SerializeField] private GameObject _optionsBox;

        private Dialogue _currentDialogue;
        private List<Option> _options;
        private string _currentSentence;
        private bool _dialogueRunning;
        private bool _typing;
        private bool _staggered;

        private void Awake()
        {
            OnEndDialogue = new UnityEvent();
        }

        public void StartDialogue(Dialogue dialogue)
        {
            StartDialogue(dialogue, null);
        }

        public void StartDialogue(Dialogue dialogue, List<Option> options)
        {
            _staggered = false;

            _dialogueRunning = true;
            _currentDialogue = dialogue;
            _options = options ??= new List<Option>();
            if (!dialogue.Empty)
            {
                _dialogueBox.SetActive(true);
            }
            DisplayNextSentence(true);
        }

        public void DisplayNextSentence(bool firstSentence = false)
        {
            if (_currentDialogue.GetNextSentence(out _currentSentence, firstSentence))
            {
                StopAllCoroutines();
                StartCoroutine(TypeSentence(_currentSentence));
            }
            else if (_options.Count > 0)
            {
                _optionsBox.SetActive(true);
                InitializeAllOptions();
            }
            else
            {
                EndDialogue();
            }
        }

        private IEnumerator TypeSentence(string sentence)
        {
            _typing = true;
            _dialogueText.text = "";

            foreach (char letter in sentence.ToCharArray())
            {
                _dialogueText.text += letter;
                _characterSoundAS.Play();
                yield return new WaitForSecondsRealtime(_characterDelay);
            }

            _typing = false;
        }

        public void DisplayLastSentence()
        {
            _dialogueText.text = _currentDialogue.GetLastSentence();
            if (_options.Count > 0)
            {
                _optionsBox.SetActive(true);
                InitializeAllOptions();
            }
        }

        public void EndDialogue()
        {
            _dialogueRunning = false;
            _dialogueBox.SetActive(false);
            _optionsBox.SetActive(false);
            OnEndDialogue.Invoke();
        }

        private void Update()
        {
            if (!_staggered)
            {
                _staggered = true;
                return;
            }

            if (!_dialogueRunning)
                return;

            if (!Input.GetKeyDown(_nextKey))
                return;

            if (_typing)
            {
                _dialogueText.text = _currentSentence;
                StopAllCoroutines();
                _typing = false;
            }
            else
            {
                DisplayNextSentence();
            }
        }

        private void InitializeAllOptions()
        {
            foreach (Transform t in _optionsBox.transform)
            {
                t.gameObject.SetActive(false);
            }

            InitializeEndDialogueOption();

            for (int i = 0; i < _options.Count; i++)
            {
                if (!_options[i].FlagsSatisfied())
                    continue;

                InitializeOption(_options[i], _optionsBox.transform.GetChild(i+1));
            }
        }

        private void InitializeEndDialogueOption()
        {
            Transform optionT = _optionsBox.transform.GetChild(0);
            Button optionButton = optionT.GetComponent<Button>();

            optionT.gameObject.SetActive(true);
            optionT.GetChild(0).GetComponent<TextMeshProUGUI>().text = "* (En otro momento).";
            optionButton.onClick.RemoveAllListeners();
            optionButton.onClick.AddListener(() =>
            {
                _dialogueBox.SetActive(false);
                _optionsBox.SetActive(false);
                _dialogueRunning = false;
                EndDialogue();
            });
        }

        private void InitializeOption(Option option, Transform optionT)
        {
            Button optionButton = optionT.GetComponent<Button>();

            optionT.gameObject.SetActive(true);
            optionT.GetChild(0).GetComponent<TextMeshProUGUI>().text = option.Text;
            optionButton.onClick.RemoveAllListeners();
            optionButton.onClick.AddListener(() =>
            {
                _dialogueBox.SetActive(false);
                _optionsBox.SetActive(false);
                _dialogueRunning = false;
                option.OnSelect.Invoke();
            });
        }
    }
}
