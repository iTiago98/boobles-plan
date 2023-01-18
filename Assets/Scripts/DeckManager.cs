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

        public List<CardsData> GetPlayerCards(bool playingStoryMode)
        {
            InitializeAuxDeck(playingStoryMode);
            return _playerAuxDeck;
        }

        private void AddCard(CardsData cardsData, Opponent_Name opponentName, bool newCard = true)
        {
            _playerDeck.Add(cardsData);
            if (newCard) AddNewCard(cardsData, opponentName);
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



        #region Extra Cards

        public void InitializeAuxDeck(bool playingStoryMode)
        {
            SetDeck(playerDeckBase.cards, ref _playerAuxDeck);

            if (playingStoryMode)
            {
                List<CardsData> extraCards = new List<CardsData>(GetExtraCards(Opponent_Name.Tutorial));

                if (_opponentName != Opponent_Name.Tutorial) extraCards.AddRange(GetExtraCards(_opponentName));

                foreach (CardsData card in extraCards)
                {
                    if (_playerDeck.Contains(card) && !_playerAuxDeck.Contains(card))
                        _playerAuxDeck.Add(card);
                }
            }
            else
            {
                AddExtraCards(_opponentName);
            }
        }

        public bool HasAlternateWinConditionCard()
        {
            if (_opponentName == Opponent_Name.Tutorial) return false;
            return _playerAuxDeck.Contains(GetExtraCards(_opponentName)[0]);
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

            PauseMenu.Instance.AddNewCard(temp);
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
                PauseMenu.Instance.RemoveAlert(_newCards[indexToRemove], _newCards.Count - 1);
                _newCards.RemoveAt(indexToRemove);
            }
        }

        public int GetNewCardsCountFromOpponent(Opponent_Name opponentName)
        {
            int count = 0;
            foreach (CardData card in _newCards)
            {
                if (card.opponent == opponentName) count++;
            }
            return count;
        }

        #endregion

        #region Add Extra Cards

        #region Quick Access

        public void AddGranFinal() { AddCard(playerExtraCards[0], Opponent_Name.Tutorial); }

        public void AddHipervitaminado() { AddCard(citrianoExtraCards[0], Opponent_Name.Citriano); }
        public void AddNuevaCepaDelEscorbuto() { AddCard(citrianoExtraCards[1], Opponent_Name.Citriano); }
        public void AddExprimirLaVerdad() { AddCard(citrianoExtraCards[2], Opponent_Name.Citriano); }
        public void AddMaquinaDeZumo() { AddCard(citrianoExtraCards[3], Opponent_Name.Citriano); }

        public void AddVictoriaPorDesgaste() { AddCard(ppBrosExtraCards[0], Opponent_Name.PPBros); }
        public void AddPared() { AddCard(ppBrosExtraCards[1], Opponent_Name.PPBros); }
        public void AddPalaDeNocobich() { AddCard(ppBrosExtraCards[2], Opponent_Name.PPBros); }
        public void AddGomuGomuNo() { AddCard(ppBrosExtraCards[3], Opponent_Name.PPBros); }
        public void AddPelotaBomba() { AddCard(ppBrosExtraCards[4], Opponent_Name.PPBros); }

        public void AddHaPerdidoUsteLosPapele() { AddCard(secretaryExtraCards[0], Opponent_Name.Secretary); }
        public void AddTraigoLosAnexosCorrespondientes() { AddCard(secretaryExtraCards[1], Opponent_Name.Secretary); }
        public void AddAfidavit() { AddCard(secretaryExtraCards[2], Opponent_Name.Secretary); }
        public void AddResaltarUnaContradiccion() { AddCard(secretaryExtraCards[3], Opponent_Name.Secretary); }

        public void AddHipervitaminadoPlus() { AddCard(bossExtraCards[0], Opponent_Name.Boss); }
        public void AddVictoriaPorDesgastePlus() { AddCard(bossExtraCards[1], Opponent_Name.Boss); }
        public void AddHaPerdidoUsteLosPapelePlus() { AddCard(bossExtraCards[2], Opponent_Name.Boss); }

        #endregion

        #region Game Mode

        private void AddExtraCards(Opponent_Name name)
        {
            switch (name)
            {
                case Opponent_Name.Tutorial: AddTutorialCards(); break;
                case Opponent_Name.Citriano: AddCitrianoCards(); break;
                case Opponent_Name.PPBros: AddPPBrosCards(); break;
                case Opponent_Name.Secretary: AddSecretaryCards(); break;
                case Opponent_Name.Boss: AddBossCards(); break;
            }
        }

        public void AddTutorialCards()
        {
            foreach (CardsData card in playerExtraCards)
            {
                _playerAuxDeck.Add(card);
            }
        }

        public void AddCitrianoCards()
        {
            foreach (CardsData card in citrianoExtraCards)
            {
                _playerAuxDeck.Add(card);
            }
        }

        public void AddPPBrosCards()
        {
            foreach (CardsData card in ppBrosExtraCards)
            {
                _playerAuxDeck.Add(card);
            }
        }

        public void AddSecretaryCards()
        {
            foreach (CardsData card in secretaryExtraCards)
            {
                _playerAuxDeck.Add(card);
            }
        }

        public void AddBossCards()
        {
            foreach (CardsData card in bossExtraCards)
            {
                _playerAuxDeck.Add(card);
            }
        }

        #endregion

        #endregion

        #region Get Extra Cards

        public void CheckExtraCards()
        {
            List<Reference> flags = new List<Reference>() { Reference.GranFinalObtenida };
            SetExtraCards(flags, playerExtraCards, Opponent_Name.Tutorial);

            flags = new List<Reference>() { Reference.HipervitaminadoObtenida, Reference.EscorbutoObtenida,
                Reference.ExprimirObtenida, Reference.MaquinaZumosObtenida };

            SetExtraCards(flags, citrianoExtraCards, Opponent_Name.Citriano);

            flags = new List<Reference>() { Reference.VictoriaPorDesgasteObtenida, Reference.ParedObtenida,
                Reference.LaPalaDeNocobichObtenida, Reference.GomuGomuNoObtenida, Reference.PelotaBombaObtenida };

            SetExtraCards(flags, ppBrosExtraCards, Opponent_Name.PPBros);

            flags = new List<Reference>() { Reference.PerderLosPapelesObtenida, Reference.AnexosCorrespondientesObtenida,
                Reference.AfidavitObtenida, Reference.ResaltarUnaContradiccionObtenida };

            SetExtraCards(flags, secretaryExtraCards, Opponent_Name.Secretary);

            if (FlagManager.Instance.GetFlag(Reference.BossHall4))
            {
                flags = new List<Reference>() { Reference.CitrianoVictoriaAlternativa, Reference.PPBVictoriaAlternativa,
                Reference.SecretaryVictoriaAlternativa };

                SetExtraCards(flags, bossExtraCards, Opponent_Name.Boss);
            }
        }

        private void SetExtraCards(List<Reference> flags, List<CardsData> extraCards, Opponent_Name opponentName)
        {
            for (int i = 0; i < extraCards.Count; i++)
            {
                Reference flag = flags[i];
                CardsData card = extraCards[i];

                if (FlagManager.Instance.GetFlag(flag)) AddCard(card, opponentName, newCard: false);
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
                case Opponent_Name.Tutorial: return playerExtraCards;
                case Opponent_Name.Citriano: return citrianoExtraCards;
                case Opponent_Name.PPBros: return ppBrosExtraCards;
                case Opponent_Name.Secretary: return secretaryExtraCards;
                case Opponent_Name.Boss: return bossExtraCards;

                default:
                    return null;
            }
        }

        #endregion

        #endregion
    }

}