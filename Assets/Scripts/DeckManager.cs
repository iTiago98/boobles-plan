using System.Collections.Generic;
using UnityEngine;
using Booble.CardGame;
using Booble.CardGame.Cards.DataModel;
using Booble.CardGame.AI;
using Booble.UI;

namespace Booble.Managers
{
    public class DeckManager : MonoBehaviour
    {
        public static DeckManager Instance { get; private set; }

        [SerializeField] private CardsDataContainer playerDeckBase;

        [Header("Extra Cards")]
        [SerializeField] private List<CardsData> playerExtraCards;
        [SerializeField] private List<CardsData> citrianoExtraCards;
        [SerializeField] private List<CardsData> ppBrosExtraCards;
        [SerializeField] private List<CardsData> secretaryExtraCards;
        [SerializeField] private List<CardsData> bossExtraCards;

        private List<CardsData> _playerDeck;
        private List<CardsData> _opponentDeck;

        private Opponent_Name _opponentName;
        private Opponent_Name _previousOpponent;


        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            SetPlayerCards();
            PauseMenu.Instance.InitializeCardMenu();
        }

        public void AddCard(CardsData cardsData, Opponent_Name opponentName)
        {
            _playerDeck.Add(cardsData);
            AddNewCard(cardsData, opponentName);
        }

        public List<CardsData> GetPlayerCards() => _playerDeck;
        public List<CardsData> GetOpponentCards() => _opponentDeck;
        public Opponent_Name GetOpponentName() => _opponentName;

        public void SetPlayerCards()
        {
            SetDeck(playerDeckBase.cards, ref _playerDeck);
        }

        public void SetOpponent(Opponent_Name opponentName)
        {
            _previousOpponent = _opponentName;
            _opponentName = opponentName;
        }

        public void SetOpponentCards(CardsDataContainer deck)
        {
            SetDeck(deck.cards, ref _opponentDeck);
        }
        public List<CardsData> GetPlayerBaseCards()
        {
            List<CardsData> copy = new List<CardsData>();
            SetDeck(playerDeckBase.cards, ref copy);
            return copy;
        }

        private void SetDeck(List<CardsData> source, ref List<CardsData> dest)
        {
            dest = new List<CardsData>();
            foreach (CardsData data in source)
            {
                CardsData temp = new CardsData();
                temp.name = data.name;
                temp.sprite = data.sprite;
                temp.cost = data.cost;
                temp.strength = data.strength;
                temp.defense = data.defense;
                temp.type = data.type;
                temp.effects = data.effects;

                dest.Add(temp);
            }
        }


        #region New Cards

        public struct CardData
        {
            public CardsData data;
            public Opponent_Name opponent;
        }

        private List<CardData> _newCards = new List<CardData>();
        public List<CardData> GetNewCards() => _newCards;

        public void AddNewCard(CardsData data, Opponent_Name name)
        {
            CardData temp = new CardData();
            temp.data = data;
            temp.opponent = name;
            _newCards.Add(temp);

            PauseMenu.Instance.UpdateAlerts();
        }

        public void RemoveNewCard(CardsData data)
        {
            int indexToRemove = -1;
            foreach (CardData card in _newCards)
            {
                if (card.data.name == data.name)
                {
                    indexToRemove = _newCards.IndexOf(card);
                    break;
                }
            }

            if (indexToRemove != -1)
            {
                _newCards.RemoveAt(indexToRemove);
                PauseMenu.Instance.UpdateAlerts();
            }
        }

        #endregion

        #region Add Extra Cards

        #region Tutorial Cards

        public void AddGranFinal()
        {
            AddCard(playerExtraCards[0], Opponent_Name.Tutorial);
        }

        #endregion

        #region Citriano Cards

        public void AddCitrianoCards()
        {
            if (_previousOpponent == Opponent_Name.Citriano) return;

            AddHipervitaminado();
            AddNuevaCepaDelEscorbuto();
            AddExprimirLaVerdad();
            AddMaquinaDeZumo();
        }

        public void RemoveCitrianoCards()
        {
            foreach (CardsData card in _playerDeck)
            {
                if (citrianoExtraCards.Contains(card)) indexToRemove.Add(_playerDeck.IndexOf(card));
            }
        }

        public void AddHipervitaminado()
        {
            AddCard(citrianoExtraCards[0], Opponent_Name.Citriano);
        }

        public void AddNuevaCepaDelEscorbuto()
        {
            AddCard(citrianoExtraCards[1], Opponent_Name.Citriano);
        }

        public void AddExprimirLaVerdad()
        {
            AddCard(citrianoExtraCards[2], Opponent_Name.Citriano);
        }

        public void AddMaquinaDeZumo()
        {
            AddCard(citrianoExtraCards[3], Opponent_Name.Citriano);
        }

        #endregion

        #region PPBros Cards

