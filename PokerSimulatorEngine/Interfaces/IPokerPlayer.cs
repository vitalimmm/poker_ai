
namespace PokerSimulatorEngine.Interfaces
{
    public interface IPokerPlayer
    {
        void PokerGameActionHandler(PokerEventArgs pokerEvents);
        PokerEnums.PlayerAction ComputeAction(Card[] hand, double currentStack, out double betAmount);
    }
}
