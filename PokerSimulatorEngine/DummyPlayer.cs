using PokerSimulatorEngine.Interfaces;
using System.Collections.ObjectModel;
using System;

namespace PokerSimulatorEngine
{
    public class DummyPlayer : IPokerPlayer
    {
        public PokerEnums.PlayerAction ComputeAction(Card[] hand, double currentStack, out double betAmount)
        {
            throw new NotImplementedException();
        }

        public void PokerGameActionHandler(PokerEventArgs pokerEvents)
        {
            throw new NotImplementedException();
        }
    }
}
