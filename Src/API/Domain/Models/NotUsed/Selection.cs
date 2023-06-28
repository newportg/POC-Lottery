//using Rules;
//using System;
//using System.Collections.Generic;

//namespace Models
//{
//    public class Selection
//    {
//        private static ThunderBallRules _rules;
//        public Selection(ThunderBallRules rules, RenatoGianella pattern)
//        {
//            _rules = rules;
//            RandomDraw(pattern);
//        }
//        public string Index()
//        {
//            string str = "";

//            foreach (int i in MainBalls)
//            {
//                str += i.ToString();
//            }

//            return str + BonusBall.ToString();
//        }

//        public List<int> MainBalls = new();
//        public int BonusBall { get; set; }

//        private void RandomDraw(RenatoGianella pattern)
//        {
//            if (pattern.Count == 0)
//                return;

//            var rgpattern = pattern;
//            var div = (int)Math.Ceiling((double)_rules.NoOfBalls / (_rules.NoOfMainBalls - 1));

//            Random random = new();

//            for (var i = 0; i < rgpattern.Pattern.Length; i++)
//            {
//                for (var j = 0; j < rgpattern.Pattern[i]; j++)
//                {
//                    int ball;
//                    do
//                    {
//                        ball = random.Next(1 + (div * i), (div * (i + 1)));

//                    } while (MainBalls.Contains(ball));
//                    MainBalls.Add(ball);
//                }
//            }

//            BonusBall = random.Next(1, _rules.ThunderBallMax);
//        }
//    }
//}
