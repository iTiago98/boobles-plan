using Booble.CardGame.Cards.DataModel;
using Booble.Flags;
using Booble.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Booble.UI
{
    public class CardMenu : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private GameObject _playerCardsMenu;
        [SerializeField] private GameObject _playerCardsList;
        [SerializeField] private Button _playerButton;
        [SerializeField] private Sprite _playerCardBack;

        [Header("Citriano")]
        [SerializeField] private GameObject _citrianoCardsMenu;
        [SerializeField] private GameObject _citrianoCardsList;
        [SerializeField] private Button _citrianoButton;
        [SerializeField] private Sprite _citrianoCardBack;

        [Header("PPBros")]
        [SerializeField] private GameObject _ppBrosCardsMenu;
        [SerializeField] private GameObject _ppBrosCardsList;
        [SerializeField] private Button _ppBrosButton;
        [SerializeField] private Sprite _ppBrosCardBack;

        [Header("Secretary")]
        [SerializeField] private GameObject _secretaryCardsMenu;
        [SerializeField] private GameObject _secretaryCardsList;
        [SerializeField] private Button _secretaryButton;
        [SerializeField] private Sprite _secretaryCardBack;

        [Header("Boss")]
        [SerializeField] private GameObject _bossCardsMenu;
        [SerializeField] private GameObject _bossCardsList;
        [SerializeField] private Button _bossButton;
        [SerializeField] private Sprite _bossCardBack;

        private GameObject _currentMenu;
        private bool _initialized;

        private List<CardsData> _playerBaseCards;

        public void SetCardsMenu()
        {
            if (!_initialized) GetPlayerBaseCards();
            UpdateExtraCards();
            SetButtonsAvailable();
        }

        private void GetPlayerBaseCards()
        {
            _playerBaseCards = DeckManager.Instance.GetPlayerBaseCards();
            _currentMenu = _playerCardsMenu;
            _initialized = true;
        }

        private void UpdateExtraCards()
        {
            if (_currentMenu == _playerCardsMenu)
            {
                List<CardsData> temp = new List<CardsData>(_playerBaseCards);
                temp.Add(DeckManager.Instance.GetGranFinal());
                SetCards(_playerCardsList, temp, _playerCardBack);
            }
            else if (_currentMenu == _citrianoCardsMenu)
            {
                SetCards(_citrianoCardsList, DeckManager.Instance.GetCitrianoExtraCards(), _citrianoCardBack);
            }
            else if (_currentMenu == _ppBrosCardsMenu)
            {
                SetCards(_ppBrosCardsList, DeckManager.Instance.GetPPBrosExtraCards(), _ppBrosCardBack);
            }
            else if (_currentMenu == _secretaryCardsMenu)
            {
                SetCards(_secretaryCardsList, DeckManager.Instance.GetSecretaryExtraCards(), _secretaryCardBack);
            }
            else if (_currentMenu == _bossCardsMenu)
            {
                SetCards(_bossCardsList, DeckManager.Instance.GetBossExtraCards(), _bossCardBack);
            }
        }

        private void SetButtonsAvailable()
        {
            bool flag0, flag1, flag2, flag3;

            flag0 = FlagManager.Instance.GetFlag(Flag.Reference.Car0);
            flag1 = FlagManager.Instance.GetFlag(Flag.Reference.Car1);
            flag2 = FlagManager.Instance.GetFlag(Flag.Reference.Car2);
            flag3 = FlagManager.Instance.GetFlag(Flag.Reference.Car3);

            SetAvailable(_playerButton, flag0, GetInteractable(_playerCardsMenu));
            SetAvailable(_citrianoButton, flag1, GetInteractable(_citrianoCardsMenu));
            SetAvailable(_ppBrosButton, flag2, GetInteractable(_ppBrosCardsMenu));
            SetAvailable(_secretaryButton, flag3, GetInteractable(_secretaryCardsMenu));
            SetAvailable(_bossButton, flag3, GetInteractable(_bossCardsMenu));
        }

        private bool GetInteractable(GameObject menu)
        {
            return _currentMenu != menu;
        }

        private void SetAvailable(Button button, bool available, bool interactable = false)
        {
            if (available)
            {
                button.image.color = Color.white;
                button.interactable = interactable;
            }
            else
            {
                button.image.color = Color.black;
                button.interactable = false;
            }
        }

        private void SetCards(GameObject cardsParent, List<CardsData> cards, Sprite cardBack)
        {
            int index = 0;
            foreach (Transform cardObject in cardsParent.transform)
            {
                CardsData cardData = cards[index];
                Image image = cardObject.GetComponent<Image>();

                if (cardData == null)
                {
                    image.color = Color.grey;
                    image.sprite = cardBack;
                }
                else
                {
                    image.color = Color.white;
                    cardObject.GetComponent<CardUI>().Initialize(cardData);
                }

                index++;
            }
        }

        #region Buttons Click
        public void OnPlayerButtonClick()
        {
            _currentMenu.SetActive(false);
            _playerCardsMenu.SetActive(true);
            _currentMenu = _playerCardsMenu;
            UpdateExtraCards();
        }

        public void OnCitrianoButtonClick()
        {
            _currentMenu.SetActive(false);
            _citrianoCardsMenu.SetActive(true);
            _currentMenu = _citrianoCardsMenu;
            UpdateExtraCards();
        }

        public void OnPPBrosButtonClick()
        {
            _currentMenu.SetActive(false);
            _ppBrosCardsMenu.SetActive(true);
            _currentMenu = _ppBrosCardsMenu;
            UpdateExtraCards();
        }

        public void OnSecretaryButtonClick()
        {
            _currentMenu.SetActive(false);
            _secretaryCardsMenu.SetActive(true);
            _currentMenu = _secretaryCardsMenu;
            UpdateExtraCards();
        }

        public void OnBossButtonClick()
        {
            _currentMenu.SetActive(false);
            _bossCardsMenu.SetActive(true);
            _currentMenu = _bossCardsMenu;
            UpdateExtraCards();
        }

        #endregion
    }
}