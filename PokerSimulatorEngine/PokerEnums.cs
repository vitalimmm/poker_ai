using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public enum PokerAction
        {
            Fold, Call, Check, Raise, HandStarted, FlopStarted, TurnStarted, RiverStarted, WinnerDeclared, PlayerLeft, PlayerJoined
        }
    }
}
