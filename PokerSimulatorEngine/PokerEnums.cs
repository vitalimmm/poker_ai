
namespace PokerSimulatorEngine
{
    public class PokerEnums
    {
        public enum CardRank
        {
            One, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace
        }

        public enum CardSuit
        {
            Hearts, Diamonds, Spades, Clubs
        }

        public enum PlayerAction
        {
            Fold, Bet
        }

        public enum GameAction
        {
            PlayerPlayed, HandStarted, FlopStarted, TurnStarted, RiverStarted, HandEnded, PlayerLeft, PlayerJoined, PlayerWon, PlayerPartialWon
        }
    }
}
