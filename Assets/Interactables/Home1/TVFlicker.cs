using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class TVFlicker : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Vector2 _alphaLimits;
    [SerializeField] private Vector2 _durationLimits;

    private void Awake()
    {
        NextFlicker();
    }

    private void NextFlicker()
    {
        float nextAlpha = Random.Range(_alphaLimits.x, _alphaLimits.y);
        float nextDuration = Random.Range(_durationLimits.x, _durationLimits.y);
        _renderer.DOFade(nextAlpha, nextDuration).SetEase(Ease.InOutSine);
        Invoke(nameof(NextFlicker), nextDuration);
    }
}
