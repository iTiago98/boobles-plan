using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Booble.Flags;
using Booble.Interactables;
using Booble.Interactables.Dialogues;
using Booble.Interactables.Events;
using Booble.Managers;
using Booble.Player;
using Booble.UI;
using UnityEngine;

public class Animation2_1 : DialogueEvent
{
    [SerializeField] private Controller _controller;
    [SerializeField] private Fader _fader;
    [SerializeField] private Dialogue _preDialogue;
    [SerializeField] private List<Option> _options0;
    [SerializeField] private Dialogue _dialogue0;
    [SerializeField] private Dialogue _dialogue1;
    [SerializeField] private Dialogue _dialogue2;
    [SerializeField] private Dialogue _dialogue3;
    [SerializeField] private GameObject _puerta;

    private bool _continue;
    private bool _repeat;

    private void Awake()
    {
        if(FlagManager.Instance.GetFlag(Flag.Reference.Cerrar))
        {
            _puerta.GetComponent<Collider2D>().enabled = false;
            _puerta.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public override void Execute()
    {
        StartCoroutine(Animation());
    }

    private IEnumerator Animation()
    {
        ThrowDialogue(_preDialogue);
        yield return new WaitUntil(() => _continue);
        _continue = false;
        
        _fader.FadeOut(Continue);
        yield return new WaitUntil(() => _continue);
        _continue = false;
        
        do
        {
            ThrowDialogue(_dialogue0, _options0);
            yield return new WaitUntil(() => _continue);
            _continue = false;
        
            if (FlagManager.Instance.GetFlag(Flag.Reference.Cerrar))
            {
                MusicManager.Instance.StopMusic();
                ThrowDialogue(_dialogue1);
                yield return new WaitUntil(() => _continue);
                _continue = false;
        
                yield return new WaitForSeconds(5);
                
                ThrowDialogue(_dialogue2);
                yield return new WaitUntil(() => _continue);
                _continue = false;
                
                yield return new WaitForSeconds(5);
                
                ThrowDialogue(_dialogue3);
                yield return new WaitUntil(() => _continue);
                _continue = false;
                
                _repeat = false;
            }
        } while (_repeat);
        
        if(FlagManager.Instance.GetFlag(Flag.Reference.Cerrar))
        {
            _puerta.GetComponent<Collider2D>().enabled = false;
            _puerta.GetComponent<SpriteRenderer>().enabled = false;
        }
        
        MusicManager.Instance.PlayMusic(MusicReference.Lounge);
        _fader.FadeIn(Continue);
        yield return new WaitUntil(() => _continue);
        _continue = false;
     
        Interactable.EndInteraction();
    }
    
    private void ThrowDialogue(Dialogue diag, List<Option> options = null)
    {
        _continue = false;
        options?.ForEach(op => op.DialogueOption.DoOnSelect.AddListener(() => _continue = true));
        DialogueManager.Instance.StartDialogue(diag, options);
        DialogueManager.Instance.OnEndDialogue.RemoveAllListeners();
        DialogueManager.Instance.OnEndDialogue.AddListener(() => _continue = true);
    }

    private void Continue()
    {
        _continue = true;
    }

    public void Exit()
    {
        _repeat = false;
    }
}
