using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using Booble.Managers;

namespace Booble.UI
{
	public class Fader : MonoBehaviour
	{
        [field: SerializeField] public float FadeDuration { get; private set; }
        [SerializeField] private Image _fadeScreen;
        [SerializeField] private TextMeshProUGUI _fadeText;

        private SceneLoader _sceneLoader;
        public void SetSceneLoader(SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        public void FadeIn()
        {
            FadeIn(null);
        }

        public void FadeIn(TweenCallback callback)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Append(_fadeScreen.DOFade(0, FadeDuration));
            sequence.AppendCallback(callback);
            sequence.AppendCallback(() => _sceneLoader.SetFading(false));
            sequence.Play();
        }

        public void FadeOut()
        {
            FadeOut(null);
        }

        public void FadeOut(TweenCallback callback)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.AppendCallback(() => _sceneLoader.SetFading(true));
            sequence.Append(_fadeScreen.DOFade(1, FadeDuration));
            sequence.AppendCallback(callback);
            sequence.Play();
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
