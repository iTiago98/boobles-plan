namespace DataModel
{
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
    }
}