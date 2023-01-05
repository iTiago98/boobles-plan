using Booble.CardGame.Cards;
using Booble.Interactables.Dialogues;
using UnityEngine;

namespace Booble.CardGame.Dialogues
{
    public class PPBrosDialogues : InterviewDialogue
    {
        [Header("Extra cards")]
        [SerializeField] private Dialogue _paredCardDialogue;
        [SerializeField] private Dialogue _palaCardDialogue;
        [SerializeField] private Dialogue _gomuCardDialogue;
        [SerializeField] private Dialogue _pelotaBombaCardDialogue;
        [SerializeField] private Dialogue _matarLaBolaCardDialogue;

        override public void CheckDialogue(Card cardPlayed)
        {
            if (cardPlayed.name.Contains("Pared")) ThrowDialogue(_paredCardDialogue);
            if (cardPlayed.name.Contains("La pala de Ñocobich")) ThrowDialogue(_palaCardDialogue);
            if (cardPlayed.name.Contains("Gomu Gomu no")) ThrowDialogue(_gomuCardDialogue);
            if (cardPlayed.name.Contains("Pelota bomba")) ThrowDialogue(_pelotaBombaCardDialogue);
            if (cardPlayed.name.Contains("Matar la bola")) ThrowDialogue(_matarLaBolaCardDialogue);
        }
    }
}