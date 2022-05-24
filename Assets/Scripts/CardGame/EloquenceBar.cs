using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EloquenceBar : MonoBehaviour
{
    public Image playerEloquenceImage;
    public Image opponentEloquenceImage;

    public void UpdateEloquence(int playerEloquence, int opponentEloquence)
    {
        int maxEloquence = playerEloquence + opponentEloquence;

        playerEloquenceImage.fillAmount = (float)playerEloquence / maxEloquence;
        opponentEloquenceImage.fillAmount = (float)opponentEloquence / maxEloquence;
    }
}
