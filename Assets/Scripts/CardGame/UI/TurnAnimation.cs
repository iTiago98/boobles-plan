using Booble.CardGame.Managers;
using Booble.Managers;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Booble.CardGame.Managers.TurnManager;

namespace Booble.CardGame.UI
{
    public class TurnAnimation : MonoBehaviour
    {
        [SerializeField] private Sprite _interviewStartSprite;
        [SerializeField] private Sprite _roundStartSprite;
        [SerializeField] private Sprite _opponentTurnMSprite;
        [SerializeField] private Sprite _opponentTurnWSprite;
        [SerializeField] private Sprite _playerTurnSprite;
        [SerializeField] private Sprite _clashSprite;
        [SerializeField] private Sprite _roundEndSprite;
        [SerializeField] private Sprite _interviewEndSprite;
        [SerializeField] private Sprite _interviewWinSprite;
        [SerializeField] private Sprite _interviewLoseSprite;

        private Image _imageComponent;

        private void Start()
        {
            _imageComponent = GetComponent<Image>();
        }

        public void SetTurnAnimation(Turn turn)
        {
            Debug.Log(turn);
            TurnManager.Instance.StopFlow();

            Sprite sprite = null;

            switch (turn)
            {
                case Turn.INTERVIEW_START:
                    sprite = _interviewStartSprite;
                    break;
                case Turn.ROUND_START:
                    sprite = _roundStartSprite;
                    break;
                case Turn.PLAYER:
                    sprite = _playerTurnSprite;
                    break;
                case Turn.OPPONENT:
                    if (DeckManager.Instance.GetOpponentName() == Opponent_Name.Secretary)
                        sprite = _opponentTurnWSprite;
                    else
                        sprite = _opponentTurnMSprite;
                    break;
                case Turn.DISCARDING:
                    break;
                case Turn.CLASH:
                    sprite = _clashSprite;
                    break;
                case Turn.ROUND_END:
                    sprite = _roundEndSprite;
                    break;
                case Turn.INTERVIEW_END:
                    sprite = _interviewEndSprite;
                    break;
            }

            SetTurnAnimation(sprite, TurnManager.Instance.ContinueFlow);
        }

        public void SetInterviewEndAnimation(bool playerWin, Action endCallback)
        {
            if (playerWin) SetTurnAnimation(_interviewWinSprite, endCallback);
            else SetTurnAnimation(_interviewLoseSprite, endCallback);
        }

        private void SetTurnAnimation(Sprite sprite, Action endCallback)
        {
            _imageComponent.sprite = sprite;

            Sequence sequence = DOTween.Sequence();

            sequence.Append(_imageComponent.DOFade(1, 0.5f));
            sequence.AppendInterval(0.5f);
            sequence.Append(_imageComponent.DOFade(0, 0.5f));
            sequence.AppendCallback(() => endCallback());

            sequence.Play();

            UIManager.Instance.SetEndTurnButtonInteractable(false);
        }

    }
}