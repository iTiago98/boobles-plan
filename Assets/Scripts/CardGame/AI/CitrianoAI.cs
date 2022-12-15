using CardGame.Cards;
using CardGame.Level;
using CardGame.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CardGame.AI
{
    public class CitrianoAI : OpponentAI
    {
        protected override bool ArgumentIsGoodChoice(Card argument)
        {
            switch (argument.name)
            {
                case "Antiestreñimiento":
                    return _contender.currentMana == _contender.currentMaxMana
                        && _contender.hand.numCards <= CardGameManager.Instance.settings.handCapacity - argument.effect.intParameter1;

                case "Antioxidante":
                case "Antiarrugas":
                case "Vitamina C":
                case "5 piezas de fruta al día":
                default:
                    return true;
            }

        }

        protected override bool GetBestPosition(Card argument, CardZone cardZone, Card oppositeCard, CardZone bestCardZone, Card bestOppositeCard)
        {
            switch (argument.name)
            {
                case "5 piezas de fruta al día":
                    return GetWeakestOppositeCard(oppositeCard, bestOppositeCard);

                case "Antiarrugas":
                    return GetStrongestOppositeCard(oppositeCard, bestOppositeCard);

                case "Antiestreñimiento":
                case "Antioxidante":
                case "Vitamina C":
                default:
                    return DefaultGetBestPosition(oppositeCard, bestOppositeCard);
            }
        }
    }
}
