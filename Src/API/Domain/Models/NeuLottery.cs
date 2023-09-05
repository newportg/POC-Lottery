using System.Collections.Generic;

namespace Domain.Models
{
    public class NeuLottery
    {
        public string Name { get; set; }
        public Rules Rules { get; set; }
        public List<string> Draws { get; set; }
    }
    public class Rules
    {
        public int MaxMainBall { get; set; }
        public int MaxBonusBall { get; set; }
        public int NoOfBallsToSelect { get; set; }
        public int NoOfBonusBallsToSelect { get; set; }
        public List<Win> Win { get; set; }
        public int NoOfGuesses { get; set; }
        public int CostPerGuess { get; set; }
    }
    public class Draws
    {
        public string DrawNumber { get; set; }
        public string DrawDate { get; set; }
        public Draw Draw { get; set; }
        public List<Guess> Guesses { get; set; }
    }
    public class Draw
    {
        public List<int> Balls { get; set; }
        public List<int> BonusBalls { get; set; }
        public string Machine { get; set; }
        public string BallSet { get; set; }
        public Analysis Analysis { get; set; }
    }

    public class Guesses
    {
        public string DrawNumber { get; set; }
        public string DrawDate { get; set; }
        public List<Guess> Guess { get; set; }
    }
    public class Guess
    {
        public List<Ball> MainBalls { get; set; }
        public List<BonusBall> BonusBalls { get; set; }
        public Analysis Analysis { get; set; }

        public int NoOfBallMatches { get; set; }
        public int NoOfBonusBallMatches { get; set; }
        public int Win { get; set; }
    }
    public class Analysis
    {
        public List<int> RGPattern { get; set; }
        public List<int> Delta { get; set; }
        public int NoOfOddBalls { get; set; }
        public int MainBallTotal { get; set; }
    }
    public class Ball
    {
        public int Number { get; set; }
        public bool Match { get; set; }
    }
    public class BonusBall
    {
        public int Number { get; set; }
        public bool Match { get; set; }
    }
    public class Win
    {
        public int MainBalls { get; set; }
        public int BonusBalls { get; set; }
        public int Pays { get; set; }
    }
}
