using System;
using System.Collections;
using System.Collections.Generic;
using Booble.Flags;
using Booble.Interactables;
using Booble.Interactables.Dialogues;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DennisWalk : MonoBehaviour
{
        [SerializeField] private Vector3 _finalDennisPos;
        [SerializeField] private Dialogue _dialogue;
        
        private FlagManager _flagManager;
        private DialogueManager _diagManager;
        private Animator _dennisAnim;

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

                _dennisAnim = GetComponent<Animator>();
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
                        _dennisAnim.SetTrigger("Walk");
                        transform.DOMove(_finalDennisPos, 2)
                                .SetEase(Ease.Linear)
                                .OnComplete(() =>
                                {
                                        gameObject.SetActive(false);
                                        Interactable.EndInteraction();
                                });
                });
        }
}
