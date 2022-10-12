using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Booble.UI
{
	public class FadeIn : MonoBehaviour
	{
        [field: SerializeField] public float FadeDuration { get; private set; }
        [SerializeField] private Image _fadeScreen;

        private void Awake()
        {
            _fadeScreen.DOFade(1, FadeDuration).From();
        }

        public void FadeIn2()
        {
            _fadeScreen.DOFade(0, FadeDuration);
        }

        public void FadeOut()
        {
            FadeOut(null);
        }

        public void FadeOut(TweenCallback callback)
        {
            _fadeScreen.DOFade(1, FadeDuration).OnComplete(callback);
        }
    }
}
