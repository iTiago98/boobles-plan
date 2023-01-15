using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MirrorAlpha : MonoBehaviour
{
    [SerializeField] private Image _target;

    private Image _image;
    private TMP_Text _text;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _text = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (_image != null) _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, _target.color.a);
        if (_text != null) _text.color = new Color(_text.color.r, _text.color.g, _text.color.b, _target.color.a);
    }
}
