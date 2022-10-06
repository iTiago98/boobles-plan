using CardGame;
using CardGame.Cards.DataModel;
using CardGame.Cards.DataModel.Effects;
using Santi.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : Singleton<DeckManager>
{

    public CardsDataContainer playerDeckBase;

    public List<CardsDataContainer> opponentsDecks;

    private List<CardsData> _playerDeck;
    private List<CardsData> _opponentDeck;

    private Opponent_Name opponentName;


    //private CardsData _granFinal;

    private void Start()
    {
        SetPlayerCards();
    }

    //public void AddGranFinal()
    //{
    //    AddCard(_granFinal);
    //}

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
            case "Mondaroriano":
                this.opponentName = Opponent_Name.Mondaroriano;
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

            //if(temp.name == "Gran final")
            //{
            //    _granFinal = temp;
            //}
            //else
            //{
            dest.Add(temp);
            //}
        }
    }


}

