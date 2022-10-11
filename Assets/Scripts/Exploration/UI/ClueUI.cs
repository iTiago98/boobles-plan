using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using Booble.Flags;
using Booble.Interactables;
using DG.Tweening;
using UnityEngine;

namespace Booble.UI
{
    public class ClueUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _panel;
        [SerializeField] private float _closedX;
        [SerializeField] private float _openX;
        [SerializeField] private float _transitionDuration;
        [SerializeField] private KeyCode _openCluesKey;
        [SerializeField] private List<ClueFlag> _clueFlags;

        private ClueState _state = ClueState.Closed;
        
        private void Update()
        {
            if(Interactable.BlockActions && !Interactable.CluesOpen)
                return;
            
            if(!Input.GetKey(_openCluesKey))
                return;

            if(_state == ClueState.Transition)
                return;
            
            switch (_state)
            {
                case ClueState.Open:
                    _panel.DOAnchorPosX(_closedX, _transitionDuration)
                        .OnComplete(() =>
                        {
                            Interactable.CluesOpen = false;
                            _state = ClueState.Closed;
                        });
                    break;
                case ClueState.Closed:
                    UpdateClues();
                    Interactable.CluesOpen = true;
                    _panel.DOAnchorPosX(_openX, _transitionDuration)
                        .OnComplete(() => _state = ClueState.Open);
                    break;
            }

            _state = ClueState.Transition;
        }

        public void UpdateClues()
        {
            foreach (ClueFlag clueFlag in _clueFlags)
            {
                clueFlag.ClueElement.SetActive(FlagManager.Instance.GetFlag(clueFlag.FlagReference));
            }
        }
    }

    public enum ClueState
    {
        Transition,
        Open,
        Closed
    }

    [System.Serializable]
    public class ClueFlag
    {
        [field: SerializeField]
        public GameObject ClueElement { get; private set; }
        [field: SerializeField]
        public Flag.Reference FlagReference { get; private set; }
    }
}
