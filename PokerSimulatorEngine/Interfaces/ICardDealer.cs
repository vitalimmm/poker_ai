using System;
using System.Collections.Generic;

namespace PokerSimulatorEngine.Interfaces
{
    public interface ICardDealer
    {
        void InitCardDeack();
        void SortHands(List<IndexedHand> handsList,
            Card communityA, Card communityB, Card communityC, Card communityD, Card communityE);
        Card GetCard();
    }
}
