using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Booble.UI
{
    public class AlertImage : MonoBehaviour
    {
        private Sequence _heartbeatSequence;

        private void OnEnable()
        {
            if (_heartbeatSequence == null) SetAlertHeartbeatSequence();
            _heartbeatSequence.Restart();
        }

        private void OnDisable()
        {
            _heartbeatSequence.Pause();
        }

        private void SetAlertHeartbeatSequence()
        {
            RectTransform rt = (RectTransform)transform;
            _heartbeatSequence = DOTween.Sequence();
            _heartbeatSequence.Append(rt.DOSizeDelta(10 * Vector2.one, 0.2f).SetRelative());
            _heartbeatSequence.Append(rt.DOSizeDelta(-10 * Vector2.one, 0.2f).SetRelative());
            _heartbeatSequence.Append(rt.DOSizeDelta(10 * Vector2.one, 0.2f).SetRelative());
            _heartbeatSequence.Append(rt.DOSizeDelta(-10 * Vector2.one, 0.2f).SetRelative());
            _heartbeatSequence.AppendInterval(1f);
            _heartbeatSequence.SetLoops(-1);
            _heartbeatSequence.SetUpdate(true);
            _heartbeatSequence.Pause();
        }
    }
}