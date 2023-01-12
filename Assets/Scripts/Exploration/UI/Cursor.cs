using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Santi.Utils;
using TMPro;

namespace Booble.UI
{
	public class Cursor : Singleton<Cursor>
	{
		private enum CursorText { None, Approach, Interact }

		private static string _approachString = "Acercarse";
		private static string _interactString = "Interactuar";

		[SerializeField] private RectTransform _cursorT;
		[SerializeField] private RectTransform _backgroundT;
		[SerializeField] private Vector2 _defaultAnchoredPosition;
		[SerializeField] private Vector2 _alternateAnchoredPosition;

		private CanvasScaler _canvasScaler;
		private TextMeshProUGUI _actionText;
		private CursorText _currentCursorText = CursorText.None;

		private void Awake()
		{
			UnityEngine.Cursor.lockState = CursorLockMode.Confined;
			_canvasScaler = _cursorT.GetComponentInParent<CanvasScaler>();
			_actionText = _cursorT.GetComponentInChildren<TextMeshProUGUI>();
			_backgroundT.gameObject.SetActive(false);
			SetApproachText();
		}

		private void Update()
		{
			_cursorT.anchoredPosition = new Vector2
			(
				Input.mousePosition.x.Map(0, Screen.width, -_canvasScaler.referenceResolution.x / 2, _canvasScaler.referenceResolution.x / 2),
				Input.mousePosition.y.Map(0, Screen.height, -_canvasScaler.referenceResolution.y / 2, _canvasScaler.referenceResolution.y / 2)
			);

			float cursorWidth = _cursorT.sizeDelta.x.Map(0,
				_canvasScaler.referenceResolution.x, 0, Screen.width);
			float backgroundWidth = _backgroundT.sizeDelta.x.Map(0,
				_canvasScaler.referenceResolution.x, 0, Screen.width);
			float offset = _defaultAnchoredPosition.x.Map(0,
				_canvasScaler.referenceResolution.x, 0, Screen.width);
			float x = _cursorT.position.x + cursorWidth/2 + offset + backgroundWidth/2;
			
			_backgroundT.anchoredPosition = x < Screen.width ? _defaultAnchoredPosition : _alternateAnchoredPosition;
		}

		public void SetApproachText()
        {
			if(_currentCursorText != CursorText.Approach)
            {
				_actionText.text = _approachString;
				_currentCursorText = CursorText.Approach;
			}
        }

		public void SetInteractText()
        {
			if(_currentCursorText != CursorText.Interact)
            {
				_actionText.text = _interactString;
				_currentCursorText = CursorText.Interact;
            }
        }

		public void ShowActionText(bool isShown)
        {
	        _backgroundT.gameObject.SetActive(isShown);
        }
    }
}
