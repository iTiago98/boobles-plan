using System;
using System.Collections;
using System.Collections.Generic;
using Booble;
using Booble.Interactables;
using Booble.Interactables.Dialogues;
using Booble.Managers;
using Booble.Player;
using Booble.UI;
using Unity.VisualScripting;
using UnityEngine;

public class HomeAnimation0 : MonoBehaviour
{
    [SerializeField] private Controller _controller;
    [SerializeField] private Transform _nelaPos;
    [SerializeField] private Dialogue _dialogue0;
    [SerializeField] private Fader _fader;

    private bool _continue;
    
    private void Start()
    {
        Interactable.ManualInteractionActivation();
        StartCoroutine(Animation());
    }

    private IEnumerator Animation()
    {
        yield return new WaitForSeconds(_fader.FadeDuration);
        
        _controller.SetDestination(_nelaPos.position.x);
        yield return new WaitUntil(() => _controller.Arrived);
        
        DialogueManager.Instance.StartDialogue(_dialogue0);
        DialogueManager.Instance.OnEndDialogue.RemoveAllListeners();
        DialogueManager.Instance.OnEndDialogue.AddListener(Continue);
        yield return new WaitUntil(() => _continue);
        _continue = false;

        _fader.FadeOut(Continue);
        yield return new WaitUntil(() => _continue);
        _continue = false;
        
        SceneLoader.Instance.LoadScene(Scenes.CAR_0);
    }
    
    private void Continue()
    {
        _continue = true;
    }
}
