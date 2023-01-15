using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace Booble.UI
{
	public class Fader : MonoBehaviour
	{
        [field: SerializeField] public float FadeDuration { get; private set; }
        [SerializeField] private Image _fadeScreen;
        [SerializeField] private TextMeshProUGUI _fadeText;

        public void FadeIn()
        {
            _fadeScreen.DOFade(0, FadeDuration).OnComplete(null);
        }

        public void FadeIn(TweenCallback callback)
        {
            _fadeScreen.DOFade(0, FadeDuration).OnComplete(callback);
        }

        public void FadeOut()
        {
            FadeOut(null);
        }

        public void FadeOut(TweenCallback callback)
        {
            _fadeScreen.DOFade(1, FadeDuration).OnComplete(callback);
        }

        public void SetVisible(bool value)
        {
            Color newColor = _fadeScreen.color;

            newColor.a = value ? 1f : 0f;

            _fadeScreen.color = newColor;
        }

        public void SetText(string text)
        {
            if (text == "") return;

            _fadeText.gameObject.SetActive(true);
            _fadeText.text = text;
        }

        public void DisableText()
        {
            _fadeText.gameObject.SetActive(false);
        }

        public bool HasText() { return _fadeText.text != ""; }
    }
}
