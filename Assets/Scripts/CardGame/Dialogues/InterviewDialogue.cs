using Booble.CardGame.Cards;
using Booble.CardGame.Managers;
using Booble.Interactables.Dialogues;
using Booble.Managers;
using Booble.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Booble.CardGame.Dialogues
{
    public abstract class InterviewDialogue : MonoBehaviour
    {
        protected DialogueManager _dialogueManager;

        [SerializeField] protected Dialogue _startDialogue;
        [SerializeField] protected Dialogue _winDialogue;
        [SerializeField] protected Dialogue _alternateWinDialogue;
        [SerializeField] protected Dialogue _loseDialogue;

        protected bool _dialogueEnd;
        public bool GetDialogueEnd() => _dialogueEnd;

        private void Awake()
        {
            _dialogueManager = DialogueManager.Instance;
        }

        abstract public bool CheckDialogue(Card cardPlayed);
        virtual public void ThrowStartDialogue()
        {
            ThrowDialogue(_startDialogue, CardGameManager.Instance.StartGame);
        }

        public void ThrowEndDialogue(bool playerWin, Action onEndAction)
        {
            Dialogue dialogue = CardGameManager.Instance.alternateWinCondition ? _alternateWinDialogue :
                playerWin ? _winDialogue : _loseDialogue;

            ThrowDialogue(dialogue, onEndAction);
        }

        protected void ThrowDialogue(Dialogue diag, Action onEndDialogue = null, List<Option> options = null, bool hideBackOption = false)
        {
            if (diag == null || _dialogueManager == null)
            {
                if (onEndDialogue != null) onEndDialogue();
                return;
            }
            _dialogueEnd = false;
            CardGameManager.Instance.PauseGame();

            _dialogueManager.StartDialogue(diag, options, hideBackOption);
            _dialogueManager.OnEndDialogue.RemoveAllListeners();
            _dialogueManager.OnEndDialogue.AddListener(() =>
            {
                CardGameManager.Instance.ResumeGame();
                if (onEndDialogue != null) onEndDialogue();
                _dialogueEnd = true;
            });
        }
    }
}