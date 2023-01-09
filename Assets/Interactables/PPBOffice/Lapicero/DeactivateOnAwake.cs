using System;
using System.Collections;
using System.Collections.Generic;
using Booble.Flags;
using UnityEngine;

public class DeactivateOnAwake : MonoBehaviour
{
    [SerializeField] private Flag.Reference _flag;

    private void Awake()
    {
        if (FlagManager.Instance.GetFlag(_flag))
        {
            gameObject.SetActive(false);
        }
    }
}
