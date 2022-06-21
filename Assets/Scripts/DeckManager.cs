using CardGame.Cards.DataModel;
using CardGame.Cards.DataModel.Effects;
using Santi.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : Singleton<DeckManager>
{

    public CardsDataContainer playerDeckBase;

    private List<CardsData> _playerDeck;
    private List<CardsData> _opponentDeck;

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

    public void SetPlayerCards()
    {
        SetDeck(playerDeckBase, ref _playerDeck);
    }

    public void SetOpponentCards(CardsDataContainer opponentDeckBase)
    {
        SetDeck(opponentDeckBase, ref _opponentDeck);
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


}

