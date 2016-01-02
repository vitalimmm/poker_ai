using PokerSimulatorEngine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PokerSimulatorEngine
{
    public class PokerGame : IPokerGameEvents
    {
        public static readonly int MAX_PLAYERS = 10, MIN_PLAYERS = 2;

        private List<PlayerInfo> _playersInfo;
        private ICardDealer _cardDealer;
        private int _currentDealerIndex, _currentPlayingHandCount;
        private Card[] _communityCards;
        private List<IndexedHand> _currentHandsList;

        public event PokerEventHandler PokerActionPreformed;
        public double BigBlind { get; set; }
        public double SmallBlind { get; set; }

        public PokerGame(ICardDealer cardDealer, List<Tuple<IPokerPlayer, double>> players, double bigBlind, double smallBlind)
        {
            if (players.Count > 10)
                throw new Exception("too many players (got: " + players.Count + ", max: " + MAX_PLAYERS + ")");

            _playersInfo = new List<PlayerInfo>(MAX_PLAYERS);
            foreach (var player in players)
                AddPlayer(player.Item1, player.Item2);

            _cardDealer = cardDealer;
            _currentDealerIndex = 0;
            _currentHandsList = new List<IndexedHand>(MAX_PLAYERS);

            BigBlind = bigBlind;
            SmallBlind = smallBlind;
        }

        public void AddPlayer(IPokerPlayer player, double stackSize)
        {
            var playerInfo = new PlayerInfo();
            playerInfo.Player = player;
            playerInfo.Stack = stackSize;
            _playersInfo.Add(playerInfo);

            PokerActionPreformed += player.PokerGameActionHandler;

            FireActionEvent(new PokerEventArgs(PokerEnums.GameAction.PlayerJoined, 0, player, -1, stackSize));
        }

        public void PlaySingleHand()
        {
            if(_playersInfo.Count < MIN_PLAYERS)
                throw new Exception("not enougth players (currently: " + _playersInfo.Count + ", min: " + MIN_PLAYERS + ")");

            var totalBetSize = BigBlind;

            InitSingleHand();

            // pre-flop
            totalBetSize = StartBettingCycle((_currentDealerIndex + 3) % _playersInfo.Count, totalBetSize);

            if (_currentPlayingHandCount == 1)
            {
                EndCurrentHand();
                return;
            }

            // flop
            _communityCards[0] = _cardDealer.GetCard();
            _communityCards[1] = _cardDealer.GetCard();
            _communityCards[2] = _cardDealer.GetCard();

            FireActionEvent(new PokerEventArgs(PokerEnums.GameAction.FlopStarted, _communityCards[0], _communityCards[1], _communityCards[2], _currentPlayingHandCount));
            totalBetSize = StartBettingCycle((_currentDealerIndex + 1) % _playersInfo.Count, totalBetSize);

            if (_currentPlayingHandCount == 1)
            {
                EndCurrentHand();
                return;
            }

            // turn
            _communityCards[3] = _cardDealer.GetCard();

            FireActionEvent(new PokerEventArgs(PokerEnums.GameAction.TurnStarted, _communityCards[3], _currentPlayingHandCount));
            totalBetSize = StartBettingCycle((_currentDealerIndex + 1) % _playersInfo.Count, totalBetSize);

            if (_currentPlayingHandCount == 1)
            {
                EndCurrentHand();
                return;
            }

            // river
            _communityCards[3] = _cardDealer.GetCard();

            FireActionEvent(new PokerEventArgs(PokerEnums.GameAction.RiverStarted, _communityCards[3], _currentPlayingHandCount));
            StartBettingCycle((_currentDealerIndex + 1) % _playersInfo.Count, totalBetSize);

            EndCurrentHand();
        }

        private void InitSingleHand()
        {
            FireActionEvent(new PokerEventArgs(PokerEnums.GameAction.HandStarted, 0, null, _playersInfo.Count, -1));

            _currentDealerIndex++;
            _cardDealer.InitCardDeack();
            _currentPlayingHandCount = _playersInfo.Count;

            foreach (var playerInfo in _playersInfo)
            {
                playerInfo.BetSize = 0;
                playerInfo.IsFold = false;
                playerInfo.Cards[0] = _cardDealer.GetCard();
                playerInfo.Cards[1] = _cardDealer.GetCard();
            }

            var smallPlayerInfo = _playersInfo[(_currentDealerIndex + 1) % _playersInfo.Count];
            var bigPlayerInfo = _playersInfo[(_currentDealerIndex + 2) % _playersInfo.Count];

            smallPlayerInfo.BetSize = Math.Min(smallPlayerInfo.Stack, SmallBlind);
            smallPlayerInfo.Stack -= smallPlayerInfo.BetSize;
            FireActionEvent(new PokerEventArgs(PokerEnums.GameAction.PlayerPlayed, PokerEnums.PlayerAction.Bet, smallPlayerInfo.Player, smallPlayerInfo.BetSize, smallPlayerInfo.Stack));

            bigPlayerInfo.BetSize = Math.Min(bigPlayerInfo.Stack, BigBlind);
            bigPlayerInfo.Stack -= bigPlayerInfo.BetSize;
            FireActionEvent(new PokerEventArgs(PokerEnums.GameAction.PlayerPlayed, PokerEnums.PlayerAction.Bet, smallPlayerInfo.Player, bigPlayerInfo.BetSize, bigPlayerInfo.Stack));
        }

        private double StartBettingCycle(int firstPlayerIndex, double currentTotalBet)
        {
            var minRaise = BigBlind;

            foreach (var playerInfo in _playersInfo)
                playerInfo.IsActed = false;


            var currPlayerIndex = firstPlayerIndex;
            while (!(_playersInfo[currPlayerIndex].IsActed && !_playersInfo[currPlayerIndex].IsFold && _playersInfo[currPlayerIndex].BetSize == currentTotalBet) &&
                (_currentPlayingHandCount > 1 || _playersInfo[currPlayerIndex].BetSize < currentTotalBet))
            {
                _playersInfo[currPlayerIndex].IsActed = true;

                if(!(_playersInfo[currPlayerIndex].IsFold || _playersInfo[currPlayerIndex].Stack == 0))
                {
                    double currPlayerBet;
                    var currPlayerAction = _playersInfo[currPlayerIndex].Player.ComputeAction(_playersInfo[currPlayerIndex].Cards, _playersInfo[currPlayerIndex].Stack, out currPlayerBet);

                    if (currPlayerAction == PokerEnums.PlayerAction.Bet)
                    {
                        if (ValidatePlayerBet(_playersInfo[currPlayerIndex], currPlayerBet, currentTotalBet, minRaise))
                        {
                            BetPlayer(_playersInfo[currPlayerIndex], currPlayerBet);

                            if (_playersInfo[currPlayerIndex].BetSize > currentTotalBet)
                                minRaise = _playersInfo[currPlayerIndex].BetSize - currentTotalBet;

                            currentTotalBet = Math.Max(_playersInfo[currPlayerIndex].BetSize, currentTotalBet);
                        }
                        else
                            FoldPlayer(_playersInfo[currPlayerIndex]);
                    }
                    else if(currPlayerAction == PokerEnums.PlayerAction.Fold)
                        FoldPlayer(_playersInfo[currPlayerIndex]);
                }

                currPlayerIndex = (currPlayerIndex + 1) % _playersInfo.Count;
            }

            return currentTotalBet;
        }

        private bool ValidatePlayerBet(PlayerInfo player, double playerBetAmount, double totalBetAmount, double minRaise)
        {
            var betAmountNeeded = totalBetAmount - player.BetSize;

            if (playerBetAmount > player.Stack)
                return false;

            if (playerBetAmount == betAmountNeeded || playerBetAmount == player.Stack)
                return true;

            if (playerBetAmount >= betAmountNeeded + minRaise)
                return true;

            return false;
        }

        private void FoldPlayer(PlayerInfo playerInfo)
        {
            if (!playerInfo.IsFold)
            {
                playerInfo.IsFold = true;
                _currentPlayingHandCount--;
                FireActionEvent(new PokerEventArgs(PokerEnums.GameAction.PlayerPlayed, PokerEnums.PlayerAction.Fold, playerInfo.Player, -1, playerInfo.Stack));
            }
        }

        private void BetPlayer(PlayerInfo playerInfo, double amount)
        {
            var actualBetAmount = Math.Min(amount, playerInfo.Stack);

            FireActionEvent(new PokerEventArgs(PokerEnums.GameAction.PlayerPlayed, PokerEnums.PlayerAction.Bet, playerInfo.Player, actualBetAmount, playerInfo.Stack));

            playerInfo.BetSize += actualBetAmount;
            playerInfo.Stack -= actualBetAmount;

            if(playerInfo.Stack == 0)
                _currentPlayingHandCount--;
        }

        private void EndCurrentHand()
        {
            _currentHandsList.Clear();

            for(int i = 0; i < _playersInfo.Count; i++)
            {
                if (!_playersInfo[i].IsFold)
                {
                    _playersInfo[i].IndexedHand.CardA = _playersInfo[i].Cards[0];
                    _playersInfo[i].IndexedHand.CardB = _playersInfo[i].Cards[1];
                    _playersInfo[i].IndexedHand.PlayerIndex = i;

                    _currentHandsList.Add(_playersInfo[i].IndexedHand);
                }
            }

            _cardDealer.SortHands(_currentHandsList,
                _communityCards[0], _communityCards[1], _communityCards[2], _communityCards[3], _communityCards[4]);

            var winningHandIndex = 0;
            var hasAnotherWinner = true;
            double totalWinnings;
            var winningEnumAction = PokerEnums.GameAction.PlayerWon;

            while (hasAnotherWinner)
            {
                hasAnotherWinner = false;
                totalWinnings = 0;

                var winningPlayerIndex = _currentHandsList[winningHandIndex].PlayerIndex;
                var winningSize = _playersInfo[winningPlayerIndex].BetSize;

                foreach(var playerInfo in _playersInfo)
                {
                    if(playerInfo.BetSize <= winningSize)
                    {
                        totalWinnings += playerInfo.BetSize;
                        _playersInfo[winningPlayerIndex].Stack += playerInfo.BetSize;
                        playerInfo.BetSize = 0;
                    }
                    else
                    {
                        hasAnotherWinner = true;
                        playerInfo.BetSize += winningSize;
                        _playersInfo[winningPlayerIndex].Stack += winningSize;
                        playerInfo.BetSize -= winningSize;
                    }
                }

                FireActionEvent(new PokerEventArgs(winningEnumAction, 0, _playersInfo[winningPlayerIndex].Player, totalWinnings, _playersInfo[winningPlayerIndex].Stack));

                if (hasAnotherWinner)
                {
                    winningHandIndex++;
                    winningEnumAction = PokerEnums.GameAction.PlayerPartialWon;
                }
            }

            _playersInfo.RemoveAll(
                playerInfo =>
                {
                    if (playerInfo.Stack == 0)
                        FireActionEvent(new PokerEventArgs(PokerEnums.GameAction.PlayerLeft, 0, playerInfo.Player, -1, 0));
                    return playerInfo.Stack == 0;
                });
        }

        private void FireActionEvent(PokerEventArgs pokerEventArgs)
        {
            PokerActionPreformed(pokerEventArgs);
        }

        private class PlayerInfo
        {
            public IPokerPlayer Player;
            public double Stack;
            public double BetSize = 0;
            public bool IsFold = false, IsActed = false;
            public Card[] Cards = new Card[2];
            public IndexedHand IndexedHand = new IndexedHand();
        }
    }
}
