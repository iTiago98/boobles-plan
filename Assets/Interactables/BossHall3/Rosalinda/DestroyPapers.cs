using System.Collections;
using System.Collections.Generic;
using Booble.Flags;
using UnityEngine;

public class DestroyPapers : MonoBehaviour
{
    public void CheckDestroy()
    {
        if(FlagManager.Instance.GetFlag(Flag.Reference.ResaltarUnaContradiccionObtenida)
           && !FlagManager.Instance.GetFlag(Flag.Reference.PapelesDestruidos))
        {
            FlagManager.Instance.SetFlag(Flag.Reference.PapelesDestruidos);
        }
    }
}
