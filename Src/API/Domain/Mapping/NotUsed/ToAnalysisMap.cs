//using AutoMapper;
//using Models;
//using Rules;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace Mapping
//{
//    public class ToAnalysisMap : Profile
//    {
//        private static ThunderBallRules _rules;

//        public ToAnalysisMap()
//        {
//            _rules = new ThunderBallRules();

//            // Add as many of these lines as you need to map your objects
//            CreateMap<List<LotteryDto>, Models.ThunderBallAnalysis>()
//                .ForMember(dst => dst.BallTotalAvg, opt => opt.MapFrom(src => BallTotalAvg(src)))
//                .ForMember(dst => dst.BallAvgSpread, opt => opt.MapFrom(src => BallAvgSpread(src)))
//                .ForMember(dst => dst.NumBallDrawings, opt => opt.MapFrom(src => NumBallDrawings(src)))
//                .ForMember(dst => dst.OddBalls, opt => opt.MapFrom(src => OddBalls(src)))
//                .ForMember(dst => dst.RenatoGianellaOccurrance, opt => opt.MapFrom(src => RenatoGianellaOccurrance(src)))
//                .ForMember(dst => dst.Delta, opt => opt.MapFrom(src => Delta(src)));
//        }

//        private int BallTotalAvg(IList<LotteryDto> src)
//        {
//            var ballAvgTotal = 0;

//            foreach (var item in src)
//            {
//                ballAvgTotal += item.Balls[0] + item.Balls[1] + item.Balls[2] + item.Balls[3] + item.Balls[4];
//            }
//            if (src.Count == 0)
//                return 0;
//            return ballAvgTotal / src.Count;
//        }

//        private List<Ball> BallAvgSpread(IList<LotteryDto> src)
//        {
//            List<Ball> balls = new List<Ball>();

//            foreach (var item in src)
//            {
//                var avg = item.Balls[0] + item.Balls[1] + item.Balls[2] + item.Balls[3] + item.Balls[4];
//                var ball = balls.Find(x => x.BallNumber.Contains(avg.ToString()));
//                if (ball == null)
//                {
//                    ball = new Ball()
//                    {
//                        BallNumber = avg.ToString(),
//                        Count = 1,
//                    };
//                    balls.Add(ball);
//                }
//                else
//                {
//                    ball.Count += 1;
//                }
//            }

//            return balls;
//        }

//        private List<Ball> NumBallDrawings(IList<LotteryDto> src)
//        {
//            List<Ball> balls = new List<Ball>();

//            foreach (var item in src)
//            {
//                foreach (var ball in item.Balls)
//                    BallCount(balls, ball.ToString(), item.DrawNumber);
//            }

//            // Order the list
//            return balls.OrderBy(x => x.Count).ToList();
//        }
//        private static void BallCount(List<Ball> balls, string item, string drawNumber)
//        {
//            var ball = balls.Find(x => x.BallNumber.Contains(item));
//            if (ball == null)
//            {
//                ball = new Ball()
//                {
//                    BallNumber = item,
//                    Count = 1,
//                    Draws = new List<string>()
//                };
//                ball.Draws.Add(drawNumber);
//                balls.Add(ball);
//            }
//            else
//            {
//                ball.Count += 1;
//                ball.Draws.Add(drawNumber);
//            }
//        }

//        /// <summary>
//        /// Count of the number of odd/even balls across all draws
//        /// </summary>
//        /// <param name="src"></param>
//        /// <returns></returns>
//        private int[] OddBalls(List<LotteryDto> src)
//        {
//            var oddBalls = new int[6];

//            foreach (var item in src)
//            {
//                oddBalls[NumOddBalls(item.Balls)] += 1;
//            }

//            // Order the list
//            return oddBalls;
//        }

//        private int NumOddBalls(List<int> src)
//        {
//            var res1 = ((src[0] % 2) != 0) ? 1 : 0;
//            var res2 = ((src[1] % 2) != 0) ? 1 : 0;
//            var res3 = ((src[2] % 2) != 0) ? 1 : 0;
//            var res4 = ((src[3] % 2) != 0) ? 1 : 0;
//            var res5 = ((src[4] % 2) != 0) ? 1 : 0;

//            return res1 + res2 + res3 + res4 + res5;
//        }

//        private List<RenatoGianella> RenatoGianellaOccurrance(List<LotteryDto> src)
//        {
//            List<RenatoGianella> rg = new List<RenatoGianella>();

//            foreach (var item in src)
//            {
//                var rgp = RenatoGianellaPattern(item.Balls);

//                var name = Intarraytostring(rgp);
//                var rgi = rg.Find(x => x.Name.Contains(name));
//                if (rgi == null)
//                {
//                    rgi = new RenatoGianella()
//                    {
//                        Name = Intarraytostring(rgp),
//                        Count = 1,
//                        Pattern = rgp,
//                        Draws = new List<string>()
//                    };
//                    rgi.Draws.Add(item.DrawNumber);
//                    rg.Add(rgi);
//                }
//                else
//                {
//                    rgi.Count += 1;
//                    rgi.Draws.Add(item.DrawNumber);
//                }
//            }

//            // Order the list
//            return rg.OrderBy(x => x.Count).ToList();
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

//        /// <summary>
//        /// Brazillian Mathamatician
//        ///  If you write out the lottery numbers into rows
//        ///  and assign each row a colour. 
//        ///  Then you can see which patterns occur the most.
//        ///  
//        /// 1-9   = R
//        /// 10-19 = G
//        /// 20-29 = B
//        /// 30-32 = Y
//        /// </summary>
//        /// <param name="src"></param>
//        /// <returns></returns>
//        private int[] RenatoGianellaPattern(List<int> src)
//        {
//            var rgby = new int[_rules.NoOfMainBalls];
//            rgby[BallRGColour(src[0])] += 1;
//            rgby[BallRGColour(src[1])] += 1;
//            rgby[BallRGColour(src[2])] += 1;
//            rgby[BallRGColour(src[3])] += 1;
//            rgby[BallRGColour(src[4])] += 1;
//            return rgby;
//        }

//        private static int BallRGColour(int ball)
//        {
//            var div = (int)Math.Ceiling((double)_rules.NoOfBalls / _rules.NoOfMainBalls);

//            for (int i = 0; i <= _rules.NoOfMainBalls - 1; i++)
//            {
//                if (ball > (div * i) && ball < (div * (i + 1)) + 1)
//                    return i;
//            }
//            return -1;
//        }

//        private List<int[]> Delta(List<LotteryDto> src)
//        {
//            var lst = new List<int[]>();

//            foreach (var item in src)
//            {
//                lst.Add(Delta(item.Balls));
//            }
//            return lst;
//        }

//        private int[] Delta(List<int> src)
//        {
//            var balls = new int[_rules.NoOfMainBalls];
//            balls[0] = src[0];
//            balls[1] = src[1] - src[0];
//            balls[2] = src[2] - src[1];
//            balls[3] = src[3] - src[2];
//            balls[4] = src[4] - src[3];

//            return balls;
//        }

//    }
//}
