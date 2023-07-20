namespace Domain.Models
{
    public class Stats
    {
        public int Win { get; set; } = 0;
        public int Guesses { get; set; } = 0;

        public int NoOfDraws { get; set; } = 0;
        public int WinlessDraws { get; set; } = 0;
        public int NoOfDrawsWhereReturnStake { get; set; } = 0;
        public int NoOfDrawsWhereReturnMoreThanStake { get; set; } = 0;
        public int NoOfDrawsWhereReturnLessThanStake { get; set; } = 0;

        public int matchesZero { get; set; } = 0;
        public int matchesOne { get; set; } = 0;
        public int matchesOneAndTball { get; set; } = 0;
        public int matchesTwo { get; set; } = 0;
        public int matchesThree { get; set; } = 0;
        public int matchesThreeAndTball { get; set; } = 0;
        public int matchesFour { get; set; } = 0;
        public int matchesFourAndTball { get; set; } = 0;
        public int matchesFive { get; set; } = 0;
        public int matchesFiveAndTball { get; set; } = 0;

    }
}
