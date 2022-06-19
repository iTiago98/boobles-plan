using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Booble.Interactables.Dialogues;
using static Booble.Interactables.Interactable;
using DG.Tweening;

namespace Booble.Interactables.Arcadio
{
	public class ThrowCoinsDialogue : MonoBehaviour
	{
        [SerializeField]
        private enum EndType { Nothing, Return, Close, Callback }
        
        [SerializeField] private Dialogue _dialogue1;
        [SerializeField] private Dialogue _dialogue2;
        [SerializeField] private Dialogue _dialogue3;
        [SerializeField] private Continues _arcCont;
        [SerializeField] private List<AnimatorIdentifier> _animatorIdentifiers;
        [SerializeField] private Collider2D _arcadeCollider;
        
		private DialogueManager _diagManager;
        private Dialogue _dialogue;

        protected virtual void Awake()
        {
            _diagManager = DialogueManager.Instance;
        }

        public void StartInteraction()
        {
            int coinCount = _arcCont.CoinCount;

            switch(coinCount)
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

            _diagManager.StartDialogue(_dialogue, _animatorIdentifiers);
            _diagManager.OnEndDialogue.RemoveAllListeners();

            if(coinCount < 3)
            {
                _diagManager.OnEndDialogue.AddListener(() => Interactable.EndInteraction());
            }
            else
            {
                _diagManager.OnEndDialogue.AddListener(() => WalkToSofa());
            }
        }

        private void WalkToSofa()
        {
            _arcadeCollider.enabled = true;
            transform.parent.DOMoveX(2.5f, 2)
                                .SetEase(Ease.Linear)
                                .SetRelative()
                                .OnComplete(() => Interactable.EndInteraction());
        }
	}
}
