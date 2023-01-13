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
using Unity.VisualScripting;
using UnityEngine;

public class HomeAnimation1 : MonoBehaviour
{
    [SerializeField] private Controller _controller;
    [SerializeField] private Transform _talkPos;
    [SerializeField] private Transform _hidePos;
    [SerializeField] private Fader _fader;
    [SerializeField] private GameObject _dark;
    [SerializeField] private GameObject _light;
    [SerializeField] private GameObject _sitNela;
    [SerializeField] private Dialogue _dataSaved;
    [SerializeField] private Dialogue _dialogue0;
    [SerializeField] private Dialogue _dialogue1;
    [SerializeField] private List<Option> _options;
    
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
        yield return new WaitForSeconds(_fader.FadeDuration);
        
        _controller.SetDestination(_talkPos.position.x);
        yield return new WaitUntil(() => _controller.Arrived);
        
        DialogueManager.Instance.StartDialogue(_dialogue0);
        DialogueManager.Instance.OnEndDialogue.RemoveAllListeners();
        DialogueManager.Instance.OnEndDialogue.AddListener(Continue);
        yield return new WaitUntil(() => _continue);
        _continue = false;

        _fader.FadeOut(Continue);
        yield return new WaitUntil(() => _continue);
        _continue = false;
        
        _controller.SetDestination(_hidePos.position.x);
        _controller.transform.position = new Vector3(_hidePos.position.x, _controller.transform.position.y, _controller.transform.position.z);
        _dark.SetActive(true);
        _light.SetActive(true);
        _sitNela.SetActive(true);
        _fader.FadeIn(Continue);
        yield return new WaitUntil(() => _continue);
        _continue = false;

        do
        {
            _clicked = false;
            yield return new WaitUntil(() => _clicked);

            DialogueManager.Instance.StartDialogue(_dialogue1, _options);
            DialogueManager.Instance.OnEndDialogue.RemoveAllListeners();
            DialogueManager.Instance.OnEndDialogue.AddListener(Continue);

            yield return new WaitUntil(() => _continue);
            _continue = false;
        } while (!_endScene);
        
        FlagManager.Instance.SetFlag(Flag.Reference.Home1);
        DialogueManager.Instance.StartDialogue(_dataSaved);
        DialogueManager.Instance.OnEndDialogue.RemoveAllListeners();
        DialogueManager.Instance.OnEndDialogue.AddListener(Continue);
        yield return new WaitUntil(() => _continue);
        _continue = false;

        _fader.FadeOut(Continue);
        yield return new WaitUntil(() => _continue);
        _continue = false;
        
        SceneLoader.Instance.LoadScene(Scenes.CAR_0);
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
