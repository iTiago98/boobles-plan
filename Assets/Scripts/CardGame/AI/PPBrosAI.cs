using Booble.CardGame.Cards;
using Booble.CardGame.Level;
using Booble.CardGame.Managers;

namespace Booble.CardGame.AI
{
    public class PPBrosAI : OpponentAI
    {
        protected override bool ArgumentIsGoodChoice(Card argument)
        {
            Contender player = CardGameManager.Instance.player;

            switch (argument.name)
            {
                case "Backspin":
                    return Board.Instance.NumCardsOnTable(player) > 0;

                case "La cinta de Fereder":
                    return _contender.hand.numCards > 1;

                case "Apurando la línea":
                    return player.hand.numCards > _contender.hand.numCards;

                case "Dejadita":
                    return Board.Instance.NumCardsOnTable(player) >= 2;

                case "Matar la bola":
                case "Piel de plátano de Rafa Nadar":
                default:
                    return true;
            }
        }

        protected override bool GetBestPosition(Card argument, CardZone cardZone, Card oppositeCard, CardZone bestCardZone, Card bestOppositeCard)
        {
            switch (argument.name)
            {
                case "Backspin":
                    return GetReboundOppositeCard(argument, oppositeCard, bestOppositeCard);

                case "Matar la bola":
                case "Dejadita":
                case "La cinta de Fereder":
                case "Piel de plátano de Rafa Nadar":
                case "Apurando la línea":
                default:
                    return DefaultGetBestPosition(oppositeCard, bestOppositeCard);
            }
        }

        private bool GetReboundOppositeCard(Card argument, Card oppositeCard, Card bestOppositeCard)
        {
            if (oppositeCard != null)
            {
                if (bestOppositeCard == null) return true;

                bool betterStats = GetStats(oppositeCard) > GetStats(bestOppositeCard);
                bool reboundKill = (oppositeCard.Stats.strength > oppositeCard.Stats.defense) && (oppositeCard.Stats.defense <= argument.Stats.defense);

                return reboundKill && betterStats;
            }

            return false;
        }
    }
}