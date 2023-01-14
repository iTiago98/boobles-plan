using System.Collections.Generic;
using UnityEngine;
using Booble.CardGame;
using Booble.CardGame.Cards.DataModel;
using Booble.CardGame.AI;
using Booble.UI;
using Booble.Flags;
using static Booble.Flags.Flag;

namespace Booble.Managers
{
    public class DeckManager : DontDestroyOnLoad<DeckManager>
    {
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

        private List<CardsData> _playerAuxDeck;

        public List<CardsData> GetPlayerCards(bool playingStoryMode) => playingStoryMode ? _playerDeck : _playerAuxDeck;

        private void AddCard(CardsData cardsData, Opponent_Name opponentName, bool aux = false, bool showAddedText = true, bool showAlertIcons = true)
        {
            if (aux)
            {
                _playerAuxDeck.Add(cardsData);
            }
            else
            {
                _playerDeck.Add(cardsData);
                AddNewCard(cardsData, opponentName, showAddedText, showAlertIcons);
            }
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

        #region Story Mode

        public void SetBaseDeck()
        {
            SetDeck(playerDeckBase.cards, ref _playerDeck);
        }

        public List<CardsData> GetOpponentCards() => _opponentDeck;
        public void SetOpponentCards(CardsDataContainer deck)
        {
            SetDeck(deck.cards, ref _opponentDeck);
        }

        public Opponent_Name GetOpponentName() => _opponentName;
        public void SetOpponent(Opponent_Name opponentName)
        {
            _opponentName = opponentName;
        }

        public List<CardsData> GetPlayerBaseCards()
        {
            List<CardsData> copy = new List<CardsData>();
            SetDeck(playerDeckBase.cards, ref copy);
            return copy;
        }

        #endregion

        public void InitializeAuxDeck(Opponent_Name name)
        {
            SetDeck(playerDeckBase.cards, ref _playerAuxDeck);
            SetOpponent(name);
            AddExtraCards(name);
        }

        private void AddExtraCards(Opponent_Name name)
        {
            switch (name)
            {
                case Opponent_Name.Tutorial: break;
                case Opponent_Name.Citriano: AddCitrianoCards(); break;
                case Opponent_Name.PPBros: AddPPBrosCards(); break;
                case Opponent_Name.Secretary: AddSecretaryCards(); break;
                case Opponent_Name.Boss: AddBossCards(); break;
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

        public void AddNewCard(CardsData data, Opponent_Name name, bool showAddedText, bool showAlertIcons)
        {
            CardData temp = new CardData();
            temp.data = data;
            temp.opponent = name;
            _newCards.Add(temp);

            PauseMenu.Instance.UpdateAlerts(_newCards, showAddedText, showAlertIcons);
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
                PauseMenu.Instance.UpdateAlerts(_newCards, showAddedText: false, showAlertIcons: false);
            }
        }

        #endregion

        #region Add Extra Cards

        #region Tutorial Cards


        public void AddGranFinal(bool aux = false)
        {
            AddCard(playerExtraCards[0], Opponent_Name.Tutorial, aux);
        }

        #endregion

        #region Citriano Cards

        public void AddCitrianoCards()
        {
            AddHipervitaminado(true);
            AddNuevaCepaDelEscorbuto(true);
            AddExprimirLaVerdad(true);
            AddMaquinaDeZumo(true);
        }

        public void AddHipervitaminado(bool aux = false)
        {
            AddCard(citrianoExtraCards[0], Opponent_Name.Citriano, aux);
        }

        public void AddNuevaCepaDelEscorbuto(bool aux = false)
        {
            AddCard(citrianoExtraCards[1], Opponent_Name.Citriano, aux);
        }

        public void AddExprimirLaVerdad(bool aux = false)
        {
            AddCard(citrianoExtraCards[2], Opponent_Name.Citriano, aux);
        }

        public void AddMaquinaDeZumo(bool aux = false)
        {
            AddCard(citrianoExtraCards[3], Opponent_Name.Citriano, aux);
        }

        #endregion

        #region PPBros Cards

        public void AddPPBrosCards()
        {
            AddVictoriaPorDesgaste(true);
            AddPared(true);
            AddPalaDeNocobich(true);
            AddGomuGomuNo(true);
            AddPelotaBomba(true);
        }

        public void AddVictoriaPorDesgaste(bool aux = false)
        {
            AddCard(ppBrosExtraCards[0], Opponent_Name.PPBros, aux);
        }

        public void AddPared(bool aux = false)
        {
            AddCard(ppBrosExtraCards[1], Opponent_Name.PPBros, aux);
        }

        public void AddPalaDeNocobich(bool aux = false)
        {
            AddCard(ppBrosExtraCards[2], Opponent_Name.PPBros, aux);
        }

        public void AddGomuGomuNo(bool aux = false)
        {
            FlagManager.Instance.SetFlag(Flag.Reference.GomuGomuNoObtenida);
            AddCard(ppBrosExtraCards[3], Opponent_Name.PPBros, aux);
        }

        public void AddPelotaBomba(bool aux = false)
        {
            AddCard(ppBrosExtraCards[4], Opponent_Name.PPBros, aux);
        }

        #endregion

        #region Secretary Cards

        public void AddSecretaryCards()
        {
            AddHaPerdidoUsteLosPapele(true);
            AddTraigoLosAnexosCorrespondientes(true);
            AddAfidavit(true);
            AddResaltarUnaContradiccion(true);
        }

        public void AddHaPerdidoUsteLosPapele(bool aux = false)
        {
            AddCard(secretaryExtraCards[0], Opponent_Name.Secretary, aux);
        }

        public void AddTraigoLosAnexosCorrespondientes(bool aux = false)
        {
            AddCard(secretaryExtraCards[1], Opponent_Name.Secretary, aux);
        }

        public void AddAfidavit(bool aux = false)
        {
            AddCard(secretaryExtraCards[2], Opponent_Name.Secretary, aux);
        }

        public void AddResaltarUnaContradiccion(bool aux = false)
        {
            AddCard(secretaryExtraCards[3], Opponent_Name.Secretary, aux);
        }

        #endregion

        #region Boss Extra Cards

        public void AddBossCards()
        {
            AddHipervitaminadoPlus(true);
            AddVictoriaPorDesgastePlus(true);
            AddHaPerdidoUsteLosPapelePlus(true);
        }

        public void AddHipervitaminadoPlus(bool aux = false)
        {
            AddCard(bossExtraCards[0], Opponent_Name.Boss, aux);
        }

        public void AddVictoriaPorDesgastePlus(bool aux = false)
        {
            AddCard(bossExtraCards[1], Opponent_Name.Boss, aux);
        }

        public void AddHaPerdidoUsteLosPapelePlus(bool aux = false)
        {
            AddCard(bossExtraCards[2], Opponent_Name.Boss, aux);
        }

        #endregion

        #endregion

        #region Get Extra Cards

        public void CheckExtraCards()
        {
            List<Reference> flags = new List<Reference>() { Reference.GranFinalObtenida };
            List<CardsData> extraCards = GetPlayerExtraCards();

            SetExtraCards(flags, extraCards, Opponent_Name.Tutorial);

            flags = new List<Reference>() { Reference.HipervitaminadoObtenida, Reference.EscorbutoObtenida, Reference.ExprimirObtenida, Reference.MaquinaZumosObtenida };
            extraCards = GetCitrianoExtraCards();

            SetExtraCards(flags, extraCards, Opponent_Name.Citriano);

            flags = new List<Reference>() { Reference.VictoriaPorDesgasteObtenida, Reference.ParedObtenida, Reference.LaPalaDeNocobichObtenida, Reference.GomuGomuNoObtenida, Reference.PelotaBombaObtenida };
            extraCards = GetPPBrosExtraCards();

            SetExtraCards(flags, extraCards, Opponent_Name.PPBros);
        }

        private void SetExtraCards(List<Reference> flags, List<CardsData> extraCards, Opponent_Name opponentName)
        {
            for (int i = 0; i < extraCards.Count; i++)
            {
                Reference flag = flags[i];
                CardsData card = extraCards[i];

                if (FlagManager.Instance.GetFlag(flag)) AddCard(card, opponentName, aux: false, showAddedText: false, showAlertIcons: false);
            }
        }

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

        private List<CardsData> GetPlayerExtraCards() => playerExtraCards;
        private List<CardsData> GetCitrianoExtraCards() => citrianoExtraCards;
        private List<CardsData> GetPPBrosExtraCards() => ppBrosExtraCards;
        private List<CardsData> GetSecretaryExtraCards() => secretaryExtraCards;
        private List<CardsData> GetBossExtraCards() => bossExtraCards;

        #endregion
    }

}