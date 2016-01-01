using PokerSimulatorEngine.Interfaces;

namespace PokerSimulatorEngine
{
    public struct PokerEvent
    {
        public readonly PokerEnums.PokerAction action;
        public readonly IPokerPlayer player;
        public readonly Card card;
        public readonly double amount;
        public readonly double stack;

        public PokerEvent(PokerEnums.PokerAction action, IPokerPlayer player, Card card, double amount, double stack)
        {
            this.action = action;
            this.player = player;
            this.card = card;
            this.amount = amount;
            this.stack = stack;
        }

        public PokerEvent(PokerEnums.PokerAction action, Card card)
        {
            this.action = action;
            this.player = null;
            this.card = card;
            this.amount = -1;
            this.stack = -1;
        }

        public PokerEvent(PokerEnums.PokerAction action, IPokerPlayer player, double amount, double stack)
        {
            this.action = action;
            this.player = player;
            this.card = null;
            this.amount = amount;
            this.stack = stack;
        }

        public PokerEvent(PokerEnums.PokerAction action, IPokerPlayer player, double stack)
        {
            this.action = action;
            this.player = player;
            this.card = null;
            this.amount = -1;
            this.stack = stack;
        }
    }
}
