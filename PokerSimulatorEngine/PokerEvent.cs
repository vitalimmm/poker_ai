using PokerSimulatorEngine.Interfaces;

namespace PokerSimulatorEngine
{
    public delegate void PokerEventHandler(PokerEventArgs eventArgs);

    public struct PokerEventArgs
    {
        public readonly PokerEnums.GameAction gameAction;
        public readonly PokerEnums.PlayerAction playerAction;
        public readonly IPokerPlayer player;
        public readonly Card cardA, cardB, cardC;
        public readonly double amount;
        public readonly double stack;

        public PokerEventArgs(PokerEnums.GameAction gameAction, PokerEnums.PlayerAction playerAction, IPokerPlayer player, double amount, double stack)
        {
            this.gameAction = gameAction;
            this.playerAction = playerAction;
            this.player = player;
            this.amount = amount;
            this.stack = stack;

            cardA = cardB = cardC = Card.Empty;
        }

        public PokerEventArgs(PokerEnums.GameAction gameAction, Card card, double amount)
        {
            this.cardA = card;
            this.gameAction = gameAction;
            this.amount = amount;

            this.playerAction = 0;
            this.player = null;
            this.stack = -1;
            cardB = cardC = Card.Empty;
        }

        public PokerEventArgs(PokerEnums.GameAction gameAction, Card cardA, Card cardB, Card cardC, double amount)
        {
            this.cardA = cardA;
            this.cardB = cardB;
            this.cardC = cardC;
            this.gameAction = gameAction;
            this.amount = amount;

            this.playerAction = 0;
            this.player = null;
            this.stack = -1;
        }
    }
}
