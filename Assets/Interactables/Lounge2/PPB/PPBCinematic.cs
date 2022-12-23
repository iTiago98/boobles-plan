using System.Collections;
using System.Collections.Generic;
using Booble.Interactables;
using Booble.Interactables.Dialogues;
using Booble.UI;
using DG.Tweening;
using UnityEngine;

public class PPBCinematic : MonoBehaviour
{
    [SerializeField] private Fader _fader;
    [SerializeField] private Dialogue _dialogue1;
    [SerializeField] private Dialogue _dialogue2;
    [SerializeField] private GameObject _dennis;
    [SerializeField] private Transform _anaT;
    [SerializeField] private Animator _anaAnim;
    [SerializeField] private float _anaXPos;
    
    public void StartCinematic()
    {
        _fader.FadeOut(() =>
        {
            _dennis.SetActive(true);
            _fader.FadeIn(() =>
            {
                DialogueManager.Instance.StartDialogue(_dialogue1);
                DialogueManager.Instance.OnEndDialogue.RemoveAllListeners();
                DialogueManager.Instance.OnEndDialogue.AddListener(() =>
                {
                    _anaT.gameObject.SetActive(true);
                    _anaAnim.SetBool("Walking", true);
                    _anaT.DOMoveX(_anaXPos, 2)
                        .SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            _anaAnim.SetBool("Walking", false);
                            DialogueManager.Instance.StartDialogue(_dialogue2);
                            DialogueManager.Instance.OnEndDialogue.RemoveAllListeners();
                            DialogueManager.Instance.OnEndDialogue.AddListener(() =>
                            {
                                _fader.FadeOut(() =>
                                {
                                    _anaT.gameObject.SetActive(false);
                                    _dennis.SetActive(false);
                                    _fader.FadeIn(() => Interactable.EndInteraction());
                                });
                            });
                        });
                });
            });
        });
    }
    
    
}
