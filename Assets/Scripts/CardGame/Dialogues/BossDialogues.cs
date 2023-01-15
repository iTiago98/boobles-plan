using Booble.CardGame.Cards;
using Booble.Interactables.Dialogues;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Booble.CardGame.Dialogues
{
    public class BossDialogues : InterviewDialogue
    {
        [Header("Extra cards")]
        [SerializeField] private Dialogue _llevarseElMeritoCardDialogue;
        [SerializeField] private Dialogue _hipervitaminadoCardDialogue;
        [SerializeField] private Dialogue _victoriaPorDesgasteCardDialogue;
        [SerializeField] private Dialogue _haPerdidoUsteLosPapeleCardDialogue;

        private bool _llevarseElMeritoCardDialogueShown;
        private bool _hipervitaminadoCardDialogueShown;
        private bool _victoriaPorDesgasteCardDialogueShown;
        private bool _haPerdidoUsteLosPapeleCardDialogueShown;

        override public bool CheckDialogue(Card cardPlayed)
        {
            if (cardPlayed.name.Contains("Llevarse el mérito") && !_llevarseElMeritoCardDialogueShown)
            {
                ThrowDialogue(_llevarseElMeritoCardDialogue);
                _llevarseElMeritoCardDialogueShown = true;
                return true;
            }
            else if (cardPlayed.name.Contains("Hipervitaminado") && !_hipervitaminadoCardDialogueShown)
            {
                ThrowDialogue(_hipervitaminadoCardDialogue);
                _hipervitaminadoCardDialogueShown = true;
                return true;
            }
            else if (cardPlayed.name.Contains("Victoria por desgaste") && !_victoriaPorDesgasteCardDialogueShown)
            {
                ThrowDialogue(_victoriaPorDesgasteCardDialogue);
                _victoriaPorDesgasteCardDialogueShown = true;
                return true;
            }
            else if (cardPlayed.name.Contains("Ha perdido usté los papele") && !_haPerdidoUsteLosPapeleCardDialogueShown)
            {
                ThrowDialogue(_haPerdidoUsteLosPapeleCardDialogue);
                _haPerdidoUsteLosPapeleCardDialogueShown = true;
                return true;
            }

            return false;
        }
    }
}