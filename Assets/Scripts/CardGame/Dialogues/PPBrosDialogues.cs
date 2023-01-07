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

        private bool _paredCardDialogueShown;
        private bool _palaCardDialogueShown;
        private bool _gomuCardDialogueShown;
        private bool _pelotaBombaCardDialogueShown;
        private bool _matarLaBolaCardDialogueShown;

        override public bool CheckDialogue(Card cardPlayed)
        {
            if (cardPlayed.name.Contains("Pared") && !_paredCardDialogueShown)
            {
                ThrowDialogue(_paredCardDialogue);
                _paredCardDialogueShown = true;
                return true;
            }
            else if (cardPlayed.name.Contains("La pala de Ñocobich") && !_palaCardDialogueShown)
            {
                ThrowDialogue(_palaCardDialogue);
                _palaCardDialogueShown = true;
                return true;
            }
            else if (cardPlayed.name.Contains("Gomu Gomu no") && !_gomuCardDialogueShown)
            {
                ThrowDialogue(_gomuCardDialogue);
                _gomuCardDialogueShown = true;
                return true;
            }
            else if (cardPlayed.name.Contains("Pelota bomba") && !_pelotaBombaCardDialogueShown)
            {
                ThrowDialogue(_pelotaBombaCardDialogue);
                _pelotaBombaCardDialogueShown = true;
                return true;
            }
            else if (cardPlayed.name.Contains("Matar la bola") && !_matarLaBolaCardDialogueShown)
            {
                ThrowDialogue(_matarLaBolaCardDialogue);
                _matarLaBolaCardDialogueShown = true;
                return true;
            }

            return false;
        }
    }
}