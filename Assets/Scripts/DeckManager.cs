using System;
using CardGame;
using CardGame.Cards.DataModel;
using CardGame.Cards.DataModel.Effects;
using Santi.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public static DeckManager Instance { get; private set; }
    
    [SerializeField] private CardsDataContainer playerDeckBase;

    [SerializeField] private List<CardsDataContainer> opponentsDecks;

    [Header("Extra Cards")]
    [SerializeField] private List<CardsData> playerExtraCards;
    [SerializeField] private List<CardsData> citrianoExtraCards;
    [SerializeField] private List<CardsData> pinponbrosExtraCards;
    [SerializeField] private List<CardsData> secretaryExtraCards;

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

    public List<CardsData> GetPlayerCards()
    {
        return _playerDeck;
    }

    public List<CardsData> GetOpponentCards()
    {
        return _opponentDeck;
    }

    public Opponent_Name GetOpponentName()
    {
        return _opponentName;
    }

    public void SetPlayerCards()
    {
        SetDeck(playerDeckBase, ref _playerDeck);
        AddSecretaryCards();
    }

    public void SetOpponentCards(Opponent_Name opponentName)
    {
        CardsDataContainer opponentDeck = null;
        switch(opponentName)
        {
            case Opponent_Name.Tutorial:
                opponentDeck = opponentsDecks[0];
                break;
            case Opponent_Name.Citriano:
                opponentDeck = opponentsDecks[1];
                break;
            case Opponent_Name.PingPongBros:
                opponentDeck = opponentsDecks[2]; 
                break;
            case Opponent_Name.Secretary:
                opponentDeck = opponentsDecks[3]; 
                break;
            case Opponent_Name.Jefe:
                opponentDeck = opponentsDecks[4]; 
                break;
        }
        _opponentName = opponentName;

        SetDeck(opponentDeck, ref _opponentDeck);
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

    public void AddCitrianoCards()
    {
        AddHipervitaminado();
        AddNuevaCepaDelEscorbuto();
        AddExprimirLaVerdad();
        AddMaquinaDeZumo();
    }

    public void RemoveCitrianoCards()
    {
        foreach(CardsData card in _playerDeck)
        {
            if (citrianoExtraCards.Contains(card)) _playerDeck.Remove(card);
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

    public void AddPPBrosCards()
    {
        AddVictoriaPorDesgaste();
        AddPared();
        AddPalaDeNocobich();
        AddGomuGomuNo();
        AddPelotaBomba();
    }

    public void AddVictoriaPorDesgaste()
    {
        AddCard(pinponbrosExtraCards[0]);
    }

    public void AddPared()
    {
        AddCard(pinponbrosExtraCards[1]);
    }
    
    public void AddPalaDeNocobich()
    {
        AddCard(pinponbrosExtraCards[2]);
    }

    public void AddGomuGomuNo()
    {
        AddCard(pinponbrosExtraCards[3]);
    }

    public void AddPelotaBomba()
    {
        AddCard(pinponbrosExtraCards[4]);
    }

    public void AddSecretaryCards()
    {
        AddHaPerdidoUsteLosPapele();
        AddTraigoLosAnexosCorrespondientes();
        AddAfidavit();
        AddResaltarUnaContradiccion();
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
}

