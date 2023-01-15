using System.Collections;
using System.Collections.Generic;
using Booble;
using Booble.CardGame;
using Booble.Flags;
using Booble.Interactables;
using Booble.Interactables.Dialogues;
using Booble.Managers;
using Booble.Player;
using Booble.UI;
using DG.Tweening;
using UnityEngine;

public class BossOfficeEndingAnimation : MonoBehaviour
{
    [SerializeField] private Controller _controller;
    [SerializeField] private Fader _fade;
    [SerializeField] private Dialogue _dialogue;

    private Camera _cam;
    private bool _dialogueEnd;
    private DialogueManager _diagMng;

    private void Awake()
    {
        _cam = Camera.main;
        _diagMng = DialogueManager.Instance;
    }

    private void Start()
    {
        StartCoroutine(OfficeDialogueCoroutine());
    }

    private IEnumerator OfficeDialogueCoroutine()
    {
        _controller.enabled = false;
        Interactable.ManualInteractionActivation();
        yield return new WaitForSeconds(_fade.FadeDuration);
        
       ThrowDialogue(_dialogue);
       yield return new WaitUntil(() => _dialogueEnd);
       _dialogueEnd = false;

       SceneLoader.Instance.LoadCredits();
    }

    private void ThrowDialogue(Dialogue diag, List<Option> options = null)
    {
        _dialogueEnd = false;
        options?.ForEach(op => op.DialogueOption.DoOnSelect.AddListener(() => _dialogueEnd = true));
        _diagMng.StartDialogue(diag, options, hideBackOption: true);
        _diagMng.OnEndDialogue.RemoveAllListeners();
        _diagMng.OnEndDialogue.AddListener(() => _dialogueEnd = true);
    }

    public void DialogueEnd()
    {
        _dialogueEnd = true;
    }
}