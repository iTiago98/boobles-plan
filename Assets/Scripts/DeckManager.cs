 using System.Collections.Generic;
using UnityEngine;
using Booble.CardGame;
using Booble.CardGame.Cards.DataModel;
using Booble.CardGame.AI;

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
        }

        public void AddCard(CardsData cardsData)
        {
            _playerDeck.Add(cardsData);
        }

        public List<CardsData> GetPlayerCards() => _playerDeck;
        public List<CardsData> GetOpponentCards() => _opponentDeck;
        public Opponent_Name GetOpponentName() => _opponentName;

        public void SetPlayerCards()
        {
            SetDeck(playerDeckBase, ref _playerDeck);
        }

        public void SetOpponent(Opponent_Name opponentName)
        {
            _opponentName = opponentName;
        }

        public void SetOpponentCards(CardsDataContainer deck)
        {
            SetDeck(deck, ref _opponentDeck);
        }

        private void SetDeck(CardsDataContainer source, ref List<CardsData> dest)
        {
            dest = new List<CardsData>();
            foreach (CardsData data in source.cards)
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

        #region Add Extra Cards

        public void AddGranFinal()
        {
            Debug.Log("ADD1");
            AddCard(playerExtraCards[0]);
        }

        List<int> indexToRemove = new List<int>();

        public void RemoveExtraCards()
        {
            if (_opponentName != Opponent_Name.Citriano) RemoveCitrianoCards();
            if (_opponentName != Opponent_Name.PingPongBros) RemovePPBrosCards();
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

        #region Citriano Cards

        public void AddCitrianoCards()
        {
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
            AddCard(citrianoExtraCards[0]);
        }

        public void AddNuevaCepaDelEscorbuto()
        {
            AddCard(citrianoExtraCards[1]);
        }

        public void AddExprimirLaVerdad()
        {
            AddCard(citrianoExtraCards[2]);
        }

        public void AddMaquinaDeZumo()
        {
            AddCard(citrianoExtraCards[3]);
        }

        #endregion

        #region PPBros Cards

        public void AddPPBrosCards()
        {
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
            AddCard(ppBrosExtraCards[0]);
        }

        public void AddPared()
        {
            AddCard(ppBrosExtraCards[1]);
        }

        public void AddPalaDeNocobich()
        {
            AddCard(ppBrosExtraCards[2]);
        }

        public void AddGomuGomuNo()
        {
            AddCard(ppBrosExtraCards[3]);
        }

        public void AddPelotaBomba()
        {
            AddCard(ppBrosExtraCards[4]);
        }

        #endregion

        #region Secretary Cards

        public void AddSecretaryCards()
        {
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
            AddCard(secretaryExtraCards[0]);
        }

        public void AddTraigoLosAnexosCorrespondientes()
        {
            AddCard(secretaryExtraCards[1]);
        }

        public void AddAfidavit()
        {
            AddCard(secretaryExtraCards[2]);
        }

        public void AddResaltarUnaContradiccion()
        {
            AddCard(secretaryExtraCards[3]);
        }

        #endregion

        #region Boss Extra Cards

        public void AddBossCards()
        {
            AddHipervitaminadoPlus();
            AddVictoriaPorDesgastePlus();
            AddHaPerdidoUsteLosPapelePlus();
        }

        public void AddHipervitaminadoPlus()
        {
            AddCard(bossExtraCards[0]);
        }

        public void AddVictoriaPorDesgastePlus()
        {
            AddCard(bossExtraCards[1]);
        }

        public void AddHaPerdidoUsteLosPapelePlus()
        {
            AddCard(bossExtraCards[2]);
        }

        #endregion

        #endregion
    }

}