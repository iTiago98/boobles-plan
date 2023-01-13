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

        private GameObject _currentMenu;

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
            _currentMenu = _contendersInfo[0].cardMenu;
        }

        #endregion

        #region Update Cards

        public void UpdateExtraCards(List<CardData> newCards, bool showAlertIcons)
        {
            foreach (ContenderInfo contenderInfo in _contendersInfo)
            {
                contenderInfo.cardButton.ShowAlert(false);
            }

            foreach (CardData newCard in newCards)
            {
                ContenderInfo contenderInfo = _contendersInfo[(int)newCard.opponent];

                AlertButton button = contenderInfo.cardButton;
                button.ShowAlert(showAlertIcons);

                foreach (Transform transform in contenderInfo.cardList.transform)
                {
                    CardUI card = transform.GetComponent<CardUI>();
                    if (card.GetName() == newCard.data.name)
                    {
                        card.SetFront();
                        card.ShowAlertImage(showAlertIcons);
                    }
                }
            }
        }

        #endregion

        #region Buttons 

        private void SetButtonsAvailable()
        {
            bool flag0, flag1, flag2, flag3;

            flag0 = FlagManager.Instance.GetFlag(Flag.Reference.Home0);
            flag1 = FlagManager.Instance.GetFlag(Flag.Reference.Home1);
            flag2 = FlagManager.Instance.GetFlag(Flag.Reference.Home2);
            flag3 = FlagManager.Instance.GetFlag(Flag.Reference.Home3);

            foreach (ContenderInfo contenderInfo in _contendersInfo)
            {
                bool flag = false;
                switch (contenderInfo.name)
                {
                    case Opponent_Name.Tutorial:
                        flag = true;
                        break;
                    case Opponent_Name.Citriano:
                        flag = flag0;
                        break;
                    case Opponent_Name.PPBros:
                        flag = flag1;
                        break;
                    case Opponent_Name.Secretary:
                        flag = flag2;
                        break;
                    case Opponent_Name.Boss:
                        flag = flag3;
                        break;
                }

                SetAvailable(contenderInfo.cardButton, flag, GetInteractable(contenderInfo.cardMenu), contenderInfo.buttonSprite);
            }
        }

        private bool GetInteractable(GameObject menu)
        {
            return _currentMenu != menu;
        }

        private void SetAvailable(AlertButton button, bool available, bool interactable, Sprite sprite)
        {
            if (available)
            {
                button.SetImage(sprite);
                button.SetInteractable(interactable);
            }
            else
            {
                button.SetImage(_buttonNotAvailableSprite);
                button.SetInteractable(false);
            }
        }

        #region Buttons Click

        private void OnButtonClick(int contender)
        {
            ContenderInfo contenderInfo = _contendersInfo[contender];

            _currentMenu.SetActive(false);
            contenderInfo.cardMenu.SetActive(true);
            _currentMenu = contenderInfo.cardMenu;
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