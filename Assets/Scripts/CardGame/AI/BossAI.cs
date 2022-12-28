
using Booble.CardGame.Cards;
using Booble.CardGame.Level;
using Booble.CardGame.Managers;

namespace Booble.CardGame.AI
{
    public class BossAI : OpponentAI
    {
        protected override bool ArgumentIsGoodChoice(Card argument)
        {
            Contender player = CardGameManager.Instance.player;

            switch (argument.name)
            {
                case "Abro paraguas":
                    return Board.Instance.HasCard(_contender, "¡Va a llover!");

                case "Cláusula sorpresa":
                case "¡Seguridad!":
                    return Board.Instance.NumCardsOnTable(player) > 0;

                case "De casa se viene cagao":
                    return Board.Instance.NumCardsOnTable(player) >= 2;

                case "¡Para eso te pago!":
                    return Board.Instance.NumCardsOnTable(player) > Board.Instance.NumCardsOnTable(_contender);

                default:
                    return true;
            }
        }

        protected override bool GetBestPosition(Card argument, CardZone cardZone, Card oppositeCard, CardZone bestCardZone, Card bestOppositeCard)
        {
            switch (argument.name)
            {
                case "Cláusula sorpresa":
                case "¡Seguridad!":
                    return GetStrongestOppositeCard(oppositeCard, bestOppositeCard);

                case "Abro paraguas":
                case "¡Para eso te pago!":
                    return GetWeakestOppositeCard(oppositeCard, bestOppositeCard);

                case "De casa se viene cagao":
                default:
                    return DefaultGetBestPosition(oppositeCard, bestOppositeCard);
            }
        }
    }
}
