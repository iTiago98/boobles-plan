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
        //private bool _dialogueEnd;
        protected DialogueManager _dialogueManager;

        [SerializeField] protected Dialogue _startDialogue;
        [SerializeField] protected Dialogue _winDialogue;
        [SerializeField] protected Dialogue _alternateWinDialogue;
        [SerializeField] protected Dialogue _loseDialogue;

        private void Awake()
        {
            _dialogueManager = DialogueManager.Instance;
        }

        abstract public void CheckDialogue(Card cardPlayed);
        virtual public void ThrowStartDialogue()
        {
            if (_startDialogue != null && _dialogueManager != null) ThrowDialogue(_startDialogue, CardGameManager.Instance.StartGame);
            else
                CardGameManager.Instance.StartGame(); // TEMPORAL
        }

        public void ThrowEndDialogue(bool playerWin)
        {
            Action onEndDialogue = SceneLoader.Instance.UnloadInterviewScene;

            Dialogue dialogue = null;

            if (CardGameManager.Instance.alternateWinCondition)
                dialogue = _alternateWinDialogue;
            else if (playerWin)
                dialogue = _winDialogue;
            else
                dialogue = _loseDialogue;

            if (dialogue != null) ThrowDialogue(dialogue, onEndDialogue);
            else onEndDialogue();
        }

        protected void ThrowDialogue(Dialogue diag, Action onEndDialogue = null, List<Option> options = null)
        {
            if (diag == null || _dialogueManager == null) return;
            CardGameManager.Instance.PauseGame();

            // _dialogueEnd = false;
            _dialogueManager.StartDialogue(diag, options);
            _dialogueManager.OnEndDialogue.RemoveAllListeners();
            _dialogueManager.OnEndDialogue.AddListener(() =>
            {
                //_dialogueEnd = true;
                onEndDialogue();
                CardGameManager.Instance.ResumeGame();
            });
        }
    }
}