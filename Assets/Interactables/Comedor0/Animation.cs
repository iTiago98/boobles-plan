using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using Booble.Interactables;
using Booble.Interactables.Dialogues;
using Booble.Player;
using Booble.UI;
using DG.Tweening;
using UnityEngine;

public class Animation : MonoBehaviour
{
    private static readonly int WL = Animator.StringToHash("WalkingLeft");
    
    [Header("Dialogues")]
    [SerializeField] private Dialogue _menuDialogue;
    [SerializeField] private Dialogue _drinksDialogue;
    [SerializeField] private List<Option> _drinksOptions;
    [SerializeField] private Dialogue _orderDialogue;
    [SerializeField] private Dialogue _sitDialogue;

    [Header("Waypoints")]
    [SerializeField] private Transform _drinks0;
    [SerializeField] private Transform _drinks1;
    [SerializeField] private Transform _order0;
    [SerializeField] private Transform _order1;
    [SerializeField] private Transform _sit0;
    [SerializeField] private Transform _sit1;

    [Header("Animators")]
    [SerializeField] private Animator _nelaAnim;
    [SerializeField] private Animator _quecaAnim;

    [Header("Misc")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private Controller _nelaController;
    [SerializeField] private FadeIn _fade;
    
    private DialogueManager _diagMng;

    private bool _dialogueEnd;

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
        yield return new WaitForSeconds(_fade.FadeDuration);
        
        ThrowDialogue(_menuDialogue);
        yield return new WaitUntil(() => _dialogueEnd);

        _nelaAnim.SetBool(WL, true);
        float quecaMoveDuration = MoveCharacter(_quecaAnim, _drinks0.position);
        _quecaAnim.SetBool(WL, true);
        float nelaMoveDuration = MoveCharacter(_nelaAnim, _drinks1.position);
        yield return new WaitForSeconds(nelaMoveDuration);
        
        _nelaAnim.SetBool(WL, false);
        _quecaAnim.SetBool(WL, false);
        ThrowDialogue(_drinksDialogue, _drinksOptions);
        yield return new WaitUntil(() => _dialogueEnd);
        
        _quecaAnim.SetBool(WL, true);
        quecaMoveDuration = MoveCharacter(_quecaAnim, _order0.position);
        _nelaAnim.SetBool(WL, true);
        nelaMoveDuration = MoveCharacter(_nelaAnim, _order1.position);
        yield return new WaitForSeconds(nelaMoveDuration);
        
        _nelaAnim.SetBool(WL, false);
        ThrowDialogue(_orderDialogue);
        yield return new WaitForSeconds(quecaMoveDuration - nelaMoveDuration);
        
        _quecaAnim.SetBool(WL, false);
        yield return new WaitUntil(() => _dialogueEnd);
        
        _quecaAnim.SetBool(WL, true);
        quecaMoveDuration = MoveCharacter(_quecaAnim, _sit0.position);
        _nelaAnim.SetBool(WL, true);
        nelaMoveDuration = MoveCharacter(_nelaAnim, _sit1.position);
        if (quecaMoveDuration < nelaMoveDuration)
        {
            yield return new WaitForSeconds(quecaMoveDuration);
            _quecaAnim.SetBool(WL, false);
            yield return new WaitForSeconds(nelaMoveDuration - quecaMoveDuration);
            _nelaAnim.SetBool(WL, false);
        }
        else
        {
            yield return new WaitForSeconds(nelaMoveDuration);
            _nelaAnim.SetBool(WL, false);
            yield return new WaitForSeconds(quecaMoveDuration - nelaMoveDuration);
            _quecaAnim.SetBool(WL, false);
        }

        _fade.FadeOut();
        yield return new WaitForSeconds(_fade.FadeDuration);
        
        _fade.FadeIn2();
        yield return new WaitForSeconds(_fade.FadeDuration);
        
        ThrowDialogue(_sitDialogue);
        yield return new WaitUntil(() => _dialogueEnd);
        
        Debug.Log("END");
    }

    private void ThrowDialogue(Dialogue diag, List<Option> options = null)
    {
        _dialogueEnd = false;
        _diagMng.StartDialogue(diag, options);
        _diagMng.OnEndDialogue.RemoveAllListeners();
        _diagMng.OnEndDialogue.AddListener(() => _dialogueEnd = true);
    }

    private float MoveCharacter(Animator characterAnim, Vector3 destination)
    {
        float moveDuration = Vector2.Distance(characterAnim.transform.position, destination) / _moveSpeed;
        characterAnim.transform.DOMove(destination, moveDuration).SetEase(Ease.Linear);
        return moveDuration;
    }
}
