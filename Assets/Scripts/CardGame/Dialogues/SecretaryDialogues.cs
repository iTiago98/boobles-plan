using Booble.CardGame.Cards;
using Booble.Interactables.Dialogues;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Booble.CardGame.Dialogues
{
    public class SecretaryDialogues : InterviewDialogue
    {
        [Header("Extra cards")]
        [SerializeField] private Dialogue _afidavitCardDialogue;
        [SerializeField] private Dialogue _anexosCorrespondientesCardDialogue;
        [SerializeField] private Dialogue _ordenesDeArribaCardDialogue;
        [SerializeField] private Dialogue _resaltarContradiccionCardDialogue;

        private bool _afidavitCardDialogueShown;
        private bool _anexosCorrespondientesCardDialogueShown;
        private bool _ordenesDeArribaCardDialogueShown;
        private bool _resaltarContradiccionCardDialogueShown;

        override public bool CheckDialogue(Card cardPlayed)
        {
            if (cardPlayed.name.Contains("Afidávit") && !_afidavitCardDialogueShown)
            {
                ThrowDialogue(_afidavitCardDialogue);
                _afidavitCardDialogueShown = true;
                return true;
            }
            else if (cardPlayed.name.Contains("Traigo los anexos correspondientes") && !_anexosCorrespondientesCardDialogueShown)
            {
                ThrowDialogue(_anexosCorrespondientesCardDialogue);
                _anexosCorrespondientesCardDialogueShown = true;
                return true;
            }
            else if (cardPlayed.name.Contains("Órdenes de arriba") && !_ordenesDeArribaCardDialogueShown)
            {
                ThrowDialogue(_ordenesDeArribaCardDialogue);
                _ordenesDeArribaCardDialogueShown = true;
                return true;
            }
            else if (cardPlayed.name.Contains("Resaltar una contradicción") && !_resaltarContradiccionCardDialogueShown)
            {
                ThrowDialogue(_resaltarContradiccionCardDialogue);
                _resaltarContradiccionCardDialogueShown = true;
                return true;
            }

            return false;
        }
    }
}