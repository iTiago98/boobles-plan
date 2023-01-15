using System;
using System.Collections;
using System.Collections.Generic;
using Booble;
using Booble.Flags;
using Booble.Interactables;
using Booble.Interactables.Dialogues;
using Booble.Managers;
using Booble.Player;
using Booble.UI;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HomeEndingAnimation : MonoBehaviour
{
    private readonly float PAUSE_DURATION = 5f;
    
    [SerializeField] private Controller _controller;
    [SerializeField] private Fader _fader;
    [SerializeField] private GameObject _mask;
    [SerializeField] private Dialogue _dialogue0;
    [SerializeField] private Dialogue _dialogue1;
    [SerializeField] private Dialogue _dialogue2;
    [SerializeField] private Dialogue _dialogue3;
    [SerializeField] private Dialogue _dialogue4;
    [SerializeField] private Dialogue _dialogue5;
    [SerializeField] private List<Option> _options;
    [SerializeField] private SpriteRenderer _censurao;
    [SerializeField] private GameObject _fin;
    
    private bool _continue;
    private bool _clicked = true;
    private bool _endScene;

    private void Start()
    {
        Interactable.ManualInteractionActivation();
        StartCoroutine(Animation());
    }

    private void Update()
    {
        if (_clicked)
            return;

        if (!Input.GetKeyDown(KeyCode.Mouse0))
            return;

        _clicked = true;
    }
    
    private IEnumerator Animation()
    {
        if (FlagManager.Instance.GetFlag(Flag.Reference.FinalBueno))
        {
            yield return new WaitForSeconds(_fader.FadeDuration);

            yield return new WaitForSeconds(PAUSE_DURATION);
            
            _mask.SetActive(false);
            yield return new WaitForSeconds(PAUSE_DURATION);

            DialogueManager.Instance.StartDialogue(_dialogue0);
            DialogueManager.Instance.OnEndDialogue.RemoveAllListeners();
            DialogueManager.Instance.OnEndDialogue.AddListener(Continue);
            yield return new WaitUntil(() => _continue);
            _continue = false;
            yield return new WaitForSeconds(PAUSE_DURATION);
            
            DialogueManager.Instance.StartDialogue(_dialogue1);
            DialogueManager.Instance.OnEndDialogue.RemoveAllListeners();
            DialogueManager.Instance.OnEndDialogue.AddListener(Continue);
            yield return new WaitUntil(() => _continue);
            _continue = false;
            yield return new WaitForSeconds(PAUSE_DURATION);
            
            DialogueManager.Instance.StartDialogue(_dialogue2);
            DialogueManager.Instance.OnEndDialogue.RemoveAllListeners();
            DialogueManager.Instance.OnEndDialogue.AddListener(Continue);
            yield return new WaitUntil(() => _continue);
            _continue = false;
            yield return new WaitForSeconds(PAUSE_DURATION);
            
            do
            {
                _clicked = false;
                yield return new WaitUntil(() => _clicked);

                DialogueManager.Instance.StartDialogue(_dialogue3, _options);
                DialogueManager.Instance.OnEndDialogue.RemoveAllListeners();
                DialogueManager.Instance.OnEndDialogue.AddListener(Continue);

                yield return new WaitUntil(() => _continue);
                _continue = false;
            } while (!_endScene);

            DialogueManager.Instance.StartDialogue(_dialogue4);
            DialogueManager.Instance.OnEndDialogue.RemoveAllListeners();
            DialogueManager.Instance.OnEndDialogue.AddListener(Continue);
            yield return new WaitUntil(() => _continue);
            _continue = false;

            _censurao.DOColor(Color.black, 10);
            DialogueManager.Instance.StartDialogue(_dialogue5);
            DialogueManager.Instance.OnEndDialogue.RemoveAllListeners();
            DialogueManager.Instance.OnEndDialogue.AddListener(Continue);
            yield return new WaitUntil(() => _continue);
            _continue = false;
        }
        
        _fin.SetActive(true);
        yield return new WaitForSeconds(PAUSE_DURATION);

        SceneLoader.Instance.LoadMainMenuScene();
    }
    
    public void Continue()
    {
        _continue = true;
    }

    public void EndScene()
    {
        _endScene = true;
    }
}
