using System.Collections;
using System.Collections.Generic;
using Booble.Flags;
using Booble.Interactables;
using Booble.Interactables.Dialogues;
using Booble.Managers;
using Booble.Player;
using Booble.UI;
using UnityEngine;

public class Lounge4Animation : MonoBehaviour
{
    [SerializeField] private Controller _controller;
    [SerializeField] private Fader _fade;
    [SerializeField] private Transform _anaTalkPosition;
    [SerializeField] private Transform _elevatorPosition;
    [SerializeField] private Dialogue _dialogue;
    [SerializeField] private Dialogue _dialogue2;
    [SerializeField] private List<Option> _options;
    [SerializeField] private Dialogue _winconDialogue;
    [SerializeField] private Dialogue _winconDialogue2;
    [SerializeField] private Dialogue _outroDialogue;
    [SerializeField] private Dialogue _elevatorDialogue;
    
    private bool _dialogueEnd;
    private bool _extraDialogue;
    private DialogueManager _diagMng;

    private void Awake()
    {
        _diagMng = DialogueManager.Instance;
    }

    private void Start()
    {
        StartCoroutine(OfficeDialogueCoroutine());
    }

    private IEnumerator OfficeDialogueCoroutine()
    {
        // _controller.enabled = false;
        Interactable.ManualInteractionActivation();
        yield return new WaitForSeconds(_fade.FadeDuration);
        
        _controller.SetDestination(_anaTalkPosition.position.x);
        yield return new WaitUntil(() => _controller.Arrived);
        
        ThrowDialogue(_dialogue, _options);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;
        
        ThrowDialogue(_dialogue2, _options);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;

        if (_extraDialogue)
        {
            ThrowDialogue(_winconDialogue);
            yield return new WaitUntil(() => _dialogueEnd);
            _dialogueEnd = false;
            
            if(FlagManager.Instance.GetFlag(Flag.Reference.CitrianoVictoriaAlternativa))
            {
                DeckManager.Instance.AddHipervitaminadoPlus();
            }

            if (FlagManager.Instance.GetFlag(Flag.Reference.PPBVictoriaAlternativa))
            {
                DeckManager.Instance.AddVictoriaPorDesgastePlus();
            }

            if (FlagManager.Instance.GetFlag(Flag.Reference.SecretaryVictoriaAlternativa))
            {
                DeckManager.Instance.AddHaPerdidoUsteLosPapelePlus();
            }
            
            ThrowDialogue(_winconDialogue2);
            yield return new WaitUntil(() => _dialogueEnd);
            _dialogueEnd = false;
        }
        
        ThrowDialogue(_outroDialogue);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;
        
        _controller.SetDestination(_elevatorPosition.position.x);
        yield return new WaitUntil(() => _controller.Arrived);
        
        ThrowDialogue(_elevatorDialogue);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;
        
        FlagManager.Instance.SetFlag(Flag.Reference.BossHall4);
        SceneLoader.Instance.LoadBossHall4();
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

    public void CheckWinConDialogue()
    {
        DialogueEnd();
        if (!FlagManager.Instance.GetFlag(Flag.Reference.Lounge4Agapito)
            || !FlagManager.Instance.GetFlag(Flag.Reference.Lounge4Marido)
            || (!FlagManager.Instance.GetFlag(Flag.Reference.CitrianoVictoriaAlternativa)
                && !FlagManager.Instance.GetFlag(Flag.Reference.PPBVictoriaAlternativa)
                && !FlagManager.Instance.GetFlag(Flag.Reference.SecretaryVictoriaAlternativa)))
        {
            return;
        }

        _extraDialogue = true;
    }
}
