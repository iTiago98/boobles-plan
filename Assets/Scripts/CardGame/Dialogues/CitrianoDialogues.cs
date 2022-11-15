using Booble.Interactables.Dialogues;
using CardGame.Cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame.Dialogues
{
    public class CitrianoDialogues : InterviewDialogue
    {
        [SerializeField] private Dialogue _maquinaDeZumosCardDialogue;

        override public void CheckDialogue(Card cardPlayed) 
        {
            if (cardPlayed.name.Contains("Premisa")) ThrowDialogue(_maquinaDeZumosCardDialogue);
        }
    }
}