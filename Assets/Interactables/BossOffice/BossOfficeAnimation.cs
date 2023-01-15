using System.Collections;
using System.Collections.Generic;
using Booble;
using Booble.CardGame;
using Booble.Flags;
using Booble.Interactables;
using Booble.Interactables.Dialogues;
using Booble.Managers;
using Booble.Player;
using Booble.UI;
using DG.Tweening;
using UnityEngine;

public class BossOfficeAnimation : MonoBehaviour
{
    [SerializeField] private Controller _controller;
    [SerializeField] private Fader _fade;
    [SerializeField] private Dialogue _dialogue0;
    [SerializeField] private Dialogue _dialogue1;
    [SerializeField] private Transform _machine;
    [SerializeField] private Transform _finalMachinePos;
    [SerializeField] private Dialogue _dialogue2;
    [SerializeField] private GameObject _emilin;
    [SerializeField] private GameObject _boss;
    [SerializeField] private Dialogue _dialogue3;

    private Camera _cam;
    private bool _dialogueEnd;
    private DialogueManager _diagMng;

    private void Awake()
    {
        _cam = Camera.main;
        _diagMng = DialogueManager.Instance;
    }

    private void Start()
    {
        StartCoroutine(OfficeDialogueCoroutine());
    }

    private IEnumerator OfficeDialogueCoroutine()
    {
        _controller.enabled = false;
        Interactable.ManualInteractionActivation();
        yield return new WaitForSeconds(_fade.FadeDuration);

        ThrowDialogue(_dialogue0);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;

        DeckManager.Instance.SetOpponent(Opponent_Name.Boss);
        SceneLoader.Instance.LoadInterviewScene();
        yield return new WaitUntil(() => !_cam.gameObject.activeSelf);
        yield return new WaitUntil(() => _cam.gameObject.activeSelf);

        ThrowDialogue(_dialogue1);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;
        
        MusicManager.Instance.StopMusic();
       _machine.position = _finalMachinePos.position;
       _machine.rotation = _finalMachinePos.rotation;
       _cam.transform.DOShakePosition(2, 1, 10, 90, false)
           .OnComplete(() => _dialogueEnd = true);
       yield return new WaitUntil(() => _dialogueEnd);
       _dialogueEnd = false;
        
       ThrowDialogue(_dialogue2);
       yield return new WaitUntil(() => _dialogueEnd);
       _dialogueEnd = false;

       if (FlagManager.Instance.GetFlag(Flag.Reference.FinalMalo))
       {
           SceneLoader.Instance.LoadBossOfficeEnding();
           yield break;
       }
       
       _fade.FadeOut();
       yield return new WaitForSeconds(_fade.FadeDuration);

       _emilin.SetActive(true);
       _boss.SetActive(false);
       _fade.FadeIn();
       yield return new WaitForSeconds(_fade.FadeDuration);
       
       ThrowDialogue(_dialogue3);
       yield return new WaitUntil(() => _dialogueEnd);
       _dialogueEnd = false;
       
       SceneLoader.Instance.LoadCanteenEnding();
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
}
