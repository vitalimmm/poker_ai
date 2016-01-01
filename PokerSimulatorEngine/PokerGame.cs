using PokerSimulatorEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerSimulatorEngine
{
    public class PokerGame
    {
        private List<PokerPlayer> _players;
        private List<PokerEvent> _events;
        private int _currentDealerIndex;
        private double _pot, _bigBlind, _smallBlind;


        public void PlaySingleHand()
        {
            _pot = 0;
            _events.Add(new PokerEvent(
                PokerEnums.PokerAction.HandStarted,
                _players[_currentDealerIndex].player,
                _players.Count, -1));

            int smallIndex = (_currentDealerIndex + 1) % _players.Count,
                bigIndex = (_currentDealerIndex + 2) % _players.Count;

            _players[smallIndex].stack -= _smallBlind;
            _events.Add(new PokerEvent(
                PokerEnums.PokerAction.Raise, 
                _players[smallIndex].player,
                _smallBlind,
                 _players[smallIndex].stack));

            _players[bigIndex].stack -= _bigBlind;
            _events.Add(new PokerEvent(
                PokerEnums.PokerAction.Raise,
                _players[bigIndex].player,
                _bigBlind,
                 _players[bigIndex].stack));

            PlayPreFlop();
        }

        private void PlayPreFlop()
        {

        }

        private class PokerPlayer
        {
            public IPokerPlayer player;
            public double stack;
            public Card cardA, cardB;
        }
    }
}
