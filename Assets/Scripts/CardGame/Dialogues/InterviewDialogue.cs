using Booble.Interactables.Dialogues;
using CardGame.Cards;
using CardGame.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InterviewDialogue : MonoBehaviour
{
    private bool _dialogueEnd;
    private DialogueManager _dialogueManager;

    [SerializeField] protected Dialogue _startDialogue;
    [SerializeField] protected Dialogue _winDialogue;
    [SerializeField] protected Dialogue _alternateWinDialogue;
    [SerializeField] protected Dialogue _loseDialogue;

    private void Awake()
    {
        _dialogueManager = DialogueManager.Instance;
    }

    abstract public void CheckDialogue(Card cardPlayed);
    public void ThrowStartDialogue()
    {
        ThrowDialogue(_startDialogue);
    }

    public void ThrowWinDialogue()
    {
        if (CardGameManager.Instance.alternateWinCondition)
        {
            ThrowDialogue(_alternateWinDialogue);
        }
        else
        {
            ThrowDialogue(_winDialogue);
        }
    }

    public void ThrowLoseDialogue()
    {
        ThrowDialogue(_loseDialogue);
    }

    protected void ThrowDialogue(Dialogue diag, List<Option> options = null)
    {
        CardGameManager.Instance.PauseGame();

        _dialogueEnd = false;
        _dialogueManager.StartDialogue(diag, options);
        _dialogueManager.OnEndDialogue.RemoveAllListeners();
        _dialogueManager.OnEndDialogue.AddListener(() =>
        {
            _dialogueEnd = true;
            CardGameManager.Instance.ResumeGame();
        });
    }
}
