using System.Collections;
using System.Collections.Generic;
using Booble.Interactables;
using Booble.Interactables.Dialogues;
using Booble.Interactables.Events;
using Booble.UI;
using DG.Tweening;
using UnityEngine;

public class PPBCinematic : DialogueEvent
{
    [SerializeField] private Fader _fader;
    [SerializeField] private Dialogue _dialogue0;
    [SerializeField] private Dialogue _dialogue1;
    [SerializeField] private Dialogue _dialogue2;
    [SerializeField] private Dialogue _dialogue3;
    [SerializeField] private GameObject _dennis;
    [SerializeField] private Transform _anaT;
    [SerializeField] private Animator _ppbAnim;
    [SerializeField] private Animator _anaAnim;
    [SerializeField] private float _anaXPos;

    private bool _continue;
    
    public override void Execute()
    {
        StartCoroutine(Animation());
    }

    private IEnumerator Animation()
    {
        DialogueManager.Instance.StartDialogue(_dialogue0);
        DialogueManager.Instance.OnEndDialogue.RemoveAllListeners();
        DialogueManager.Instance.OnEndDialogue.AddListener(Continue);
        
        yield return new WaitUntil(() => _continue);
        _continue = false;

        DialogueManager.Instance.StartDialogue(_dialogue1);
        DialogueManager.Instance.OnEndDialogue.RemoveAllListeners();
        DialogueManager.Instance.OnEndDialogue.AddListener(Continue);
        
        yield return new WaitUntil(() => _continue);
        _continue = false;

        _fader.FadeOut(Continue);

        yield return new WaitUntil(() => _continue);
        _continue = false;
        
        _dennis.SetActive(true);
        _fader.FadeIn(Continue);
        
        yield return new WaitUntil(() => _continue);
        _continue = false;
        
        DialogueManager.Instance.StartDialogue(_dialogue2);
        DialogueManager.Instance.OnEndDialogue.RemoveAllListeners();
        DialogueManager.Instance.OnEndDialogue.AddListener(Continue);
        
        yield return new WaitUntil(() => _continue);
        _continue = false;
        
        _anaT.gameObject.SetActive(true);
        _anaAnim.SetBool("Walking", true);
        _anaT.DOMoveX(_anaXPos, 2)
            .SetEase(Ease.Linear)
            .OnComplete(Continue);
        
        yield return new WaitUntil(() => _continue);
        _continue = false;
        
        _anaAnim.GetComponent<SpriteRenderer>().flipX = false;
        _anaAnim.SetBool("Walking", false);
        DialogueManager.Instance.StartDialogue(_dialogue3);
        DialogueManager.Instance.OnEndDialogue.RemoveAllListeners();
        DialogueManager.Instance.OnEndDialogue.AddListener(Continue);
        
        yield return new WaitUntil(() => _continue);
        _continue = false;

        _fader.FadeOut(Continue);
        
        yield return new WaitUntil(() => _continue);
        _continue = false;
        
        _anaT.gameObject.SetActive(false);
        _dennis.SetActive(false);
        _fader.FadeIn(Interactable.EndInteraction);
        
        yield return null;
    }

    private void Continue()
    {
        _continue = true;
    }
}
