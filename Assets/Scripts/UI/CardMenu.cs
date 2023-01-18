using Booble.CardGame;
using Booble.CardGame.Cards.DataModel;
using Booble.Flags;
using Booble.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Booble.Managers.DeckManager;

namespace Booble.UI
{
    public class CardMenu : MonoBehaviour
    {
        [Serializable]
        public struct ContenderInfo
        {
            public Opponent_Name name;
            public GameObject cardMenu;
            public GameObject cardList;
            public AlertButton cardButton;
            public Sprite buttonSprite;
            public Sprite cardBack;
        }

        [SerializeField] private List<ContenderInfo> _contendersInfo;

        [Header("Sprites")]
        [SerializeField] private Sprite _buttonNotAvailableSprite;

        private ContenderInfo _currentMenu;

        private List<CardsData> _playerBaseCards;

        public void SetCardsMenu()
        {
            Initialize();
        }

        public void Refresh()
        {
            SetButtonsAvailable();
        }

        #region Initialize

        private void Initialize()
        {
            GetPlayerBaseCards();
            InitializeCards();
            SetPlayerMenuActive();
        }

        private void GetPlayerBaseCards()
        {
            _playerBaseCards = DeckManager.Instance.GetPlayerBaseCards();
        }

        private void InitializeCards()
        {
            foreach (ContenderInfo contenderInfo in _contendersInfo)
            {
                List<CardsData> temp;

                if (contenderInfo.name == Opponent_Name.Tutorial)
                {
                    temp = new List<CardsData>(_playerBaseCards);
                    temp.AddRange(DeckManager.Instance.GetExtraCards(contenderInfo.name));
                }
                else
                {
                    temp = DeckManager.Instance.GetExtraCards(contenderInfo.name);
                }

                InitializeCardsList(contenderInfo.cardList, temp, contenderInfo.cardBack);
            }
        }

        private void InitializeCardsList(GameObject cardsParent, List<CardsData> cards, Sprite cardBack)
        {
            int index = 0;
            foreach (Transform cardObject in cardsParent.transform)
            {
                CardsData cardData = cards[index];
                cardObject.GetComponent<CardUI>().Initialize(cardData, cardBack);

                index++;
            }
        }

        private void SetPlayerMenuActive()
        {
            foreach (ContenderInfo contenderInfo in _contendersInfo)
            {
                contenderInfo.cardMenu.SetActive(false);
            }
            _contendersInfo[0].cardMenu.SetActive(true);
            _currentMenu = _contendersInfo[0];
        }

        #endregion

        #region Update Cards

        public void AddNewCard(CardData card)
        {
            ContenderInfo contenderInfo = _contendersInfo[(int)card.opponent];

            contenderInfo.cardButton.ShowAlert(true);

            foreach (Transform transform in contenderInfo.cardList.transform)
            {
                CardUI cardUI = transform.GetComponent<CardUI>();
                if (cardUI.GetName() == card.data.name)
                {
                    cardUI.SetFront();
                    cardUI.ShowAlertImage(true);
                }
            }
        }

        public void RemoveAlert(CardData card)
        {
            ContenderInfo contenderInfo = _contendersInfo[(int)card.opponent];

            contenderInfo.cardButton.ShowAlert(DeckManager.Instance.GetNewCardsCountFromOpponent(card.opponent) - 1 > 0);

            foreach(Transform transform in contenderInfo.cardList.transform)
            {
                CardUI cardUI = transform.GetComponent<CardUI>();
                if(cardUI.GetName() == card.data.name)
                {
                    cardUI.ShowAlertImage(false);
                }
            }
        }

        #endregion

        #region Buttons 

        private void SetButtonsAvailable()
        {
            List<Flag.Reference> flags = new List<Flag.Reference>() { Flag.Reference.Car0, Flag.Reference.Day1, Flag.Reference.Day2, Flag.Reference.Day3, Flag.Reference.Day4 };

            bool available = true;
            for (int i = 0; i < flags.Count; i++)
            {
                Flag.Reference flag = flags[i];
                ContenderInfo contenderInfo = _contendersInfo[i];

                SetAvailable(contenderInfo, available);
                if (!FlagManager.Instance.GetFlag(flag))
                {
                    available = false;
                }
            }
        }

        private bool GetInteractable(GameObject menu)
        {
            return _currentMenu.cardMenu != menu;
        }

        private void SetAvailable(ContenderInfo contenderInfo, bool available)
        {
            if (available)
            {
                contenderInfo.cardButton.SetImage(contenderInfo.buttonSprite);
                contenderInfo.cardButton.SetInteractable(GetInteractable(contenderInfo.cardMenu));
            }
            else
            {
                contenderInfo.cardButton.SetImage(_buttonNotAvailableSprite);
                contenderInfo.cardButton.SetInteractable(false);
            }
        }

        #region Buttons Click

        private void OnButtonClick(int contender)
        {
            _currentMenu.cardMenu.SetActive(false);
            _currentMenu.cardButton.SetInteractable(true);

            ContenderInfo contenderInfo = _contendersInfo[contender];

            contenderInfo.cardMenu.SetActive(true);
            contenderInfo.cardButton.SetInteractable(false);
            _currentMenu = contenderInfo;
        }

        public void OnPlayerButtonClick() { OnButtonClick(0); }
        public void OnCitrianoButtonClick() { OnButtonClick(1); }
        public void OnPPBrosButtonClick() { OnButtonClick(2); }
        public void OnSecretaryButtonClick() { OnButtonClick(3); }
        public void OnBossButtonClick() { OnButtonClick(4); }

        #endregion

        #endregion

    }
}