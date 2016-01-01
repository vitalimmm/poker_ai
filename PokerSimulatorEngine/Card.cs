
namespace PokerSimulatorEngine
{
    public class Card
    {
        public Card(PokerEnums.CardRank rank, PokerEnums.CardSuit suit)
        {
            Rank = rank;
            Suit = suit;
        }

        public PokerEnums.CardRank Rank
        {
            get;
            private set;
        }

        public PokerEnums.CardSuit Suit
        {
            get;
            private set;
        }
    }
}
