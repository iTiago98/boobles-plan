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
    
    public CardsDataContainer playerDeckBase;

    public List<CardsDataContainer> opponentsDecks;

    [Header("Extra Cards")]
    public List<CardsData> playerExtraCards;
    public List<CardsData> citrianoExtraCards;

    private List<CardsData> _playerDeck;
    private List<CardsData> _opponentDeck;

    private Opponent_Name opponentName;

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
        return opponentName;
    }

    public void SetPlayerCards()
    {
        SetDeck(playerDeckBase, ref _playerDeck);
    }

    public void SetOpponentCards(string opponentName)
    {
        CardsDataContainer opponentDeck = null;
        switch(opponentName)
        {
            case "Tutorial":
                this.opponentName = Opponent_Name.Tutorial;
                opponentDeck = opponentsDecks[0];
                break;
            case "Citriano":
                this.opponentName = Opponent_Name.Citriano;
                opponentDeck = opponentsDecks[1];
                break;
            case "PingPongBros":
                this.opponentName = Opponent_Name.PingPongBros;
                opponentDeck = opponentsDecks[2]; 
                break;
            case "Secretaria":
                this.opponentName = Opponent_Name.Secretaria;
                opponentDeck = opponentsDecks[3]; 
                break;
            case "Jefe":
                this.opponentName = Opponent_Name.Jefe;
                opponentDeck = opponentsDecks[4]; 
                break;
        }

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
}

