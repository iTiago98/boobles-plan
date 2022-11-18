using Booble.Interactables.Dialogues;
using CardGame.Cards;
using CardGame.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame.Dialogues
{
    public class CitrianoDialogues : InterviewDialogue
    {
        [Header("Extra cards")]
        [SerializeField] private Dialogue _nuevaCepaCardDialogue;
        [SerializeField] private Dialogue _exprimirCardDialogue;
        [SerializeField] private Dialogue _maquinaDeZumosCardDialogue;

        override public void CheckDialogue(Card cardPlayed)
        {
            if (cardPlayed.name.Contains("Nueva cepa del escorbuto")) ThrowDialogue(_nuevaCepaCardDialogue);
            if (cardPlayed.name.Contains("Exprimir la verdad")) ThrowDialogue(_exprimirCardDialogue);
            if (cardPlayed.name.Contains("La máquina de zumo de los supermercados")) ThrowDialogue(_maquinaDeZumosCardDialogue);
        }
    }
}