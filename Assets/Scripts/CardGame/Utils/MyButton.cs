using System;
using DG.Tweening;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Booble.CardGame.Utils
{
    public class MyButton : MonoBehaviour
    {
        public TextMeshProUGUI text;
        public bool _heartbeatEffect;
        
        private Button _button;
        private Sequence _heartbeatSequence;
        private RectTransform _rt;
        
        private void Awake()
        {
            _button = GetComponent<Button>();

            _rt = (RectTransform)transform;
            _heartbeatSequence = DOTween.Sequence();
            _heartbeatSequence.Append(_rt.DOSizeDelta(10 * Vector2.one, 0.2f).SetRelative());
            _heartbeatSequence.Append(_rt.DOSizeDelta(-10 * Vector2.one, 0.2f).SetRelative());
            _heartbeatSequence.Append(_rt.DOSizeDelta(10 * Vector2.one, 0.2f).SetRelative());
            _heartbeatSequence.Append(_rt.DOSizeDelta(-10 * Vector2.one, 0.2f).SetRelative());
            _heartbeatSequence.AppendInterval(1f);
            _heartbeatSequence.SetLoops(-1);
            _heartbeatSequence.Pause();
        }

        public void SetText(string s)
        {
            text.text = s;
        }

        public void SetInteractable(bool b)
        {
            _button.interactable = b;

            if (b && _heartbeatEffect)
            {
                _heartbeatSequence.Restart();
            }
            else if(_heartbeatEffect)
            {
                _heartbeatSequence.Pause();
            }
        }

        public bool IsInteractable()
        {
            return _button.interactable;
        }

        private void OnEnable()
        {
            if (_button.interactable && _heartbeatEffect)
            {
                _heartbeatSequence.Restart();
            }
            else if(_heartbeatEffect)
            {
                _heartbeatSequence.Pause();
            }
        }
    }
}