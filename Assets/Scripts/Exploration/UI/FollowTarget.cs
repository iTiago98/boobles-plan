using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Santi.Utils;

namespace Booble.UI
{
	public class FollowTarget : MonoBehaviour
	{
		[SerializeField] private Transform _target;
		[SerializeField] private Vector2 _offset;

        private Camera _cam;
        private RectTransform _rectTransform;
        private CanvasScaler _canvasScaler;

        private void Awake()
        {
            _cam = Camera.main;
            _rectTransform = (RectTransform)transform;
            _canvasScaler = GetComponentInParent<CanvasScaler>();

            // Vector2 playerScreenPosition = _cam.WorldToScreenPoint(_target.position);
            // _offset = _rectTransform.anchoredPosition - new Vector2(
            //     playerScreenPosition.x.Map(0, Screen.width, -_canvasScaler.referenceResolution.x / 2, _canvasScaler.referenceResolution.x / 2),
            //     playerScreenPosition.y.Map(0, Screen.height, -_canvasScaler.referenceResolution.y / 2, _canvasScaler.referenceResolution.y / 2)
            // );

            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            Vector2 offset = _offset;

            Vector2 playerScreenPosition = _cam.WorldToScreenPoint(_target.position);

            float aux = playerScreenPosition.x.Map(0, Screen.width, 0, _canvasScaler.referenceResolution.x);
            if (aux + _rectTransform.sizeDelta.x + 100 > _canvasScaler.referenceResolution.x)
            {
                offset.x = -offset.x;
            }
                
            _rectTransform.anchoredPosition = offset + new Vector2
            (
                playerScreenPosition.x.Map(0, Screen.width, -_canvasScaler.referenceResolution.x / 2, _canvasScaler.referenceResolution.x / 2),
                playerScreenPosition.y.Map(0, Screen.height, -_canvasScaler.referenceResolution.y / 2, _canvasScaler.referenceResolution.y / 2)
            );
        }
    }
}
