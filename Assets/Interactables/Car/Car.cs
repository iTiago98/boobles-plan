using System;
using System.Collections;
using System.Collections.Generic;
using Booble.Flags;
using Booble.Interactables;
using Booble.Interactables.Dialogues;
using Booble.Managers;
using Booble.Player;
using Booble.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Booble.Animations
{
    public class Car : MonoBehaviour
    {
        [SerializeField] private Controller _nelaController;
        [SerializeField] private Fader _fade;
        [SerializeField] private List<Pair> _dialogues;

        [SerializeField] private bool _car;

        private bool _dialogueEnd;

        private void Start()
        {
            int i = 0;
            while (i < _dialogues.Count && FlagManager.Instance.GetFlag(_dialogues[i].FlagRef))
            {
                i++;
            }

            if (i < _dialogues.Count)
            {
                // FlagManager.Instance.SetFlag(_dialogues[i].FlagRef);
                _fade.SetText(_dialogues[i].Title);
                StartCoroutine(DialogueCoroutine(_dialogues[i].Content, _dialogues[i].Scene, i==0));
            }
        }

        private IEnumerator DialogueCoroutine(Dialogue dialogue, string scene, bool habemusPartida)
        {
            _nelaController.enabled = false;
            
            Interactable.ManualInteractionActivation();

            if (_car || SceneLoader.Instance.PreviousScene == Scenes.MAIN_MENU)
            {
                if (_fade.HasText())
                {
                    _fade.SetVisible(true);
                    yield return new WaitForSeconds(_fade.FadeDuration);

                    yield return new WaitForSeconds(3);

                    _fade.FadeIn(_fade.DisableText);
                }
            }
            
            _fade.DisableText();

            yield return new WaitForSeconds(_fade.FadeDuration);

            ThrowDialogue(dialogue);
            yield return new WaitUntil(() => _dialogueEnd);

            if (habemusPartida)
            {
                FlagManager.Instance.SetFlag(Flag.Reference.HabemusPartida);
            }
            SceneLoader.Instance.LoadScene(scene);
        }
        
        private void ThrowDialogue(Dialogue diag, List<Option> options = null)
        {
            _dialogueEnd = false;
            options?.ForEach(op => op.DialogueOption.DoOnSelect.AddListener(() => _dialogueEnd = true));
            DialogueManager.Instance.StartDialogue(diag, options);
            DialogueManager.Instance.OnEndDialogue.RemoveAllListeners();
            DialogueManager.Instance.OnEndDialogue.AddListener(() => _dialogueEnd = true);
        }
        
        [System.Serializable]
        private class Pair
        {
            [field: SerializeField] public Flag.Reference FlagRef { get; set; }
            [field: SerializeField] public Dialogue Content { get; set; }
            [field: SerializeField] public string Scene { get; set; }
            [field: SerializeField, TextArea] public string Title { get; set; }
        }
    }
}
