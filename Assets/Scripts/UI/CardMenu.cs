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

        public void SetCardsMenu()
        {
            if (!_initialized) SetPlayerCards();
            //SetButtonsAvailable();
        }

        private void SetPlayerCards()
        {
            List<CardsData> cards = DeckManager.Instance.GetPlayerCards();
            cards.Add(DeckManager.Instance.GetGranFinal());

            SetCards(null, _playerCardsList, cards, _playerCardBack);

            _currentMenu = _playerCardsMenu;
            _initialized = true;
        }

        private void SetButtonsAvailable()
        {
            _citrianoButton.interactable = FlagManager.Instance.GetFlag(Flag.Reference.Car1);
            _ppBrosButton.interactable = FlagManager.Instance.GetFlag(Flag.Reference.Car2);
            _secretaryButton.interactable = FlagManager.Instance.GetFlag(Flag.Reference.Car3);
            _bossButton.interactable = FlagManager.Instance.GetFlag(Flag.Reference.Car3);
        }

        private void SetCards(GameObject parent, GameObject cardsParent, List<CardsData> cards, Sprite cardBack)
        {
            if (parent != null) parent.SetActive(true);

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

        public void OnPlayerButtonClick()
        {
            _currentMenu.SetActive(false);
            _playerCardsMenu.SetActive(true);
            _currentMenu = _playerCardsMenu;
        }

        public void OnCitrianoButtonClick()
        {
            _currentMenu.SetActive(false);
            _citrianoCardsMenu.SetActive(true);

            SetCards(_citrianoCardsMenu, _citrianoCardsList, DeckManager.Instance.GetCitrianoExtraCards(), _citrianoCardBack);

            _currentMenu = _citrianoCardsMenu;
        }
        
        public void OnPPBrosButtonClick()
        {
            _currentMenu.SetActive(false);
            _ppBrosCardsMenu.SetActive(true);

            SetCards(_ppBrosCardsMenu, _ppBrosCardsList, DeckManager.Instance.GetPPBrosExtraCards(), _ppBrosCardBack);

            _currentMenu = _ppBrosCardsMenu;
        }
        
        public void OnSecretaryButtonClick()
        {
            _currentMenu.SetActive(false);
            _secretaryCardsMenu.SetActive(true);

            SetCards(_secretaryCardsMenu, _secretaryCardsList, DeckManager.Instance.GetSecretaryExtraCards(), _secretaryCardBack);

            _currentMenu = _secretaryCardsMenu;
        }
        
        public void OnBossButtonClick()
        {
            _currentMenu.SetActive(false);
            _bossCardsMenu.SetActive(true);

            SetCards(_bossCardsMenu, _bossCardsList, DeckManager.Instance.GetBossExtraCards(), _bossCardBack);

            _currentMenu = _bossCardsMenu;
        }


    }
}