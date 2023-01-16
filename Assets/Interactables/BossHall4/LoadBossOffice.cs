using System.Collections;
using System.Collections.Generic;
using Booble.Flags;
using Booble.Interactables.Events;
using Booble.Managers;
using Booble.UI;
using UnityEngine;

public class LoadBossOffice : MonoBehaviour
{
    public void Execute()
    {
        FlagManager.Instance.SetFlag(Flag.Reference.BossHall4);
        PauseMenu.Instance.ShowSavedDataText(SceneLoader.Instance.LoadBossOffice);
    }
}
