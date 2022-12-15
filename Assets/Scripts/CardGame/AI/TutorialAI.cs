using CardGame.Cards;
using CardGame.Level;
using CardGame.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame.AI
{
    public class TutorialAI : OpponentAI
    {
        protected override bool ArgumentIsGoodChoice(Card argument)
        {
            Contender player = CardGameManager.Instance.player;

            switch (argument.name)
            {
                case "Y por ende":
                    return _contender.currentMana == _contender.currentMaxMana
                        && _contender.hand.numCards <= CardGameManager.Instance.settings.handCapacity - argument.effect.intParameter1;

                case "Lo siguiente que vas a decir es":
                    return player.hand.numCards >= argument.effect.intParameter1;

                case "Gran final":
                    return Board.Instance.NumCardsOnTable(_contender) >= 2;

                case "Afirmación reconfortante":
                case "Premisa":
                case "Razonamiento coherente":
                case "Técnicamente correcto":
                case "Aquí están las pruebas que lo respaldan":
                default:
                    return true;
            }
        }

        protected override bool GetBestPosition(Card argument, CardZone cardZone, Card oppositeCard, CardZone bestCardZone, Card bestOppositeCard)
        {
            switch (argument.name)
            {
                case "Afirmación reconfortante":
                case "Premisa":
                case "Razonamiento coherente":
                case "Técnicamente correcto":
                case "Y por ende":
                case "Lo siguiente que vas a decir es":
                case "Aquí están las pruebas que lo respaldan":
                case "Gran final":
                default:
                    return DefaultGetBestPosition(oppositeCard, bestOppositeCard);
            }
        }
    }
}