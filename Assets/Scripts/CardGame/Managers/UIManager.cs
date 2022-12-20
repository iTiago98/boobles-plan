using CardGame.Cards;
using CardGame.Cards.DataModel;
using CardGame.Level;
using CardGame.Utils;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static CardGame.Managers.TurnManager;

namespace CardGame.Managers
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

        [SerializeField] private TextMeshProUGUI interviewEndText;
        [SerializeField] private MyButton interviewEndButton;

        [SerializeField] private MyButton endTurnButton;

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

        #endregion

        #region Steal Cards From Deck

        [Header("Steal Cards From Deck")]
        [SerializeField] private GameObject stealCardsFromDeckGameObj;
        [SerializeField] private Transform stealCardsFromDeckParent;
        [SerializeField] private GameObject stealCardsFromDeckButton;
        [SerializeField] private GameObject cardImagePrefab;

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

        private Contender _player;
        private Contender _opponent;

        private const string INTERVIEW_START_BUTTON_TEXT = "Comienza la entrevista";
        private const string ROUND_START_BUTTON_TEXT = "Comienzo de ronda";
        private const string OPPONENTM_TURN_BUTTON_TEXT = "Turno del entrevistado";
        private const string OPPONENTW_TURN_BUTTON_TEXT = "Turno de la entrevistada";
        private const string PLAYER_TURN_CLASH_BUTTON_TEXT = "Batirse";
        private const string PLAYER_TURN_SKIP_BUTTON_TEXT = "Pasar turno";
        private const string DISCARDING_BUTTON_TEXT = "Descartando";
        private const string CLASH_BUTTON_TEXT = "Combate";
        private const string END_BUTTON_TEXT = "Fin de la ronda";

        private const string INTERVIEW_WIN_TEXT = "Has ganado";
        private const string INTERVIEW_LOSE_TEXT = "Has perdido";

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _player = CardGameManager.Instance.player;
            _opponent = CardGameManager.Instance.opponent;
            _defaultMaxLife = CardGameManager.Instance.settings.initialLife;
            _defaultMaxMana = CardGameManager.Instance.settings.maxManaCounter;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (cancelPlayButton.activeSelf) OnCancelPlayButtonClick();
            }

            if (Input.GetMouseButtonUp(0) && _bannersOn) MoveBanners();
        }

        public void TurnAnimation(Turn turn)
        {
            TurnManager.Instance.StopFlow();

            switch (turn)
            {
                case Turn.INTERVIEW_START:
                    turnAnimationImage.sprite = _interviewStartSprite;
                    break;
                case Turn.ROUND_START:
                    turnAnimationImage.sprite = _roundStartSprite;
                    break;
                case Turn.PLAYER:
                    turnAnimationImage.sprite = _playerTurnSprite;
                    break;
                case Turn.OPPONENT:
                    if (DeckManager.Instance.GetOpponentName() == Opponent_Name.Secretary)
                        turnAnimationImage.sprite = _opponentTurnWSprite;
                    else
                        turnAnimationImage.sprite = _opponentTurnMSprite;
                    break;
                case Turn.DISCARDING:
                    break;
                case Turn.CLASH:
                    turnAnimationImage.sprite = _clashSprite;
                    break;
                case Turn.ROUND_END:
                    turnAnimationImage.sprite = _roundEndSprite;
                    break;
                case Turn.INTERVIEW_END:
                    turnAnimationImage.sprite = _interviewEndSprite;
                    break;
            }

            Sequence sequence = DOTween.Sequence();

            sequence.Append(turnAnimationImage.DOFade(1, 0.5f));
            sequence.AppendInterval(0.5f);
            sequence.Append(turnAnimationImage.DOFade(0, 0.5f));
            sequence.AppendCallback(() => TurnManager.Instance.ContinueFlow());

            if (turn == Turn.PLAYER) sequence.AppendCallback(() => SetEndTurnButtonInteractable(true));
            else SetEndTurnButtonInteractable(false);

            sequence.Play();

            SetEndTurnButtonText(turn);
        }

        #region Stats

        public void UpdateUIStats()
        {
            UpdateUIStats(startRound: false);
        }

        public void UpdateUIStats(bool startRound)
        {
            StartCoroutine(UpdateUIStatsCoroutine(startRound));
        }

        private IEnumerator UpdateUIStatsCoroutine(bool continueFlow)
        {
            int loops = Mathf.Max(
                Mathf.Abs(_player.life - _shownPlayerLife),
                Mathf.Abs(_player.currentMana - _shownPlayerMana),
                Mathf.Abs(_opponent.life - _shownOpponentLife),
                Mathf.Abs(_opponent.currentMana - _shownOpponentMana)
                );

            for (int i = 0; i < loops; i++)
            {
                SetStats(_player.life, _player.currentMana, _player.currentMaxMana, _player.extraMana,
                    _opponent.life, _opponent.currentMana, _opponent.currentMaxMana, _opponent.extraMana);

                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitUntil(() =>
                _shownPlayerLife == _player.life && _shownPlayerMana == _player.currentMana
                && _shownOpponentLife == _opponent.life && _shownOpponentMana == _opponent.currentMana);

            if (continueFlow) TurnManager.Instance.ContinueFlow();
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
            if (card.IsPlayerCard) ContinuePlay(card);
            else ContinuePlayOpponent(card);

            Board.Instance.RemoveTargetsHighlight();
            HidePlayButtons();
        }

        private void ContinuePlay(Card card)
        {
            StartCoroutine(ContinuePlayCoroutine(
                card,
                card.Effects.ApplyEffect,
                () => {
                    SetEndTurnButtonInteractable(true);
                }));
        }

        private void ContinuePlayOpponent(Card card)
        {
            StartCoroutine(ContinuePlayCoroutine(
                card,
                () => card.Effects.ApplyEffect(card.storedTarget),
                () => {
                    card.storedTarget = null;
                    CardGameManager.Instance.opponentAI.enabled = true;
                }));
        }

        private IEnumerator ContinuePlayCoroutine(Card card, Action applyEffect, Action onDestroy)
        {
            TurnManager.Instance.StopFlow();

            card.Stats.SubstractMana();
            applyEffect();

            yield return new WaitUntil(() => TurnManager.Instance.continueFlow);

            card.DestroyCard();

            yield return new WaitUntil(() => card == null);

            onDestroy();

            MouseController.Instance.SetHolding(null);
        }

        public void OnCancelPlayButtonClick()
        {
            Card holdingCard = MouseController.Instance.holdingCard;
            if (holdingCard != null && holdingCard.contender.isPlayer)
            {
                holdingCard.contender.hand.AddCard(holdingCard);

                if (MouseController.Instance.IsApplyingEffect) MouseController.Instance.ResetApplyingEffect();
                else if (holdingCard.Stats.type == CardType.ACTION) Board.Instance.RemoveTargetsHighlight();
                else Board.Instance.RemoveCardZonesHighlight(holdingCard);

                HidePlayButtons();
                SetEndTurnButtonInteractable(true);
                MouseController.Instance.SetHolding(null);
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

        private List<int> cardsToSteal = new List<int>();
        private int _numCardsToSteal;
        private Deck _deckToSteal;

        public void ShowStealCardsFromDeck(Deck deck, int numCards)
        {
            stealCardsFromDeckGameObj.SetActive(true);
            stealCardsFromDeckButton.SetActive(false);

            MouseController.Instance.SetStealing();

            _numCardsToSteal = (deck.numCards >= numCards) ? numCards : deck.numCards;
            _deckToSteal = deck;

            List<CardsData> deckCards = deck.GetDeckCards();

            foreach (CardsData cardData in deckCards)
            {
                GameObject cardObj = Instantiate(cardImagePrefab, stealCardsFromDeckParent);
                CardImageUI cardImageUI = cardObj.GetComponent<CardImageUI>();
                cardImageUI.Initialize(cardData, deckCards.IndexOf(cardData));
            }
        }

        public void AddStolenCard(int index)
        {
            if (cardsToSteal.Count == _numCardsToSteal) return;

            cardsToSteal.Add(index);
            if (cardsToSteal.Count == _numCardsToSteal)
            {
                stealCardsFromDeckButton.SetActive(true);
            }
        }

        public void RemoveStolenCard(int index)
        {
            cardsToSteal.Remove(index);
            stealCardsFromDeckButton.SetActive(false);
        }

        public void OnStealCardsFromDeckButtonClick()
        {
            stealCardsFromDeckGameObj.SetActive(false);
            _deckToSteal.StealCards(cardsToSteal);
            MouseController.Instance.SetSelecting();
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
        }

        public void MoveBanners()
        {
            _bannersOn = false;
            Sequence sequence = DOTween.Sequence();

            sequence.Join(_playerBanner.transform.DOMoveX(-20, 2f));
            sequence.Join(_opponentBanner.transform.DOMoveX(20, 2f));

            sequence.OnComplete(() => CardGameManager.Instance.ThrowStartDialogue());

            sequence.Play();
        }

        #endregion

        #region Game End

        public void SetInterviewWinText(bool win)
        {
            if (win) interviewEndText.text = INTERVIEW_WIN_TEXT;
            else interviewEndText.text = INTERVIEW_LOSE_TEXT;
        }

        public void ShowEndButton(bool show)
        {
            interviewEndButton.gameObject.SetActive(show);
        }

        public void OnEndButtonClick()
        {
            SceneLoader.Instance.UnloadInterviewScene();
        }

        #endregion
    }
}