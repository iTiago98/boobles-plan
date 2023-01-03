using System.Collections;
using System.Collections.Generic;
using Booble.CardGame;
using Booble.Interactables;
using Booble.Interactables.Dialogues;
using Booble.Managers;
using Booble.Player;
using Booble.UI;
using UnityEngine;

public class NelaOffice2Animation : MonoBehaviour
{
    [SerializeField] private Controller _nelaController;
    [SerializeField] private Fader _fade;
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
        StartCoroutine(OfficeDialogueCoroutine());
    }

    private IEnumerator OfficeDialogueCoroutine()
    {
        _nelaController.enabled = false;
            
        Interactable.ManualInteractionActivation();
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
            DeckManager.Instance.SetOpponent(Opponent_Name.PingPongBros);
            SceneLoader.Instance.LoadInterviewScene();
            yield return new WaitUntil(() => !_cam.gameObject.activeSelf);
            yield return new WaitUntil(() => _cam.gameObject.activeSelf);
            SceneLoader.Instance.LoadMainMenuScene();
        }
        else
        {
            SceneLoader.Instance.LoadLowerHall2();
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
