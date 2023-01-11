using System.Collections;
using System.Collections.Generic;
using Booble.CardGame;
using Booble.Flags;
using Booble.Interactables;
using Booble.Interactables.Dialogues;
using Booble.Managers;
using Booble.Player;
using Booble.UI;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NelaOffice1Animation : MonoBehaviour
{
    [SerializeField] private Controller _nelaController;
    [SerializeField] private Fader _fade;
    [SerializeField] private Image _image;
    [SerializeField] private Dialogue _dialogue;
    [SerializeField] private Dialogue _postInterview;
    [SerializeField] private Flag.Reference _day1;
    [SerializeField] private Dialogue _officeDialogue;
    [SerializeField] private List<Option> _options;
    
    private Camera _cam;
    private bool _dialogueEnd;
    private bool _throwAgain;
    private bool _interview;
    private DialogueManager _diagMng;

    private void Awake()
    {
        _diagMng = DialogueManager.Instance;
        _cam = Camera.main;
    }
    
    private void Start()
    {
        // if (!FlagManager.Instance.GetFlag(_day1))
        // {
        //     FlagManager.Instance.SetFlag(_day1);
        //     StartCoroutine(Day1AnimationCoroutine());
        // }
        // else
        // {
            StartCoroutine(OfficeDialogueCoroutine());
        // }
    }

    private IEnumerator Day1AnimationCoroutine()
    {
        _nelaController.enabled = false;
            
        Interactable.ManualInteractionActivation();
        yield return new WaitForSeconds(_fade.FadeDuration+2);

        _image.DOFade(0, 2);
        _image.GetComponentInChildren<TMP_Text>().DOFade(0, 2);
        yield return new WaitForSeconds(2);
        
        ThrowDialogue(_dialogue);
        yield return new WaitUntil(() => _dialogueEnd);
        
        SceneLoader.Instance.LoadLowerHall1();
    }

    private IEnumerator OfficeDialogueCoroutine()
    {
        _nelaController.enabled = false;
            
        Interactable.ManualInteractionActivation();
        _image.gameObject.SetActive(false);
        yield return new WaitForSeconds(_fade.FadeDuration);

        do
        {
            _throwAgain = false;
            ThrowDialogue(_officeDialogue, _options);
            yield return new WaitUntil(() => _dialogueEnd);
            if (_throwAgain || _interview)
            {
                _dialogueEnd = false;
                yield return new WaitUntil(() => _dialogueEnd);
            }
        } while (_throwAgain);

        if (_interview)
        {
            DeckManager.Instance.SetOpponent(Opponent_Name.Citriano);
            SceneLoader.Instance.LoadInterviewScene();
            yield return new WaitUntil(() => !_cam.gameObject.activeSelf);
            yield return new WaitUntil(() => _cam.gameObject.activeSelf);
            
            ThrowDialogue(_postInterview);
            yield return new WaitUntil(() => _dialogueEnd);
            _fade.FadeOut(SceneLoader.Instance.LoadHome1);
        }
        else
        {
            SceneLoader.Instance.LoadLowerHall1();
        }
    }

    private void ThrowDialogue(Dialogue diag, List<Option> options = null)
    {
        _dialogueEnd = false;
        options?.ForEach(op => op.DialogueOption.DoOnSelect.AddListener(() => _dialogueEnd = true));
        _diagMng.StartDialogue(diag, options);
        _diagMng.OnEndDialogue.RemoveAllListeners();
        _diagMng.OnEndDialogue.AddListener(() => _dialogueEnd = true);
    }

    public void ThrowAgain()
    {
        _throwAgain = true;
    }

    public void DialogueEnd()
    {
        _dialogueEnd = true;
    }

    public void Interview()
    {
        _interview = true;
    }
}
