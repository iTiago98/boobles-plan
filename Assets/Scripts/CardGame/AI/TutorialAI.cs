using CardGame.Cards;
using CardGame.Cards.DataModel.Effects;
using CardGame.Level;
using CardGame.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame.AI
{
    public class TutorialAI : BaseAI
    {
        protected override bool IsGoodChoice(CardEffect effect)
        {
            Contender player = CardGameManager.Instance.player;
            Contender opponent = CardGameManager.Instance.opponent;

            List<CardZone> playerCardZones = Board.Instance.GetCardZones(player);
            List<CardZone> opponentCardZones = Board.Instance.GetCardZones(opponent);

            switch (effect.subType)
            {
                case SubType.RETURN_CARD:
                    return playerCardZones.Count > opponentCardZones.Count;

                case SubType.SWAP_POSITION:
                    return true;

                case SubType.STAT_BOOST:
                    return GetStatsSummary(player, playerCardZones, opponent, opponentCardZones) > 0;

                case SubType.DRAW_CARD:
                    return Board.Instance.GetHand(opponent).numCards <= 3;

                case SubType.DESTROY_CARD:
                    if (effect.targetType == Target.CARD)
                    {
                        if (Board.Instance.GetFieldCardZone(player).GetCard() != null) return true;

                        return playerCardZones.Count > opponentCardZones.Count;
                    }
                    else if (effect.targetType == Target.ACARD)
                    {
                        return GetStatsSummary(player, playerCardZones, opponent, opponentCardZones) > 0;
                    }
                    break;

                default:
                    return true;
            }

            return true;
        }

        private int GetStatsSummary(Contender player, List<CardZone> playerCardZones, Contender opponent, List<CardZone> opponentCardZones)
        {
            int temp = 0;
            foreach (CardZone cardZone in playerCardZones) temp += GetStats(cardZone.GetCard());
            if (Board.Instance.GetFieldCardZone(player).GetCard() != null) temp += 5;

            foreach (CardZone cardZone in opponentCardZones) temp -= GetStats(cardZone.GetCard());
            if (Board.Instance.GetFieldCardZone(opponent).GetCard() != null) temp -= 5;

            return temp;
        }

        private int GetStats(Card card)
        {
            if (card != null) return card.strength + card.defense;
            else return 0;
        }
    }
}