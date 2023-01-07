using Booble.CardGame.Cards;
using Booble.CardGame.Cards.DataModel.Effects;
using Booble.CardGame.Level;
using DG.Tweening;
using Santi.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Booble.CardGame.Managers
{
    public class TurnManager : Singleton<TurnManager>
    {

        public enum Turn
        {
            INTERVIEW_START, ROUND_START, PLAYER, OPPONENT, DISCARDING, CLASH, ROUND_END, INTERVIEW_END
        }

        public bool IsPlayerTurn => _turn == Turn.PLAYER;
        public bool IsOpponentTurn => _turn == Turn.OPPONENT;

        private Turn _turn = Turn.INTERVIEW_START;
        public Turn turn { get { return _turn; } private set { _turn = value; } }

        public bool continueFlow { private set; get; }

        [SerializeField] private ClashManager _clash;
        public ClashManager GetClashManager() => _clash;

        public bool combat => _clash.combat;

        public void ApplyCombatActions() { _clash.ApplyCombatActions(); }

        #region Turn flow 

        public void StartGame()
        {
            StartCoroutine(StartGameCoroutine());
        }

        private IEnumerator StartGameCoroutine()
        {
            UIManager.Instance.TurnAnimation(turn);

            yield return new WaitUntil(() => GetContinueFlow());

            DrawCards(CardGameManager.Instance.settings.initialCardNumber);

            yield return new WaitUntil(() => GetContinueFlow());

            ChangeTurn();
        }

        private void StartRound()
        {
            StartCoroutine(StartRoundCoroutine());
        }

        private IEnumerator StartRoundCoroutine()
        {
            UIManager.Instance.TurnAnimation(turn);

            yield return new WaitUntil(() => GetContinueFlow());

            StopFlow();
            CardGameManager.Instance.FillMana();
            UIManager.Instance.UpdateUIStats();

            yield return new WaitUntil(() => UIManager.Instance.statsUpdated);

            ChangeTurn();
        }

        public void StartTurn()
        {
            StartCoroutine(StartTurnCoroutine());
        }

        private IEnumerator StartTurnCoroutine()
        {
            UIManager.Instance.TurnAnimation(turn);

            yield return new WaitUntil(() => GetContinueFlow());

            DrawCards(1);

            yield return new WaitUntil(() => GetContinueFlow());

            if (turn == Turn.OPPONENT)
                CardGameManager.Instance.EnableOpponentAI();
        }

        public void FinishTurn()
        {
            ChangeTurn();
        }

        private void StartClash()
        {
            if (skipCombat)
            {
                skipCombat = false;
                ChangeTurn();
            }
            else
                StartCoroutine(StartClashCoroutine());
        }

        private IEnumerator StartClashCoroutine()
        {
            UIManager.Instance.TurnAnimation(turn);

            yield return new WaitUntil(() => GetContinueFlow());

            _clash.Clash();
        }

        private void FinishRound()
        {
            if (!CardGameManager.Instance.CheckEnd())
            {
                StartCoroutine(FinishRoundCoroutine());
            }
        }

        private IEnumerator FinishRoundCoroutine()
        {
            UIManager.Instance.TurnAnimation(turn);

            yield return new WaitUntil(() => GetContinueFlow());

            CardEffectsManager.Instance.ApplyEndTurnEffects();
        }

        public void ChangeTurn()
        {
            switch (_turn)
            {
                case Turn.INTERVIEW_START:
                    SetTurn(Turn.ROUND_START);
                    StartRound();
                    break;

                case Turn.ROUND_START:
                    SetTurn(Turn.OPPONENT);
                    StartTurn();
                    break;

                case Turn.OPPONENT:
                    SetTurn(Turn.PLAYER);
                    CardGameManager.Instance.DisableOpponentAI();
                    CardGameManager.Instance.opponent.hand.CheckDiscarding();
                    break;

                case Turn.PLAYER:
                    SetTurn(Turn.DISCARDING);
                    if (!CardGameManager.Instance.player.hand.CheckStartDiscarding())
                    {
                        ChangeTurn();
                    }
                    break;

                case Turn.DISCARDING:
                    SetTurn(Turn.CLASH);
                    StartClash();
                    break;

                case Turn.CLASH:
                    SetTurn(Turn.ROUND_END);
                    FinishRound();
                    break;

                case Turn.ROUND_END:
                    SetTurn(Turn.ROUND_START);
                    StartRound();
                    break;

                case Turn.INTERVIEW_END: break;

            }
        }

        private void SetTurn(Turn turn)
        {
            _turn = turn;
        }

        public void StopFlow()
        {
            continueFlow = false;
        }

        public void ContinueFlow()
        {
            continueFlow = true;
        }

        public bool GetContinueFlow()
        {
            return continueFlow && !CardGameManager.Instance.gamePaused;
        }

        private void DrawCards(int cardNumber)
        {
            StopFlow();
            Board.Instance.DrawCards(cardNumber, _turn);
        }

        #endregion

        #region Interview End

        public void CheckEndMidTurn()
        {
            if (CardGameManager.Instance.CheckEnd())
            {
                StopFlow();
            }
        }

        public void AlternateWinCondition()
        {
            CardGameManager.Instance.SetAlternateWinCondition();
            CheckEndMidTurn();
        }

        #endregion

        #region Effects

        private bool skipCombat;

        public void SkipCombat()
        {
            skipCombat = true;
        }

        public bool GetSkipCombat()
        {
            return skipCombat;
        }

        #endregion
    }
}