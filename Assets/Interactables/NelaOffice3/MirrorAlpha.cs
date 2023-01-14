using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MirrorAlpha : MonoBehaviour
{
    [SerializeField] private Image _target;

    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    private void Update()
    {
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, _target.color.a);
    }
}
