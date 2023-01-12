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

        public List<CardsData> GetPlayerBaseCards()
        {
            List<CardsData> copy = new List<CardsData>();
            SetDeck(playerDeckBase.cards, ref copy);
            return copy;
        }

        #region Add Extra Cards

        public void AddGranFinal()
        {
            AddCard(playerExtraCards[0]);
        }

        List<int> indexToRemove = new List<int>();
        Opponent_Name _previousOpponent;

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
            if (_previousOpponent == Opponent_Name.Boss) return;

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

        #region Get Extra Cards

        public CardsData GetGranFinal()
        {
            if (_playerDeck.Contains(playerExtraCards[0])) return playerExtraCards[0];
            return null;
        }

        #region Citriano

        public List<CardsData> GetCitrianoExtraCards()
        {
            List<CardsData> cards = new List<CardsData>()
            {
                GetHipervitaminado(),
                GetNuevaCepaDelEscorbuto(),
                GetExprimirLaVerdad(),
                GetMaquinaDeZumos()
            };

            return cards;
        }
        private CardsData GetHipervitaminado()
        {
            if (_playerDeck.Contains(citrianoExtraCards[0])) return citrianoExtraCards[0];
            else return null;
        }
        private CardsData GetNuevaCepaDelEscorbuto()
        {
            if (_playerDeck.Contains(citrianoExtraCards[1])) return citrianoExtraCards[1];
            else return null;
        }
        private CardsData GetExprimirLaVerdad()
        {
            if (_playerDeck.Contains(citrianoExtraCards[2])) return citrianoExtraCards[2];
            else return null;
        }
        private CardsData GetMaquinaDeZumos()
        {
            if (_playerDeck.Contains(citrianoExtraCards[3])) return citrianoExtraCards[3];
            else return null;
        }

        #endregion

        #region PPBros

        public List<CardsData> GetPPBrosExtraCards()
        {
            List<CardsData> cards = new List<CardsData>()
            {
                GetVictoriaPorDesgaste(),
                GetPared(),
                GetPalaDeNocobich(),
                GetGomuGomuNo(),
                GetPelotaBomba()
            };

            return cards;
        }
        private CardsData GetVictoriaPorDesgaste()
        {
            if (_playerDeck.Contains(ppBrosExtraCards[0])) return ppBrosExtraCards[0];
            else return null;
        }
        private CardsData GetPared()
        {
            if (_playerDeck.Contains(ppBrosExtraCards[1])) return ppBrosExtraCards[1];
            else return null;
        }
        private CardsData GetPalaDeNocobich()
        {
            if (_playerDeck.Contains(ppBrosExtraCards[2])) return ppBrosExtraCards[2];
            else return null;
        }
        private CardsData GetGomuGomuNo()
        {
            if (_playerDeck.Contains(ppBrosExtraCards[3])) return ppBrosExtraCards[3];
            else return null;
        }
        private CardsData GetPelotaBomba()
        {
            if (_playerDeck.Contains(ppBrosExtraCards[4])) return ppBrosExtraCards[4];
            else return null;
        }

        #endregion

        #region Secretary

        public List<CardsData> GetSecretaryExtraCards()
        {
            List<CardsData> cards = new List<CardsData>()
            {
                GetHaPerdidoUsteLosPapele(),
                GetTraigoLosAnexosCorrespondientes(),
                GetAfidavit(),
                GetResaltarUnaContradiccion()
            };

            return cards;
        }
        private CardsData GetHaPerdidoUsteLosPapele()
        {
            if (_playerDeck.Contains(secretaryExtraCards[0])) return secretaryExtraCards[0];
            else return null;
        }
        private CardsData GetTraigoLosAnexosCorrespondientes()
        {
            if (_playerDeck.Contains(secretaryExtraCards[1])) return secretaryExtraCards[1];
            else return null;
        }
        private CardsData GetAfidavit()
        {
            if (_playerDeck.Contains(secretaryExtraCards[2])) return secretaryExtraCards[2];
            else return null;
        }
        private CardsData GetResaltarUnaContradiccion()
        {
            if (_playerDeck.Contains(secretaryExtraCards[3])) return secretaryExtraCards[3];
            else return null;
        }

        #endregion

        #region Boss

        public List<CardsData> GetBossExtraCards()
        {
            List<CardsData> cards = new List<CardsData>()
            {
                GetHipervitaminadoPlus(),
                GetVictoriaPorDesgastePlus(),
                GetHaPerdidoUsteLosPapelePlus(),
            };

            return cards;
        }
        private CardsData GetHipervitaminadoPlus()
        {
            if (_playerDeck.Contains(bossExtraCards[0])) return bossExtraCards[0];
            else return null;
        }
        private CardsData GetVictoriaPorDesgastePlus()
        {
            if (_playerDeck.Contains(bossExtraCards[1])) return bossExtraCards[1];
            else return null;
        }
        private CardsData GetHaPerdidoUsteLosPapelePlus()
        {
            if (_playerDeck.Contains(bossExtraCards[2])) return bossExtraCards[2];
            else return null;
        }

        #endregion

        #endregion
    }

}