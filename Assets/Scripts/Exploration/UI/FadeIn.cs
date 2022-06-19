using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Booble.UI
{
	public class FadeIn : MonoBehaviour
	{
        [SerializeField] private Image _fadeScreen;
        [SerializeField] private float _fadeDuration;

        private void Awake()
        {
            _fadeScreen.DOFade(1, _fadeDuration).From();
        }

        public void FadeIn2()
        {
            _fadeScreen.DOFade(0, _fadeDuration);
        }

        public void FadeOut()
        {
            FadeOut(null);
        }

        public void FadeOut(TweenCallback callback)
        {
            _fadeScreen.DOFade(1, _fadeDuration).OnComplete(callback);
        }
    }
}
