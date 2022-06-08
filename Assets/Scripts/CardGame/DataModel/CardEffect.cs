using CardGame.Level;
using CardGame.Managers;
using System;

namespace CardGame.Cards.DataModel
{
    [Serializable]
    public class CardEffect
    {
        public enum Effect
        {
            NONE, STAT_BOOST, DRAW_CARDS, DISCARD_CARDS, vinculo_vital
        }

        public Effect effect;

        public int strengthBoost;
        public int defenseBoost;

        public int numberCards;

        public void Apply(Card target)
        {
            switch (effect)
            {
                case Effect.NONE:
                    break;
                case Effect.STAT_BOOST:
                    target.BoostStats(strengthBoost, defenseBoost);
                    break;
                case Effect.DRAW_CARDS:
                    Board.Instance.DrawCards(numberCards, TurnManager.Instance.currentPlayer);
                    break;
                case Effect.DISCARD_CARDS:
                    Board.Instance.DiscardCards(numberCards, TurnManager.Instance.currentPlayer);
                    break;
                case Effect.vinculo_vital:
                    break;
            }
        }

        internal bool IsAppliable()
        {
            switch (effect)
            {
                case Effect.NONE:
                case Effect.DRAW_CARDS:
                case Effect.DISCARD_CARDS:
                    return true;
                case Effect.vinculo_vital:
                    break;
                case Effect.STAT_BOOST:
                    return Board.Instance.CardsOnTable(TurnManager.Instance.currentPlayer);
            }

            return false;
        }

        public bool NoTarget()
        {
            switch (effect)
            {
                case Effect.NONE:
                case Effect.DRAW_CARDS:
                case Effect.DISCARD_CARDS:
                    return true;
                case Effect.vinculo_vital:
                    break;
                case Effect.STAT_BOOST:
                    return false;
            }

            return false;
        }
    }
}