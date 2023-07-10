using System.Collections.Generic;

namespace Domain.Rules
{
    public class ThunderBallRules
    {
        public int NoOfBalls { get; } = 39;
        public int NoOfMainBalls { get; } = 5;
        public int NoOfBonusBalls { get; } = 1;
        public int CostPerGuess { get; } = 1;
        public int MinMainBallWin { get; } = 10;
        public int NoOfGuesses()
        {
            return (MinMainBallWin / CostPerGuess) - CostPerGuess;
        }
        public int ThunderBallMax { get; } = 14;

    }
}
