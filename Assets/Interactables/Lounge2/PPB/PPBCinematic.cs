using System;
using System.Collections;
using System.Collections.Generic;
using Booble.Interactables;
using Booble.Interactables.Dialogues;
using Booble.Interactables.Events;
using Booble.UI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PPBCinematic : DialogueEvent
{
    [SerializeField] private Fader _fader;
    [SerializeField] private Dialogue _dialogue0;
    [SerializeField] private Dialogue _dialogue1;
    [SerializeField] private Dialogue _dialogue2;
    [SerializeField] private Dialogue _dialogue3;
    [SerializeField] private GameObject _dennis;
    [SerializeField] private Transform _anaT;
    [SerializeField] private RectTransform _boomRT;
    [SerializeField] private Animator _ppbAnim;
    [SerializeField] private Animator _anaAnim;
    [SerializeField] private float _anaXPos;

    private bool _continue;
    private Transform _camera;

    private void Awake()
    {
        _camera = Camera.main.transform;
    }

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

        _ppbAnim.SetTrigger("boom");
        
        _boomRT.gameObject.SetActive(true);
        _boomRT.DOScale(Vector3.zero, 5).SetEase(Ease.OutExpo).From();
        _camera.DOShakePosition(5, 1, 10, 90, false, true)
            .OnComplete(Continue);

            yield return new WaitForSeconds(2);
            _boomRT.GetComponent<Image>().DOFade(0, 3);
        
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
