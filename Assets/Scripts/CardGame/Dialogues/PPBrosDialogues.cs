using Booble.Interactables.Dialogues;
using CardGame.Cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame.Dialogues
{
    public class PPBrosDialogues : InterviewDialogue
    {
        [Header("Extra cards")]
        [SerializeField] private Dialogue _paredCardDialogue;
        [SerializeField] private Dialogue _palaCardDialogue;
        [SerializeField] private Dialogue _gomuCardDialogue;

        override public void CheckDialogue(Card cardPlayed)
        {
            if (cardPlayed.name.Contains("Pared")) ThrowDialogue(_paredCardDialogue);
            if (cardPlayed.name.Contains("Pala de Ñocobich")) ThrowDialogue(_palaCardDialogue);
            if (cardPlayed.name.Contains("Gomu gomu no")) ThrowDialogue(_gomuCardDialogue);
        }
    }
}