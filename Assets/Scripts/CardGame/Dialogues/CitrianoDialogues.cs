using Booble.CardGame.Cards;
using Booble.Interactables.Dialogues;
using UnityEngine;

namespace Booble.CardGame.Dialogues
{
    public class CitrianoDialogues : InterviewDialogue
    {
        [Header("Extra cards")]
        [SerializeField] private Dialogue _nuevaCepaCardDialogue;
        [SerializeField] private Dialogue _exprimirCardDialogue;
        [SerializeField] private Dialogue _maquinaDeZumosCardDialogue;
        [SerializeField] private Dialogue _5PiezasCardDialogue;

        private bool _nuevaCepaCardDialogueShown;
        private bool _exprimirCardDialogueShown;
        private bool _maquinaDeZumosCardDialogueShown;
        private bool _5PiezasCardDialogueShown;

        override public void CheckDialogue(Card cardPlayed)
        {
            if (cardPlayed.name.Contains("Nueva cepa del escorbuto") && !_nuevaCepaCardDialogueShown)
            {
                ThrowDialogue(_nuevaCepaCardDialogue);
                _nuevaCepaCardDialogueShown = true;
            }
            if (cardPlayed.name.Contains("Exprimir la verdad") && !_exprimirCardDialogueShown)
            {
                ThrowDialogue(_exprimirCardDialogue);
                _exprimirCardDialogueShown = true;
            }
            if (cardPlayed.name.Contains("La máquina de zumo de los supermercados") && !_maquinaDeZumosCardDialogueShown)
            {
                ThrowDialogue(_maquinaDeZumosCardDialogue);
                _maquinaDeZumosCardDialogueShown = true;
            }
            if (cardPlayed.name.Contains("5 piezas de fruta al día") && !_5PiezasCardDialogueShown)
            {
                ThrowDialogue(_5PiezasCardDialogue);
                _5PiezasCardDialogueShown = true;
            }
        }
    }
}