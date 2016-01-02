
namespace PokerSimulatorEngine
{
    public struct Card
    {
        public static Card Empty = new Card();

        public readonly PokerEnums.CardRank Rank;
        public readonly PokerEnums.CardSuit Suit;

        public Card(PokerEnums.CardRank rank, PokerEnums.CardSuit suit)
        {
            Rank = rank;
            Suit = suit;
        }
    }
}
