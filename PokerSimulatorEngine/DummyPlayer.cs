using PokerSimulatorEngine.Interfaces;
using System.Collections.ObjectModel;

namespace PokerSimulatorEngine
{
    public class DummyPlayer : IPokerPlayer
    {
        public PokerEnums.PokerAction ComputeAction(ReadOnlyCollection<PokerEvent> events, Card[] hand, double currentStack, out double raiseAmount)
        {
            raiseAmount = 0;
            return PokerEnums.PokerAction.Call;
        }
    }
}
