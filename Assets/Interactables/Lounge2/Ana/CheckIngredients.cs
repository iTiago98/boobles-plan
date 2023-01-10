using System;
using System.Collections;
using System.Collections.Generic;
using Booble.Flags;
using UnityEngine;

public class CheckIngredients : MonoBehaviour
{
    private void Start()
    {
        if (FlagManager.Instance.GetFlag(Flag.Reference.IngredientesObtenidos))
            return;

        if (FlagManager.Instance.GetFlag(Flag.Reference.PajitaObtenida)
            && FlagManager.Instance.GetFlag(Flag.Reference.PolvoraObtenida)
            && FlagManager.Instance.GetFlag(Flag.Reference.GomaObtenida))
        {
            FlagManager.Instance.SetFlag(Flag.Reference.IngredientesObtenidos);
        }
    }
}
