using Booble.CardGame.Cards;
using Booble.CardGame.Cards.DataModel;
using Booble.CardGame.Level;
using Booble.CardGame.Utils;
using Booble.Managers;
using Booble.UI;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Booble.CardGame.Managers.TurnManager;

namespace Booble.CardGame.Managers
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { private set; get; }

        #region Stats Parameters

        [Header("Contenders Stats")]

        [SerializeField] private Image playerHealthImage;
        [SerializeField] private Image playerExtraHealthImage;
        [SerializeField] private Image playerExtraHealthImage2;
        [SerializeField] private List<Image> playerManaList;

        [SerializeField] private Image opponentHealthImage;
        [SerializeField] private Image opponentExtraHealthImage;
        [SerializeField] private Image opponentExtraHealthImage2;
        [SerializeField] private List<Image> opponentManaList;

        [SerializeField] private Sprite fullManaCristal;
        [SerializeField] private Sprite emptyManaCristal;
        [SerializeField] private Sprite fullExtraManaCristal;

        private int _shownPlayerLife;
        private int _shownPlayerMana;

        private int _shownOpponentLife;
        private int _shownOpponentMana;

        private int _defaultMaxLife;
        private int _defaultMaxMana;

        #endregion

        #region Extended Description Parameters

        [Header("Extended Description")]

        [SerializeField] private GameObject extendedDescriptionPanel;

        [SerializeField] private TextMeshProUGUI extendedDescriptionName;
        [SerializeField] private TextMeshProUGUI extendedDescriptionType;
        [SerializeField] private TextMeshProUGUI extendedDescriptionText;

        #endregion

        #region Deck Remaining Cards Parameters

        [Header("Deck Remaining Cards")]
        [SerializeField] private GameObject playerDeckRemainingCardsPanel;
        [SerializeField] private TextMeshProUGUI playerDeckRemainingCardsText;

        [SerializeField] private GameObject opponentDeckRemainingCardsPanel;
        [SerializeField] private TextMeshProUGUI opponentDeckRemainingCardsText;

        #endregion

        #region Interactables Parameters

        [Header("Interactables")]

        [SerializeField] private GameObject continuePlayButton;
        [SerializeField] private GameObject cancelPlayButton;

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

        #endregion

        #region Turn Animation

        [Header("Turn Animation")]
        [SerializeField] private Image turnAnimationImage;

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

        #endregion

        #region Steal Cards From Deck

        [Header("Steal Cards From Deck")]
        [SerializeField] private GameObject stealCardsFromDeckButton;

        #endregion

        #region Particle Systems

        [Header("Particle Systems")]
        [SerializeField] private GameObject particleSystem_effectApply_Prefab;
        [SerializeField] private GameObject particleSystem_effectTargetPositive_Prefab;
        [SerializeField] private GameObject particleSystem_effectTargetNegative_Prefab;

        #endregion

        #region Interview Start

        [Header("Interview Start")]
        [SerializeField] private GameObject _playerBanner;
        [SerializeField] private GameObject _opponentBanner;

        private bool _bannersOn = true;

        #endregion

        #region Lose Menu

        [Header("Lose Menu")]
        [SerializeField] private GameObject _loseMenu;

        public bool loseMenuActive => _loseMenu.activeSelf;

        #endregion


        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _defaultMaxLife = CardGameManager.Instance.settings.initialLife;
            _defaultMaxMana = CardGameManager.Instance.settings.maxManaCounter;
        }

        private void Update()
        {
            CheckMoveBanners();
            CheckPauseMenu();
        }

        #region Inputs

        private void CheckMoveBanners()
        {
            if (Input.GetMouseButtonUp(0) && _bannersOn) MoveBanners();
        }

        private void CheckPauseMenu()
        {
            if ((Input.GetKeyDown(KeyCode.Escape) || PauseMenu.Instance.hide) && !loseMenuActive)
            {
                PauseMenu.Instance.ShowHidePauseMenu();
                CardGameManager.Instance.SwitchGameState();
            }
        }

        #endregion

        #region Turn Animation

        public void TurnAnimation(Turn turn)
        {
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

            TurnAnimation(sprite, turn, TurnManager.Instance.ContinueFlow);
            SetEndTurnButtonText(turn);
        }

        public void TurnAnimation(Sprite sprite, Turn turn, TweenCallback endCallback)
        {
            turnAnimationImage.sprite = sprite;

            Sequence sequence = DOTween.Sequence();

            sequence.Append(turnAnimationImage.DOFade(1, 0.5f));
            sequence.AppendInterval(0.5f);
            sequence.Append(turnAnimationImage.DOFade(0, 0.5f));
            sequence.AppendCallback(endCallback);

            if (turn == Turn.PLAYER) sequence.AppendCallback(() => SetEndTurnButtonInteractable(true));
            else SetEndTurnButtonInteractable(false);

            sequence.Play();
        }

        #endregion

        #region Stats

        public bool statsUpdated { private set; get; }

        public void UpdateUIStats()
        {
            UpdateUIStats(startRound: false);
        }

        public void UpdateUIStats(bool startRound)
        {
            StartCoroutine(UpdateUIStatsCoroutine(startRound));
        }

        private IEnumerator UpdateUIStatsCoroutine(bool startRound)
        {
            Contender player = CardGameManager.Instance.player;
            Contender opponent = CardGameManager.Instance.opponent;

            statsUpdated = false;

            int loops = Mathf.Max(
                Mathf.Abs(player.life - _shownPlayerLife),
                Mathf.Abs(player.currentMana - _shownPlayerMana),
                Mathf.Abs(opponent.life - _shownOpponentLife),
                Mathf.Abs(opponent.currentMana - _shownOpponentMana)
                );

            for (int i = 0; i < loops; i++)
            {
                SetStats(player.life, player.currentMana, player.currentMaxMana, player.extraMana,
                    opponent.life, opponent.currentMana, opponent.currentMaxMana, opponent.extraMana);

                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitUntil(() =>
                _shownPlayerLife == player.life && _shownPlayerMana == player.currentMana
                && _shownOpponentLife == opponent.life && _shownOpponentMana == opponent.currentMana);

            statsUpdated = true;
        }

        private void SetStats(int playerCurrentLife, int playerCurrentMana, int playerCurrentMaxMana, int playerExtraMana,
            int opponentCurrentLife, int opponentCurrentMana, int opponentCurrentMaxMana, int opponentExtraMana)
        {
            if (_shownPlayerLife != playerCurrentLife)
                SetHealth(playerHealthImage, playerExtraHealthImage, playerExtraHealthImage2, ref _shownPlayerLife, playerCurrentLife);
            if (_shownOpponentLife != opponentCurrentLife)
                SetHealth(opponentHealthImage, opponentExtraHealthImage, opponentExtraHealthImage2, ref _shownOpponentLife, opponentCurrentLife);

            if (_shownPlayerMana != playerCurrentMana)
                SetMana(playerManaList, ref _shownPlayerMana, playerCurrentMana, playerCurrentMaxMana, playerExtraMana);
            if (_shownOpponentMana != opponentCurrentMana)
                SetMana(opponentManaList, ref _shownOpponentMana, opponentCurrentMana, opponentCurrentMaxMana, opponentExtraMana);
        }

        private void SetHealth(Image healthImage, Image extraHealthImage, Image extraHealthImage2, ref int shownLife, int currentLife)
        {
            if (shownLife < currentLife) shownLife++;
            else shownLife--;

            healthImage.fillAmount = (float)shownLife / _defaultMaxLife;

            if (shownLife >= _defaultMaxLife) extraHealthImage.fillAmount = (float)(shownLife - _defaultMaxLife) / _defaultMaxLife;
            if (shownLife >= (_defaultMaxLife * 2)) extraHealthImage2.fillAmount = (float)(shownLife - (_defaultMaxLife * 2)) / _defaultMaxLife;
        }

        private void SetMana(List<Image> manaList, ref int shownMana, int currentMana, int currentMaxMana, int extraMana)
        {
            //Debug.Log("Shown Mana: " + shownMana + " - CurrentMana: " + currentMana);
            if (shownMana < currentMana)
            {
                //Debug.Log("1: Mana " + (shownMana) + " full");
                if (IsExtraMana(extraMana, currentMaxMana, shownMana))
                    manaList[_defaultMaxMana + (shownMana - currentMaxMana + extraMana)].sprite = fullExtraManaCristal;

                else manaList[shownMana].sprite = fullManaCristal;

                shownMana++;
            }
            else
            {
                //Debug.Log("2: Mana " + (shownMana - 1) + " empty");
                shownMana--;
                int index;
                if (IsExtraMana(extraMana, currentMaxMana, shownMana)) index = _defaultMaxMana + (shownMana - currentMaxMana + extraMana);
                else index = shownMana;

                manaList[index].sprite = emptyManaCristal;
            }
        }

        private bool IsExtraMana(int extraMana, int currentMaxMana, int shownMana)
        {
            return extraMana > 0
                && (currentMaxMana - shownMana) <= extraMana
                && (currentMaxMana - extraMana) <= _defaultMaxMana;
        }

        public void UpdateMaxMana(Contender contender, int newMaxMana)
        {
            List<Image> manaList = (contender.role == Contender.Role.PLAYER) ? playerManaList : opponentManaList;
            manaList[newMaxMana - 1].sprite = emptyManaCristal;
        }

        #endregion

        #region End Turn Button

        public void OnEndTurnButtonClick()
        {
            TurnManager.Instance.FinishTurn();
        }

        public bool IsEndTurnButtonInteractable() { return endTurnButton.IsInteractable(); }

        public void SetEndTurnButtonInteractable(bool interactable)
        {
            endTurnButton.SetInteractable(interactable);
        }

        private void SetEndTurnButtonText(Turn turn)
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

        #endregion

        #region Extended Description

        public void ShowExtendedDescription(string name, string type, string description)
        {
            extendedDescriptionName.text = name;
            extendedDescriptionType.text = type;
            extendedDescriptionText.text = description;
            extendedDescriptionPanel.SetActive(true);
        }

        public void HideExtendedDescription()
        {
            extendedDescriptionPanel.SetActive(false);
        }

        #endregion

        #region Deck Remaining Cards

        public void ShowRemainingCards(Contender contender)
        {
            if (contender.role == Contender.Role.PLAYER) playerDeckRemainingCardsPanel.SetActive(true);
            else opponentDeckRemainingCardsPanel.SetActive(true);
        }

        public void UpdateRemainingCards(int remainingCards, int maxCards, Contender contender)
        {
            if (remainingCards < 0) remainingCards = 0;
            if (contender.role == Contender.Role.PLAYER) playerDeckRemainingCardsText.text = remainingCards + " / " + maxCards;
            else opponentDeckRemainingCardsText.text = remainingCards + " / " + maxCards;
        }

        #endregion

        #region Play Buttons

        public void OnContinuePlayButtonClick()
        {
            Card card = MouseController.Instance.holdingCard;
            card.ContinueAction();
        }

        //private void ContinuePlay(Card card)
        //{
        //    card.ContinueAction(null);

        //    //StartCoroutine(ContinuePlayCoroutine(
        //    //    card,
        //    //    () => card.Effects.ApplyFirstEffect(null),
        //    //    () =>
        //    //    {
        //    //        SetEndTurnButtonInteractable(true);
        //    //    }));
        //}

        //private void ContinuePlayOpponent(Card card)
        //{
        //    card.ContinueAction(card.storedTarget);

        //    //StartCoroutine(ContinuePlayCoroutine(
        //    //    card,
        //    //    () => card.Effects.ApplyFirstEffect(card.storedTarget),
        //    //    () =>
        //    //    {
        //    //        card.storedTarget = null;
        //    //        CardGameManager.Instance.opponentAI.enabled = true;
        //    //    }));
        //}

        //private IEnumerator ContinuePlayCoroutine(Card card, Action applyEffect, Action onDestroy)
        //{
        //    //card.Stats.SubstractMana();
        //    //applyEffect();

        //    //yield return new WaitUntil(() => card.effect.effectApplied);

        //    CardGameManager.Instance.CheckDialogue(card);

        //    yield return new WaitUntil(() => CardGameManager.Instance.dialogueEnd);

        //    card.DestroyCard();

        //    yield return new WaitUntil(() => card.destroyed);

        //    onDestroy();

        //    MouseController.Instance.SetHolding(null);
        //    CardGameManager.Instance.SetPlayingCard(false);
        //}

        public void OnCancelPlayButtonClick()
        {
            Card holdingCard = MouseController.Instance.holdingCard;
            if (holdingCard != null && holdingCard.contender.isPlayer)
            {
                holdingCard.contender.hand.AddCard(holdingCard.gameObject);

                if (MouseController.Instance.IsApplyingEffect) MouseController.Instance.ResetApplyingEffect();
                if (holdingCard.Stats.type == CardType.ACTION) Board.Instance.RemoveTargetsHighlight();
                else Board.Instance.RemoveCardZonesHighlight(holdingCard);

                HidePlayButtons();
                SetEndTurnButtonInteractable(true);
                MouseController.Instance.SetHolding(null);
                CardGameManager.Instance.SetPlayingCard(false);
            }
        }

        public void ShowContinuePlayButton()
        {
            continuePlayButton.SetActive(true);
        }
        public void ShowCancelPlayButton()
        {
            cancelPlayButton.SetActive(true);
        }

        public void HidePlayButtons()
        {
            continuePlayButton.SetActive(false);
            cancelPlayButton.SetActive(false);
        }

        #endregion

        #region Steal Cards From Deck

        public bool stealing { private set; get; }

        public void SetStealing() { stealing = true; }

        public void ShowStealCardsFromDeckButton(bool show)
        {
            stealCardsFromDeckButton.SetActive(show);
        }

        public void OnStealCardsFromDeckButtonClick()
        {
            stealing = false;
            MouseController.Instance.SetSelecting();
            ShowStealCardsFromDeckButton(false);
        }

        #endregion

        #region Particle Systems 

        //public GameObject ShowParticlesEffectApply(Transform parent)
        //{
        //    return Instantiate(particleSystem_effectApply_Prefab, parent);
        //}

        //public void ShowParticlesEffectTargetPositive(Transform parent)
        //{
        //    Instantiate(particleSystem_effectTargetPositive_Prefab, parent);
        //}

        //public void ShowParticlesEffectTargetNegative(Transform parent)
        //{
        //    Instantiate(particleSystem_effectTargetNegative_Prefab, parent);
        //}


        #endregion

        #region Interview Start

        public void InitializeBanners(Sprite image)
        {
            _opponentBanner.GetComponent<Image>().sprite = image;

            _playerBanner.SetActive(true);
            _opponentBanner.SetActive(true);
        }

        public void MoveBanners()
        {
            _bannersOn = false;
            Sequence sequence = DOTween.Sequence();

            sequence.Join(_playerBanner.transform.DOMoveX(-20, 2f));
            sequence.Join(_opponentBanner.transform.DOMoveX(20, 2f));

            sequence.OnComplete(() =>
            {
                _playerBanner.SetActive(false);
                _opponentBanner.SetActive(false);

                CardGameManager.Instance.ThrowStartDialogue();
            });

            sequence.Play();
        }

        #endregion

        #region Game End

        public void InterviewEndAnimation(bool win, TweenCallback endCallback)
        {
            if (win) TurnAnimation(_interviewWinSprite, Turn.INTERVIEW_END, endCallback);
            else TurnAnimation(_interviewLoseSprite, Turn.INTERVIEW_END, endCallback);
        }

        public void ShowLoseMenu()
        {
            _loseMenu.SetActive(true);
        }

        public void OnRetryButtonClick()
        {
            SceneLoader.Instance.ReloadInterview();
        }

        #endregion
    }
}