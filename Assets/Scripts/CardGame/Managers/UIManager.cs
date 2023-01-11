using Booble.CardGame.Cards;
using Booble.CardGame.Cards.DataModel;
using Booble.CardGame.Dialogues;
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

        [SerializeField] private GameObject playerHealthBar;
        [SerializeField] private GameObject playerManaCounter;

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
        [SerializeField] private Sprite noManaCristal;

        private int _shownPlayerLife;
        private int _shownPlayerMana;

        private int _shownOpponentLife;
        private int _shownOpponentMana;

        private int _defaultMaxLife;
        private int _defaultMaxMana;

        #endregion

        #region Extended Description Parameters

        [Header("Extended Description")]
        [SerializeField] private ExtendedDescriptionPanel _extendedDescriptionPanel;

        [Header("Turn Animation")]
        [SerializeField] private TurnAnimation _turnAnimation;

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
        [SerializeField] private EndTurnButton _endTurnButton;
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
        }

        #region Inputs

        private void CheckMoveBanners()
        {
            if (Input.GetMouseButtonUp(0) && _bannersOn) MoveBanners();
        }

        #endregion

        #region Turn Animation

        public void TurnAnimation(Turn turn)
        {
            _turnAnimation.SetTurnAnimation(turn);
            SetEndTurnButtonText(turn);
        }

        #endregion

        #region Stats

        public GameObject GetPlayerHealthBar() => playerHealthBar;
        public GameObject GetPlayerManaCounter() => playerManaCounter;

        public bool statsUpdated { private set; get; }
        private bool _hideEmptyCristals;

        public void UpdateUIStats(bool hideEmptyCristals = false)
        {
            _hideEmptyCristals = hideEmptyCristals;
            StartCoroutine(UpdateUIStatsCoroutine());
        }

        private IEnumerator UpdateUIStatsCoroutine()
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

                manaList[index].sprite = _hideEmptyCristals ? noManaCristal : emptyManaCristal;
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

        private void SetEndTurnButtonText(Turn turn) { _endTurnButton.SetEndTurnButtonText(turn); }
        public void SetEndTurnButtonInteractable(bool value = true) { _endTurnButton.SetEndTurnButtonInteractable(value); }
        public bool IsEndTurnButtonInteractable() => _endTurnButton.IsEndTurnButtonInteractable();

        #endregion

        #region Extended Description

        public void ShowExtendedDescription(CardsData data)
        {
            _extendedDescriptionPanel.Show(data);
        }

        public void HideExtendedDescription()
        {
            _extendedDescriptionPanel.Hide();
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

        public void OnCancelPlayButtonClick()
        {
            Card holdingCard = MouseController.Instance.holdingCard;
            if (holdingCard != null && holdingCard.IsPlayerCard)
            {
                holdingCard.contender.hand.AddCard(holdingCard.gameObject);

                if (MouseController.Instance.IsApplyingEffect) MouseController.Instance.ResetApplyingEffect();
                if (holdingCard.IsAction) Board.Instance.RemoveTargetsHighlight();
                else Board.Instance.RemoveCardZonesHighlight(holdingCard);

                HidePlayButtons();
                SetEndTurnButtonInteractable();
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

                CardGameManager.Instance.StartInterview();
            });

            sequence.Play();
        }

        #endregion

        #region Game End

        public void InterviewEndAnimation(bool win, Action endCallback)
        {
            _turnAnimation.SetInterviewEndAnimation(win, endCallback);
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