        public void AddPPBrosCards()
        {
            if (_previousOpponent == Opponent_Name.PPBros) return;

            AddVictoriaPorDesgaste();
            AddPared();
            AddPalaDeNocobich();
            AddGomuGomuNo();
            AddPelotaBomba();
        }

        public void RemovePPBrosCards()
        {
            foreach (CardsData card in _playerDeck)
            {
                if (ppBrosExtraCards.Contains(card)) indexToRemove.Add(_playerDeck.IndexOf(card));
            }
        }

        public void AddVictoriaPorDesgaste()
        {
            AddCard(ppBrosExtraCards[0], Opponent_Name.PPBros);
        }

        public void AddPared()
        {
            AddCard(ppBrosExtraCards[1], Opponent_Name.PPBros);
        }

        public void AddPalaDeNocobich()
        {
            AddCard(ppBrosExtraCards[2], Opponent_Name.PPBros);
        }

        public void AddGomuGomuNo()
        {
            AddCard(ppBrosExtraCards[3], Opponent_Name.PPBros);
        }

        public void AddPelotaBomba()
        {
            AddCard(ppBrosExtraCards[4], Opponent_Name.PPBros);
        }

        #endregion

        #region Secretary Cards

        public void AddSecretaryCards()
        {
            if (_previousOpponent == Opponent_Name.Secretary) return;

            AddHaPerdidoUsteLosPapele();
            AddTraigoLosAnexosCorrespondientes();
            AddAfidavit();
            AddResaltarUnaContradiccion();
        }

        public void RemoveSecretaryCards()
        {
            foreach (CardsData card in _playerDeck)
            {
                if (secretaryExtraCards.Contains(card)) indexToRemove.Add(_playerDeck.IndexOf(card));
            }
        }

        public void AddHaPerdidoUsteLosPapele()
        {
            AddCard(secretaryExtraCards[0], Opponent_Name.Secretary);
        }

        public void AddTraigoLosAnexosCorrespondientes()
        {
            AddCard(secretaryExtraCards[1], Opponent_Name.Secretary);
        }

        public void AddAfidavit()
        {
            AddCard(secretaryExtraCards[2], Opponent_Name.Secretary);
        }

        public void AddResaltarUnaContradiccion()
        {
            AddCard(secretaryExtraCards[3], Opponent_Name.Secretary);
        }

        #endregion

        #region Boss Extra Cards

        public void AddBossCards()
        {
            if (_previousOpponent == Opponent_Name.Boss) return;

            AddHipervitaminadoPlus();
            AddVictoriaPorDesgastePlus();
            AddHaPerdidoUsteLosPapelePlus();
        }

        public void AddHipervitaminadoPlus()
        {
            AddCard(bossExtraCards[0], Opponent_Name.Boss);
        }

        public void AddVictoriaPorDesgastePlus()
        {
            AddCard(bossExtraCards[1], Opponent_Name.Boss);
        }

        public void AddHaPerdidoUsteLosPapelePlus()
        {
            AddCard(bossExtraCards[2], Opponent_Name.Boss);
        }

        #endregion

        #endregion

        #region Remove Extra Cards

        List<int> indexToRemove = new List<int>();
        public void RemoveExtraCards()
        {
            if (_opponentName != Opponent_Name.Citriano) RemoveCitrianoCards();
            if (_opponentName != Opponent_Name.PPBros) RemovePPBrosCards();
            if (_opponentName != Opponent_Name.Secretary) RemoveSecretaryCards();
            RemoveCards();
        }

        private void RemoveCards()
        {
            indexToRemove.Sort();
            indexToRemove.Reverse();
            for (int i = 0; i < indexToRemove.Count; i++)
            {
                int index = indexToRemove[i];
                _playerDeck.RemoveAt(index);
            }
            indexToRemove.Clear();
        }

        #endregion

        #region Get Extra Cards

        public bool PlayerHasCard(CardsData data)
        {
            foreach (CardsData cardsData in _playerDeck)
            {
                if (cardsData.name == data.name) return true;
            }
            return false;
        }

        public List<CardsData> GetExtraCards(Opponent_Name name)
        {
            switch (name)
            {
                case Opponent_Name.Tutorial: return GetPlayerExtraCards();
                case Opponent_Name.Citriano: return GetCitrianoExtraCards();
                case Opponent_Name.PPBros: return GetPPBrosExtraCards();
                case Opponent_Name.Secretary: return GetSecretaryExtraCards();
                case Opponent_Name.Boss: return GetBossExtraCards();

                default:
                    return null;
            }
        }

        private List<CardsData> GetPlayerExtraCards()
        {
            return playerExtraCards;
        }
        private List<CardsData> GetCitrianoExtraCards()
        {
            return citrianoExtraCards;
        }
        private List<CardsData> GetPPBrosExtraCards()
        {
            return ppBrosExtraCards;
        }
        private List<CardsData> GetSecretaryExtraCards()
        {
            return secretaryExtraCards;
        }
        private List<CardsData> GetBossExtraCards()
        {
            return bossExtraCards;
           
        }

        #endregion
    }

}