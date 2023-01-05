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

        private bool _dialogueEnd;
        public bool GetDialogueEnd() => _dialogueEnd;

        private void Awake()
        {
            _dialogueManager = DialogueManager.Instance;
        }

        abstract public void CheckDialogue(Card cardPlayed);
        virtual public void ThrowStartDialogue()
        {
            ThrowDialogue(_startDialogue, CardGameManager.Instance.StartGame);
        }

        public void ThrowEndDialogue(bool playerWin)
        {
            Dialogue dialogue = null;
            Action onEndDialogue = null;

            if (CardGameManager.Instance.alternateWinCondition)
            {
                dialogue = _alternateWinDialogue;
                onEndDialogue = SceneLoader.Instance.UnloadInterviewScene;
            }
            else if (playerWin)
            {
                dialogue = _winDialogue;
                onEndDialogue = SceneLoader.Instance.UnloadInterviewScene;
            }
            else
            {
                dialogue = _loseDialogue;
                onEndDialogue = UIManager.Instance.ShowLoseMenu;
            }

            ThrowDialogue(dialogue, onEndDialogue);
        }

        protected void ThrowDialogue(Dialogue diag, Action onEndDialogue = null, List<Option> options = null)
        {
            if (diag == null || _dialogueManager == null)
            {
                if (onEndDialogue != null) onEndDialogue();
                return;
            }
            _dialogueEnd = false;
            CardGameManager.Instance.PauseGame();

            _dialogueManager.StartDialogue(diag, options);
            _dialogueManager.OnEndDialogue.RemoveAllListeners();
            _dialogueManager.OnEndDialogue.AddListener(() =>
            {
                if (onEndDialogue != null) onEndDialogue();
                CardGameManager.Instance.ResumeGame();
                _dialogueEnd = true;
            });
        }
    }
}