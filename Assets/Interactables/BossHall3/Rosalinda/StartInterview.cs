using System.Collections;
using System.Collections.Generic;
using Booble.Interactables.Dialogues;
using Booble.Interactables.Events;
using Booble.CardGame;
using Booble.Flags;
using Booble.Managers;
using Booble.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartInterview : DialogueEvent
{
    [SerializeField] private Dialogue _postInterview;
    [SerializeField] private Dialogue _postInterviewWinCon;
    [SerializeField] private Fader _fade;
    [SerializeField] private GameObject _rosalinda;
    
    private Camera _cam;
    private bool _dialogueEnd;
    private DialogueManager _diagMng;
    
    private void Awake()
    {
        _diagMng = DialogueManager.Instance;
        _cam = Camera.main;
    }
    
    public override void Execute()
    {
        StartCoroutine(Animation());
    }

    private IEnumerator Animation()
    {
        DeckManager.Instance.SetOpponent(Opponent_Name.Secretary);
        SceneLoader.Instance.LoadInterviewScene();
        yield return new WaitUntil(() => !_cam.gameObject.activeSelf);
        yield return new WaitUntil(() => _cam.gameObject.activeSelf);

        _rosalinda.GetComponent<Collider2D>().enabled = false;
        _rosalinda.GetComponent<SpriteRenderer>().enabled = false;
        FlagManager.Instance.SetFlag(Flag.Reference.Day3);
        yield return new WaitForSeconds(_fade.FadeDuration);

        if (FlagManager.Instance.GetFlag(Flag.Reference.SecretaryVictoriaAlternativa))
        {
            ThrowDialogue(_postInterviewWinCon);
        }
        else
        {
            ThrowDialogue(_postInterview);
        }
        
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;
        
        PauseMenu.Instance.ShowSavedDataText(() => _dialogueEnd = true);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;
        
        SceneLoader.Instance.LoadNelaOfficeDayStart();
    }
    
    private void ThrowDialogue(Dialogue diag, List<Option> options = null)
    {
        _dialogueEnd = false;
        options?.ForEach(op => op.DialogueOption.DoOnSelect.AddListener(() => _dialogueEnd = true));
        _diagMng.StartDialogue(diag, options);
        _diagMng.OnEndDialogue.RemoveAllListeners();
        _diagMng.OnEndDialogue.AddListener(() => _dialogueEnd = true);
    }
}
