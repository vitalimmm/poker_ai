using System.Collections.ObjectModel;

namespace PokerSimulatorEngine.Interfaces
{
    public interface IPokerPlayer
    {
        PokerEnums.PokerAction ComputeAction(ReadOnlyCollection<PokerEvent> events, Card[] hand, double currentStack, out double raiseAmount);
    }
}
