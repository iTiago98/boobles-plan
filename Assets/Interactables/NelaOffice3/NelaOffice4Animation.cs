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
using UnityEngine;
using UnityEngine.UI;

public class NelaOffice4Animation : MonoBehaviour
{
    [SerializeField] private Controller _nelaController;
    [SerializeField] private Fader _fade;
    
    [Header("Pre-flashback")]
    [SerializeField] private Dialogue _dialogue0;
    [SerializeField] private List<Option> _options0;
    [SerializeField] private Dialogue _dialogue1;
    
    [Header("Flashback")]
    [SerializeField] private Dialogue _flashback0;
    [SerializeField] private Dialogue _flashback1;
    [SerializeField] private Dialogue _flashback2;
    [SerializeField] private Dialogue _flashback3;
    [SerializeField] private Dialogue _flashback4;
    [SerializeField] private Dialogue _flashback5;
    [SerializeField] private Dialogue _flashback6;
    [SerializeField] private Dialogue _flashback7;
    [SerializeField] private Dialogue _flashback8;
    [SerializeField] private Dialogue _flashback9;
    [SerializeField] private RectTransform _dialogueBox;
    [SerializeField] private GameObject _slideShow;
    [SerializeField] private Image _blackBG;
    [SerializeField] private Image _romu;
    [SerializeField] private Image _censoredRomu;
    [SerializeField] private RectTransform _finalRomu;
    [SerializeField] private Image _lab;
    [SerializeField] private Image _ana;
    [SerializeField] private Image _jose;
    [SerializeField] private Image _agapito;
    [SerializeField] private Image _office;
    [SerializeField] private Image _oldBoss;
    [SerializeField] private Image _ana2;
    [SerializeField] private Image _jose2;
    [SerializeField] private Image _agapito2;
    [SerializeField] private Image _red;
    [SerializeField] private Image _blackBG2;
    [SerializeField] private Image _censoredRomu2;
    [SerializeField] private Image _finalRomu2;

    [Header("Post-flashback")]
    [SerializeField] private Dialogue _dialoguePF0;
    [SerializeField] private Dialogue _dialoguePF1;
    [SerializeField] private Dialogue _dialoguePF2;
    [SerializeField] private Dialogue _dialoguePF3;
    [SerializeField] private Dialogue _dialoguePF4;
    [SerializeField] private Dialogue _dialoguePF5;
    [SerializeField] private Dialogue _dialoguePF6;
    [SerializeField] private Dialogue _dialoguePF7;
    [SerializeField] private Dialogue _dialoguePF8;
    [SerializeField] private Dialogue _dialoguePF9;
    [SerializeField] private Dialogue _dialoguePF10;
    
    private bool _dialogueEnd;
    private DialogueManager _diagMng;

    private void Awake()
    {
        _diagMng = DialogueManager.Instance;
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
        
        ThrowDialogue(_dialogue0, _options0);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;
        
        ThrowDialogue(_dialogue1);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;
        
        _fade.FadeOut();
        yield return new WaitForSeconds(_fade.FadeDuration);

        _slideShow.SetActive(true);
        float originalY = _dialogueBox.anchoredPosition.y;
        _dialogueBox.anchoredPosition = new Vector2(_dialogueBox.anchoredPosition.x, -750);
        _fade.FadeIn();
        _romu.rectTransform.DOAnchorPos(_finalRomu.anchoredPosition, 120);
        _romu.rectTransform.DOSizeDelta(_finalRomu.sizeDelta, 120);
        _censoredRomu.rectTransform.DOAnchorPos(_finalRomu.anchoredPosition, 120);
        _censoredRomu.rectTransform.DOSizeDelta(_finalRomu.sizeDelta, 120);
        yield return new WaitForSeconds(_fade.FadeDuration);
        
        ThrowDialogue(_flashback0);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;

        _romu.DOFade(1, 5);
        ThrowDialogue(_flashback1);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;

        _lab.DOFade(1, 5);
        ThrowDialogue(_flashback2);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;

        ThrowDialogue(_flashback3);
        _ana.DOFade(1, 5);
        yield return new WaitForSeconds(2);
        _jose.DOFade(1, 5);
        yield return new WaitForSeconds(4);
        _agapito.DOFade(1, 5);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;
        
        ThrowDialogue(_flashback4);
        _lab.DOColor(new Color(.2f, .2f, .2f), 30);
        _ana.DOColor(new Color(.2f, .2f, .2f), 30);
        _jose.DOColor(new Color(.2f, .2f, .2f), 30);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;
        
        ThrowDialogue(_flashback5);
        _office.DOFade(1, 5);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;
        
        ThrowDialogue(_flashback6);
        _oldBoss.DOFade(1, 3);
        yield return new WaitForSeconds(2);
        _ana2.DOFade(1, 3);
        yield return new WaitForSeconds(1);
        _jose2.DOFade(1, 3);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;
        
        ThrowDialogue(_flashback7);
        _agapito2.DOFade(1, 5);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;
        
        ThrowDialogue(_flashback8);
        _red.DOFade(1, 20);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;
        
        ThrowDialogue(_flashback9);
        //_blackBG2.DOFade(1, 5);
        _censoredRomu2.DOFade(1, 5);
        _censoredRomu2.rectTransform.DOAnchorPos(_finalRomu2.rectTransform.anchoredPosition, 120);
        _censoredRomu2.rectTransform.DOSizeDelta(_finalRomu2.rectTransform.sizeDelta, 120);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;
        
        _fade.FadeOut();
        yield return new WaitForSeconds(_fade.FadeDuration);
        
        _slideShow.SetActive(false);
        _dialogueBox.anchoredPosition = new Vector2(_dialogueBox.anchoredPosition.x, originalY);
        _fade.FadeIn();
        yield return new WaitForSeconds(_fade.FadeDuration);
        
        ThrowDialogue(_dialoguePF0);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;

        if (!FlagManager.Instance.GetFlag(Flag.Reference.CitrianoVictoriaAlternativa)
            || !FlagManager.Instance.GetFlag(Flag.Reference.PPBVictoriaAlternativa)
            || !FlagManager.Instance.GetFlag(Flag.Reference.SecretaryVictoriaAlternativa))
        {
            SceneLoader.Instance.LoadLoungeScene4();
            yield break;
        }
        
        ThrowDialogue(_dialoguePF1);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;
        ThrowDialogue(_dialoguePF2);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;
        ThrowDialogue(_dialoguePF3);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;
        ThrowDialogue(_dialoguePF4);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;
        ThrowDialogue(_dialoguePF5);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;
        ThrowDialogue(_dialoguePF6);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;
        ThrowDialogue(_dialoguePF7);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;
        ThrowDialogue(_dialoguePF8);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;
        ThrowDialogue(_dialoguePF9);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;
        ThrowDialogue(_dialoguePF10);
        yield return new WaitUntil(() => _dialogueEnd);
        _dialogueEnd = false;
        
        SceneLoader.Instance.LoadLoungeScene4();
    }

    private void ThrowDialogue(Dialogue diag, List<Option> options = null)
    {
        _dialogueEnd = false;
        options?.ForEach(op => op.DialogueOption.DoOnSelect.AddListener(() => _dialogueEnd = true));
        _diagMng.StartDialogue(diag, options);
        _diagMng.OnEndDialogue.RemoveAllListeners();
        _diagMng.OnEndDialogue.AddListener(() => _dialogueEnd = true);
    }

    public void DialogueEnd()
    {
        _dialogueEnd = true;
    }
}
