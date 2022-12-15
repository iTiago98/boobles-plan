using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Booble.Interactables.Dialogues;
using static Booble.Interactables.Interactable;
using DG.Tweening;
using Booble.Flags;

namespace Booble.Interactables.Arcadio
{
	public class ThrowCoinsDialogue : MonoBehaviour
	{
        [SerializeField]
        private enum EndType { Nothing, Return, Close, Callback }
        
        [SerializeField] private Dialogue _dialogue1;
        [SerializeField] private Dialogue _dialogue2;
        [SerializeField] private Dialogue _dialogue3;
        [SerializeField] private Flag.Reference _coin1;
        [SerializeField] private Flag.Reference _coin2;
        [SerializeField] private Flag.Reference _coin3;
        //[SerializeField] private Continues _arcCont;
        [SerializeField] private Animator _arcadioAnim;
        [SerializeField] private Collider2D _arcadeCollider;
        [SerializeField] private Vector3 _finalArcadioPos;
        
		private DialogueManager _diagManager;
        private Dialogue _dialogue;

        protected virtual void Awake()
        {
            _diagManager = DialogueManager.Instance;
        }

        public void StartInteraction()
        {
            int coinCount = 0;
            if (FlagManager.Instance.GetFlag(Flag.Reference.MonedaPinPonBrosEntregada))
            {
                coinCount++;
            }
            if (FlagManager.Instance.GetFlag(Flag.Reference.MonedaSofasEntregada))
            {
                coinCount++;
            }
            if (FlagManager.Instance.GetFlag(Flag.Reference.MonedaMaquinaCafesEntregada))
            {
                coinCount++;
            }

            switch (coinCount)
            {
                case 1:
                    _dialogue = _dialogue1;
                    break;
                case 2:
                    _dialogue = _dialogue2;
                    break;
                case 3:
                    _dialogue = _dialogue3;
                    break;
            }

            _diagManager.StartDialogue(_dialogue);
            _diagManager.OnEndDialogue.RemoveAllListeners();

            if(coinCount < 3)
            {
                _diagManager.OnEndDialogue.AddListener(() => Interactable.EndInteraction());
            }
            else
            {
                _diagManager.OnEndDialogue.AddListener(() => WalkToSofa());
                FlagManager.Instance.SetFlag(Flag.Reference.AllCoins);
            }
        }

        private void WalkToSofa()
        {
            _arcadioAnim.SetTrigger("Walk");
            _arcadeCollider.enabled = true;
            transform.parent.DOMove(_finalArcadioPos, 2)
                                .SetEase(Ease.Linear)
                                .OnComplete(() =>
                                {
                                    _arcadioAnim.SetTrigger("Wait");
                                    Interactable.EndInteraction();
                                });
        }
	}
}
