using System;
using System.Collections;
using System.Collections.Generic;
using Booble.Flags;
using Booble.Interactables;
using DG.Tweening;
using UnityEngine;

public class DennisWalk : MonoBehaviour
{
        [SerializeField] private Vector3 _finalDennisPos;
        
        private Animator _dennisAnim;

        private void Awake()
        { 
                if (FlagManager.Instance.GetFlag(Flag.Reference.DennisMencionaExplosionFamilia))
                {
                        gameObject.SetActive(false);
                        return;
                }
                
                _dennisAnim = GetComponent<Animator>();
        }

        public void WalkAway()
        {
                _dennisAnim.SetTrigger("Walk");
                transform.DOMove(_finalDennisPos, 2)
                        .SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                                gameObject.SetActive(false);
                                Interactable.EndInteraction();
                        });
        }
}
