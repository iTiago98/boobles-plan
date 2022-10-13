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

namespace Booble.Interactables.Queca
{
    public class Canteen0Animation : MonoBehaviour
    {
        private static readonly int WL = Animator.StringToHash("WalkingLeft");
        private static readonly int WR = Animator.StringToHash("WalkingRight");
        
        [Header("Dialogues")]
        [SerializeField] private Dialogue _menuDialogue;
        [SerializeField] private Dialogue _drinksDialogue;
        [SerializeField] private List<Option> _drinksOptions;
        [SerializeField] private Dialogue _orderDialogue;
        [SerializeField] private Dialogue _sitDialogue;
        [SerializeField] private Dialogue _emilin1Dialogue;
        [SerializeField] private Dialogue _supersalmorejoDialogue;
        [SerializeField] private Dialogue _emilin2Dialogue;
        [SerializeField] private Dialogue _finalDialogue;

        [Header("Waypoints")]
        [SerializeField] private Transform _drinks0;
        [SerializeField] private Transform _drinks1;
        [SerializeField] private Transform _order0;
        [SerializeField] private Transform _order1;
        [SerializeField] private Transform _sit0;
        [SerializeField] private Transform _sit1;
        [SerializeField] private Transform _emilin;

        [Header("Animators")]
        [SerializeField] private Animator _nelaAnim;
        [SerializeField] private Animator _quecaAnim;
        [SerializeField] private Animator _emilinAnim;

        [Header("Misc")]
        [SerializeField] private Transform _emilinT;
        [SerializeField] private float _emilinSpawnDuration;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private Controller _nelaController;
        [SerializeField] private FadeIn _fade;
        [SerializeField] private GameObject _food;
        
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
            float quecaMoveDuration = MoveCharacter(_quecaAnim.transform, _drinks0.position);
            _quecaAnim.SetBool(WL, true);
            float nelaMoveDuration = MoveCharacter(_nelaAnim.transform, _drinks1.position);
            yield return new WaitForSeconds(nelaMoveDuration);
            
            _nelaAnim.SetBool(WL, false);
            _quecaAnim.SetBool(WL, false);
            ThrowDialogue(_drinksDialogue, _drinksOptions);
            yield return new WaitUntil(() => _dialogueEnd);
            
            _quecaAnim.SetBool(WL, true);
            quecaMoveDuration = MoveCharacter(_quecaAnim.transform, _order0.position);
            _nelaAnim.SetBool(WL, true);
            nelaMoveDuration = MoveCharacter(_nelaAnim.transform, _order1.position);
            yield return new WaitForSeconds(nelaMoveDuration);
            
            _nelaAnim.SetBool(WL, false);
            ThrowDialogue(_orderDialogue);
            yield return new WaitForSeconds(quecaMoveDuration - nelaMoveDuration);
            
            _quecaAnim.SetBool(WL, false);
            yield return new WaitUntil(() => _dialogueEnd);
            
            _quecaAnim.SetBool(WL, true);
            quecaMoveDuration = MoveCharacter(_quecaAnim.transform, _sit1.position);
            _nelaAnim.SetBool(WL, true);
            nelaMoveDuration = MoveCharacter(_nelaAnim.transform, _sit0.position);
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
            
            // Set Queca and Nela in sit position
            _fade.FadeIn2();
            yield return new WaitForSeconds(_fade.FadeDuration);
            
            ThrowDialogue(_sitDialogue);
            yield return new WaitUntil(() => _dialogueEnd);
            
            _emilinAnim.SetTrigger("Open");
            _emilinAnim.SetBool(WR, true);
            yield return new WaitForSeconds(_emilinSpawnDuration);
            
            float emilinMoveDuration = MoveCharacter(_emilinT, _emilin.position);
            yield return new WaitForSeconds(emilinMoveDuration);
            
            _emilinAnim.SetBool(WR, false);
            ThrowDialogue(_emilin1Dialogue);
            yield return new WaitUntil(() => _dialogueEnd);

            _emilinAnim.SetBool(WR, true);
            emilinMoveDuration = MoveCharacter(_emilinT, _order0.position);
            yield return new WaitForSeconds(emilinMoveDuration);

            _emilinAnim.SetBool(WR, false);
            _food.SetActive(true);
            ThrowDialogue(_supersalmorejoDialogue);
            yield return new WaitUntil(() => _dialogueEnd);

            _emilinAnim.SetBool(WL, true);
            emilinMoveDuration = MoveCharacter(_emilinT, _emilin.position);
            yield return new WaitForSeconds(emilinMoveDuration);
            
            _emilinAnim.SetBool(WL, false);
            ThrowDialogue(_emilin2Dialogue);
            yield return new WaitUntil(() => _dialogueEnd);
            
            _fade.FadeOut();
            yield return new WaitForSeconds(_fade.FadeDuration);
            
            // Set Queca and Nela in standing position
            _food.SetActive(false);
            _emilinAnim.SetTrigger("Reset");
            _fade.FadeIn2();
            yield return new WaitForSeconds(_fade.FadeDuration);
            
            ThrowDialogue(_finalDialogue);
            yield return new WaitUntil(() => _dialogueEnd);

            _quecaAnim.SetBool(WR, true);
            quecaMoveDuration = MoveCharacter(_quecaAnim.transform, _order0.position);
            yield return new WaitForSeconds(quecaMoveDuration);
            
            _quecaAnim.gameObject.SetActive(false);
            _nelaController.enabled = true;
            _nelaController.StopMovement();
            Interactable.EndInteraction();
        }

        private void ThrowDialogue(Dialogue diag, List<Option> options = null)
        {
            _dialogueEnd = false;
            options?.ForEach(op => op.DialogueOption.DoOnSelect.AddListener(() => _dialogueEnd = true));
            _diagMng.StartDialogue(diag, options);
            _diagMng.OnEndDialogue.RemoveAllListeners();
            _diagMng.OnEndDialogue.AddListener(() => _dialogueEnd = true);
        }

        private float MoveCharacter(Transform characterT, Vector3 destination)
        {
            float moveDuration = Vector2.Distance(characterT.position, destination) / _moveSpeed;
            characterT.DOMove(destination, moveDuration).SetEase(Ease.Linear);
            return moveDuration;
        }
    }
}
