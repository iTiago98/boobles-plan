using System.Collections;
using System.Collections.Generic;
using Booble.Interactables;
using Booble.Interactables.Dialogues;
using Booble.Player;
using Booble.UI;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NelaOffice1Animation : MonoBehaviour
{
    [SerializeField] private Controller _nelaController;
    [SerializeField] private FadeIn _fade;
    [SerializeField] private Image _image;
    [SerializeField] private Dialogue _dialogue;
    
    private bool _dialogueEnd;
    private DialogueManager _diagMng;

    private void Awake()
    {
        _diagMng = DialogueManager.Instance;
    }
    
    private void Start()
    {
        StartCoroutine(AnimationCoroutine());
    }

    private IEnumerator AnimationCoroutine()
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

    private void ThrowDialogue(Dialogue diag, List<Option> options = null)
    {
        _dialogueEnd = false;
        options?.ForEach(op => op.DialogueOption.DoOnSelect.AddListener(() => _dialogueEnd = true));
        _diagMng.StartDialogue(diag, options);
        _diagMng.OnEndDialogue.RemoveAllListeners();
        _diagMng.OnEndDialogue.AddListener(() => _dialogueEnd = true);
    }
}
