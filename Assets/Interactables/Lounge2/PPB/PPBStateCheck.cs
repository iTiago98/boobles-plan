using System;
using System.Collections;
using System.Collections.Generic;
using Booble.Flags;
using UnityEngine;

public class PPBStateCheck : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    
    private void Awake()
    {
        if (FlagManager.Instance.GetFlag(Flag.Reference.VictoriaPorDesgasteObtenida))
        {
            _anim.SetTrigger("boom");
        }
    }
}
