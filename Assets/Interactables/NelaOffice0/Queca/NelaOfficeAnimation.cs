using System;
using System.Collections;
using System.Collections.Generic;
using Booble.CardGame;
using Booble.Characters;
using Booble.Interactables.Dialogues;
using Booble.Managers;
using Booble.Player;
using Booble.UI;
using UnityEngine;

namespace Booble.Interactables.Queca
{
    public class NelaOfficeAnimation : MonoBehaviour
    {
        [SerializeField] private Dialogue _beforeInterviewDialogue;
        [SerializeField] private Dialogue _afterInterviewDialogue;
        [SerializeField] private Fader _fade;
        [SerializeField] private Controller _nelaController;

        private Camera _cam;
        private bool _dialogueEnd;
        private DialogueManager _diagMng;

        private void Awake()
        {
            _diagMng = DialogueManager.Instance;
            _cam = Camera.main;
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
            
            ThrowDialogue(_beforeInterviewDialogue);
            yield return new WaitUntil(() => _dialogueEnd);

            DeckManager.Instance.SetOpponent(CardGame.Opponent_Name.Tutorial);
            SceneLoader.Instance.LoadInterviewScene();
            yield return new WaitUntil(() => !_cam.gameObject.activeSelf);
            yield return new WaitUntil(() => _cam.gameObject.activeSelf);

            ThrowDialogue(_afterInterviewDialogue);
            yield return new WaitUntil(() => _dialogueEnd);
            SceneLoader.Instance.LoadCanteenScene0();
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
}
