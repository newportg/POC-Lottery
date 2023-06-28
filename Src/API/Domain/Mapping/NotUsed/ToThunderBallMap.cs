//using AutoMapper;
//using Common.Mappings;
//using Models;
//using Rules;
//using System.Collections.Generic;
//using System.Linq;

//namespace FMLottery.Mapping
//{
//    public class ToThunderBallMap : Profile
//    {
//        public ToThunderBallMap() 
//        {
//            // Add as many of these lines as you need to map your objects
//            CreateMap<List<ThunderBallDto>, Models.ThunderBall>()
//                .ForMember(dst => dst.Draws, opt => opt.MapFrom(src => src))
//                .ForMember(dst => dst.BallTotalAvg, opt => opt.MapFrom(src => BallTotalAvg(src)))
//                .ForMember(dst => dst.NumBallDrawings, opt => opt.MapFrom(src => NumBallDrawings(src)))
//                .ForMember(dst => dst.OddBalls, opt => opt.MapFrom(src => OddBalls(src)))
//                .ForMember(dst => dst.RenatoGianellaOccurrance, opt => opt.MapFrom(src => RenatoGianellaOccurrance(src)));
//        }
//        private int BallTotalAvg(IList<ThunderBallDto> src)
//        {
//            int ballAvgTotal = 0;

//            foreach (var item in src)
//            {
//                ballAvgTotal += item.BallTotal;
//            }
//            if (src.Count == 0)
//                return 0;
//            return ballAvgTotal / src.Count;
//        }
//        private List<Ball> NumBallDrawings(IList<ThunderBallDto> src)
//        {
//            List<Ball> balls = new List<Ball>();

//            foreach (var item in src)
//            {
//                foreach( var ball in item.Balls)
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
//        private int[] OddBalls(List<ThunderBallDto> src)
//        {
//            var oddBalls = new int[6];

//            foreach (var item in src)
//            {
//                oddBalls[item.NumOddBalls] += 1;
//            }

//            // Order the list
//            return oddBalls;
//        }
//        private List<RenatoGianella> RenatoGianellaOccurrance(List<ThunderBallDto> src)
//        {
//            List<RenatoGianella> rg = new List<RenatoGianella>();

//            foreach (var item in src)
//            {
//                var name = Intarraytostring(item.RenatoGianellaPattern);
//                var rgi = rg.Find(x => x.Name.Contains(name));
//                if (rgi == null)
//                {
//                    rgi = new RenatoGianella()
//                    {
//                        Name = Intarraytostring(item.RenatoGianellaPattern),
//                        Count = 1,
//                        Pattern = item.RenatoGianellaPattern,
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

//        //public void Mapping(Profile profile)
//        //{
//        //    profile.CreateMap<List<ThunderBallDto>, Models.ThunderBall>()
//        //        .ForMember(dst => dst.Draws, opt => opt.MapFrom(src => src));
//        //    //.ForMember(dst => dst.BallTotalAvg, opt => opt.MapFrom(src => BallTotalAvg(src)))
//        //    //.ForMember(dst => dst.NumBallDrawings, opt => opt.MapFrom(src => NumBallDrawings(src)))
//        //    //.ForMember(dst => dst.OddBalls, opt => opt.MapFrom(src => OddBalls(src)))
//        //    //.ForMember(dst => dst.RenatoGianellaOccurrance, opt => opt.MapFrom(src => RenatoGianellaOccurrance(src)));
//        //}
//    }
//}
