//using FluentValidation;
//using Models;
//using Rules;
//using System;
//using FluentValidation.Results;

//namespace Validation
//{
//    public class ThunderBallSelectionValidator : AbstractValidator<Selection>
//    {
//        private static ThunderBallRules _rules;
//        private readonly Models.ThunderBall _thunderBall;

//        public ThunderBallSelectionValidator(ThunderBallRules rules, Models.ThunderBall src)
//        {
//            _rules = rules;
//            _thunderBall = src;

//            // Apply Rules
//            // Get the highest ranked RG Pattern
//            RuleFor(x => x).Must(HighestRankedRgPattern);
//            // Get the OddEven Ball 
//            // Get BallTotalAverage
//            // Selection has not already been drawn
//            // Selection is not consecutive
//            RuleFor(x => x).Must(SelectionNotConsecutive);
//        }

//        private bool SelectionNotConsecutive(Selection arg)
//        {
//            arg.MainBalls.Sort();

//            for (var i = 0; i < arg.MainBalls.Count - 1; i++)
//            {
//                if (arg.MainBalls[i] + 1 != arg.MainBalls[i + 1])
//                    return true;
//            }

//            return false;
//        }

//        private bool HighestRankedRgPattern(Selection arg)
//        {
//            var rgpattern = _thunderBall.RenatoGianellaOccurrance[_thunderBall.RenatoGianellaOccurrance.Count - 1];
//            var rgby = new int[4];

//            foreach (var i in arg.MainBalls)
//            {
//                rgby[BallRgColour(i)] += 1;
//            }

//            if (Intarraytostring(rgby).Equals(rgpattern.Name))
//                return true;
//            return false;
//        }

//        private static int BallRgColour(int ball)
//        {
//            var b = ball;
//            var div = (int)Math.Ceiling((double)_rules.NoOfBalls / (_rules.NoOfMainBalls - 1));

//            for (int i = 0; i < _rules.NoOfMainBalls - 1; i++)
//            {
//                if (b > (div * i) && b < (div * (i + 1)) + 1)
//                    return i;
//            }
//            return -1;
//        }

//        private static string Intarraytostring(int[] input)
//        {
//            string str = "";

//            for (int i = 0; i < input.Length; i++)
//            {
//                str += input[i].ToString();
//            }
//            return str;
//        }
//    }
//}
