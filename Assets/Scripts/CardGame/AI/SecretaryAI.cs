using Booble.CardGame.Cards;
using Booble.CardGame.Level;
using Booble.CardGame.Managers;

namespace Booble.CardGame.AI
{
    public class SecretaryAI : OpponentAI
    {
        protected override bool ArgumentIsGoodChoice(Card argument)
        {
            Contender player = CardGameManager.Instance.player;

            switch (argument.name)
            {
                case "Es mi hora de descanso":
                    return Board.Instance.NumCardsOnTable(player) > 0;

                case "Firme aqu�":
                    return Board.Instance.NumCardsOnTable(player) < player.cardZones.Count - 1
                        && Board.Instance.NumCardsOnTable(player) >= 2;

                case "Solo se aceptan firmas en tinta negra":
                    return player.fieldCardZone.isNotEmpty;

                case "No soy tu chacha":
                    return Board.Instance.NumCardsOnTable(_contender) >= 2;

                case "�rdenes de arriba":
                    return Board.Instance.NumCardsOnTable(player) > Board.Instance.NumCardsOnTable(_contender);

                case "No s�, no soy de arte":
                case "Te falta rellenar el formulario":
                default:
                    return true;
            }
        }

        protected override bool GetBestPosition(Card argument, CardZone cardZone, Card oppositeCard, CardZone bestCardZone, Card bestOppositeCard)
        {
            switch (argument.name)
            {
                case "Es mi hora de descanso":
                    return GetStrongestOppositeCard(oppositeCard, bestOppositeCard);

                case "No s�, no soy de arte":
                case "Te falta rellenar el formulario":
                case "Firme aqu�":
                case "Solo se aceptan firmas en tinta negra":
                case "No soy tu chacha":
                case "�rdenes de arriba":
                default:
                    return DefaultGetBestPosition(oppositeCard, bestOppositeCard);
            }
        }
    }
}