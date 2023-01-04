using System;
using System.Collections;
using System.Collections.Generic;
using Booble.Flags;
using Booble.Interactables;
using Booble.Interactables.Dialogues;
using Booble.UI;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DennisWalk : MonoBehaviour
{
        [SerializeField] private Vector3 _finalDennisPos;
        [SerializeField] private Vector3 _cinematicPos;
        [SerializeField] private Dialogue _dialogue;
        [SerializeField] private Fader _fader;
        
        private FlagManager _flagManager;
        private DialogueManager _diagManager;
        // private Animator _dennisAnim;

        private void Awake()
        {
                _diagManager = DialogueManager.Instance;
                _flagManager = FlagManager.Instance;
                
                if (_flagManager.GetFlag(Flag.Reference.DennisMencionaExplosion) &&
                    _flagManager.GetFlag(Flag.Reference.DennisMencionaFamilias))
                {
                        gameObject.SetActive(false);
                        return;
                }

                // _dennisAnim = GetComponent<Animator>();
        }

        public void WalkAway()
        {
                if (!_flagManager.GetFlag(Flag.Reference.DennisMencionaExplosion) ||
                    !_flagManager.GetFlag(Flag.Reference.DennisMencionaFamilias))
                {
                        Interactable.ReturnToDialogue();
                        return;
                }
                
                _diagManager.StartDialogue(_dialogue);
                _diagManager.OnEndDialogue.RemoveAllListeners();
                _diagManager.OnEndDialogue.AddListener(() =>
                {
                        
                        _fader.FadeOut(() =>
                        {
                                gameObject.SetActive(false);
                                transform.position = _cinematicPos;
                                _fader.FadeIn(Interactable.EndInteraction);
                        });
                        
                        // _dennisAnim.SetTrigger("Walk");
                        // transform.DOMove(_finalDennisPos, 2)
                        //         .SetEase(Ease.Linear)
                        //         .OnComplete(() =>
                        //         {
                        //                 transform.position = _cinematicPos;
                        //                 gameObject.SetActive(false);
                        //                 Interactable.EndInteraction();
                        //         });
                });
        }
}