using Booble.CardGame.Managers;
using Booble.CardGame.Utils;
using Booble.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Booble.CardGame.Managers.TurnManager;

namespace Booble.CardGame.UI
{
    public class EndTurnButton : MonoBehaviour
    {
        [SerializeField] private MyButton endTurnButton;

        private const string INTERVIEW_START_BUTTON_TEXT = "Comienza la entrevista";
        private const string ROUND_START_BUTTON_TEXT = "Comienzo de ronda";
        private const string OPPONENTM_TURN_BUTTON_TEXT = "Turno del entrevistado";
        private const string OPPONENTW_TURN_BUTTON_TEXT = "Turno de la entrevistada";
        private const string PLAYER_TURN_CLASH_BUTTON_TEXT = "Batirse";
        private const string PLAYER_TURN_SKIP_BUTTON_TEXT = "Pasar turno";
        private const string DISCARDING_BUTTON_TEXT = "Descartando";
        private const string CLASH_BUTTON_TEXT = "Combate";
        private const string END_BUTTON_TEXT = "Fin de la ronda";

        public bool IsEndTurnButtonInteractable() { return endTurnButton.IsInteractable(); }

        public void OnClick()
        {
            if (CardGameManager.Instance.tutorial)
            {
                CardGameManager.Instance.ContinueTutorial();
            }
            else
                TurnManager.Instance.FinishTurn();
        }

        public void SetEndTurnButtonInteractable(bool interactable)
        {
            endTurnButton.SetInteractable(interactable);
        }

        public void SetEndTurnButtonText(Turn turn)
        {
            switch (turn)
            {
                case Turn.INTERVIEW_START:
                    endTurnButton.SetText(INTERVIEW_START_BUTTON_TEXT); break;

                case Turn.ROUND_START:
                    endTurnButton.SetText(ROUND_START_BUTTON_TEXT); break;

                case Turn.OPPONENT:
                    if (DeckManager.Instance.GetOpponentName() == Opponent_Name.Secretary)
                        endTurnButton.SetText(OPPONENTW_TURN_BUTTON_TEXT);
                    else
                        endTurnButton.SetText(OPPONENTM_TURN_BUTTON_TEXT);
                    break;

                case Turn.PLAYER:
                    if (TurnManager.Instance.GetSkipCombat())
                        endTurnButton.SetText(PLAYER_TURN_SKIP_BUTTON_TEXT);
                    else
                        endTurnButton.SetText(PLAYER_TURN_CLASH_BUTTON_TEXT);
                    break;

                case Turn.DISCARDING:
                    endTurnButton.SetText(DISCARDING_BUTTON_TEXT); break;

                case Turn.CLASH:
                    endTurnButton.SetText(CLASH_BUTTON_TEXT); break;

                case Turn.ROUND_END:
                    endTurnButton.SetText(END_BUTTON_TEXT); break;
            }
        }


    }
}