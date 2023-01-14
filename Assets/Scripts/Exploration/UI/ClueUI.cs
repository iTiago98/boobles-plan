using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using Booble.Flags;
using Booble.Interactables;
using DG.Tweening;
using TMPro;
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
        [SerializeField] private List<ClueList> clues;

        [Serializable]
        private struct ClueList
        {
            public string name;
            public GameObject panel;
            public List<ClueFlag> clues;
        }

        private ClueList _currentClues;
        private ClueState _state = ClueState.Closed;

        private void Update()
        {
            if (Interactable.BlockActions && !Interactable.CluesOpen)
                return;

            if (!Input.GetKey(_openCluesKey))
                return;

            if (_state == ClueState.Transition)
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

        public void SetCluesPanel()
        {
            if (_currentClues.panel != null) _currentClues.panel.SetActive(false);

            List<Flag.Reference> flags = new List<Flag.Reference>() { Flag.Reference.Day1, Flag.Reference.Day2, Flag.Reference.Day3 };

            for (int i = 0; i < flags.Count; i++)
            {
                Flag.Reference flag = flags[i];
                if (!FlagManager.Instance.GetFlag(flag))
                {
                    _currentClues = clues[i];
                    break;
                }
            }

            _currentClues.panel.SetActive(true);
        }

        public void UpdateClues()
        {
            SetCluesPanel();

            foreach (ClueFlag clueFlag in _currentClues.clues)
            {
                bool flagStart = FlagManager.Instance.GetFlag(clueFlag.FlagReferenceStart);
                clueFlag.ClueElement.SetActive(flagStart);

                if (flagStart && FlagManager.Instance.GetFlag(clueFlag.FlagReferenceEnd))
                {
                    clueFlag.ClueElement.GetComponent<TextMeshProUGUI>().color = Color.grey;
                }
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
        public Flag.Reference FlagReferenceStart { get; private set; }
        [field: SerializeField]
        public Flag.Reference FlagReferenceEnd { get; private set; }
    }
}
