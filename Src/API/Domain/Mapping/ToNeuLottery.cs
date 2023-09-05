using AutoMapper;
using Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace Domain.Mapping
{
    public class ToNeuLottery : Profile
    {
        private Models.Rules _rules;

        public ToNeuLottery()
        {
            _rules = Rules();

            CreateMap<List<NeuLotteryEntity>, Models.NeuLottery>()
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src[0].PartitionKey))
                .ForMember(dst => dst.Rules, opt => opt.MapFrom(src => _rules))
                .ForMember(dst => dst.Draws, opt => opt.MapFrom(src => src));

            //CreateMap<NeuLotteryEntity, Models.Draws>()
            //    .ForMember(dst => dst.Draw, opt => opt.MapFrom(src => Draws(src)));


            //CreateMap<NeuLotteryEntity, Models.Draws>()
            //    .ForMember(dst => dst.DrawDate, opt => opt.MapFrom(src => src.DrawDate))
            //    .ForMember(dst => dst.DrawNumber, opt => opt.MapFrom(src => src.DrawNumber))
            //    .ForMember(dst => dst.Draw, opt => opt.MapFrom(src => src));

            //CreateMap<NeuLotteryEntity, Models.Draw>()
            //    .ForMember(dst => dst.Balls, opt => opt.MapFrom(src => Balls(src)))
            //    .ForMember(dst => dst.BonusBalls, opt => opt.MapFrom(src => BonusBall(src)))
            //    .ForMember(dst => dst.Machine, opt => opt.MapFrom(src => src.Machine))
            //    .ForMember(dst => dst.BallSet, opt => opt.MapFrom(src => src.BallSet))
            //    .ForMember(dst => dst.Analysis, opt => opt.MapFrom(src => Analysis(src)));


            //CreateMap<List<ThunderBallEntity>, Models.Guesses>()
            //    .ForMember(dst => dst.DrawDate, opt => opt.MapFrom(src => src[0].DrawDate))
            //    .ForMember(dst => dst.DrawNumber, opt => opt.MapFrom(src => src[0].DrawNumber))
            //    .ForMember(dst => dst.Guess, opt => opt.MapFrom(src => src));

            //CreateMap<ThunderBallEntity, Models.Guess>()
            //    .ForMember(dst => dst.Balls, opt => opt.MapFrom(src => Balls(src)))
            //    .ForMember(dst => dst.BonusBalls, opt => opt.MapFrom(src => BonusBall(src)))
            //    .ForMember(dst => dst.Analysis, opt => opt.MapFrom(src => Analysis(src)));
            ////.ForMember(dst => dst.NoOfBallMatches, opt => opt.MapFrom(src => NoOfBallMatches(src)))
            ////.ForMember(dst => dst.NoOfBonusBallMatches, opt => opt.MapFrom(src => NoOfBonusBallMatches(src)))
            ////.ForMember(dst => dst.Win, opt => opt.MapFrom(src => Winings(src)));

            //CreateMap<Models.Guesses, Models.NeuLottery>()
            //    .ForMember(dst => dst.Draws, opt => opt.MapFrom(src => src));
        }

        //private Models.Draws Draws(NeuLotteryEntity src)
        //{
        //    var draw = new Draws();
        //    var entity = JsonConvert.DeserializeObject<DrawEntity>(src.Draw);

        //    draw.DrawDate = entity.DrawDate;
        //    draw.DrawNumber = entity.DrawNumber;
        //    draw.Draw = new Draw();
        //    draw.Draw.Machine = entity.Machine;
        //    draw.Draw.BonusBalls = entity.Balls.BonusBalls.ToList();
        //    draw.Draw.Balls = entity.Balls.MainBalls.ToList();

        //    return draw;
        //}

        private Models.Rules Rules()
        {
            Models.Rules rules = new Models.Rules();

            rules.MaxMainBall = 39;
            rules.MaxBonusBall = 14;
            rules.NoOfBallsToSelect = 5;
            rules.NoOfBonusBallsToSelect = 1;
            rules.NoOfGuesses = 1;
            rules.CostPerGuess = 1;
            rules.Win = new List<Win> 
            {
                new Win() { MainBalls = 0, BonusBalls = 1, Pays = 1},
                new Win() { MainBalls = 1, BonusBalls = 1, Pays = 5},
                new Win() { MainBalls = 2, BonusBalls = 1, Pays = 10},
                new Win() { MainBalls = 3, BonusBalls = 0, Pays = 10},
                new Win() { MainBalls = 3, BonusBalls = 1, Pays = 20},
                new Win() { MainBalls = 4, BonusBalls = 0, Pays = 100},
                new Win() { MainBalls = 4, BonusBalls = 1, Pays = 250},
                new Win() { MainBalls = 5, BonusBalls = 0, Pays = 5000},
                new Win() { MainBalls = 5, BonusBalls = 1, Pays = 50000},
            };

            return rules;
        }

        private Models.Analysis Analysis(ThunderBallEntity src)
        {
            var analysis = new Models.Analysis();
            analysis.Delta = Delta(src);
            analysis.RGPattern = RenatoGianellaPattern(src);
            analysis.NoOfOddBalls = NumOddBalls(src);
            analysis.MainBallTotal = BallTotal(src);

            return analysis;
        }
        private int BallTotal(ThunderBallEntity src)
        {
            return int.Parse(src.Ball1) + int.Parse(src.Ball2) + int.Parse(src.Ball3) + int.Parse(src.Ball4) + int.Parse(src.Ball5);
        }
        private List<int> Balls(ThunderBallEntity src)
        {
            var balls = new List<int>
            {
                int.Parse(src.Ball1),
                int.Parse(src.Ball2),
                int.Parse(src.Ball3),
                int.Parse(src.Ball4),
                int.Parse(src.Ball5)
            };

            balls.Sort();
            return balls;
        }
        private List<int> BonusBall(ThunderBallEntity src)
        {
            var balls = new List<int>
            {
                int.Parse(src.Thunderball)
            };
            return balls;
        }
        private int NumOddBalls(ThunderBallEntity src)
        {
            var res1 = int.Parse(src.Ball1) % 2 != 0 ? 1 : 0;
            var res2 = int.Parse(src.Ball2) % 2 != 0 ? 1 : 0;
            var res3 = int.Parse(src.Ball3) % 2 != 0 ? 1 : 0;
            var res4 = int.Parse(src.Ball4) % 2 != 0 ? 1 : 0;
            var res5 = int.Parse(src.Ball5) % 2 != 0 ? 1 : 0;

            return res1 + res2 + res3 + res4 + res5;
        }
        private List<int> Delta(ThunderBallEntity src)
        {
            var balls = new List<int>
            {
                int.Parse(src.Ball1),
                int.Parse(src.Ball2) - int.Parse(src.Ball1),
                int.Parse(src.Ball3) - int.Parse(src.Ball2),
                int.Parse(src.Ball4) - int.Parse(src.Ball3),
                int.Parse(src.Ball5) - int.Parse(src.Ball4)
            };
            return balls;
        }

        /// <summary>
        /// Brazillian Mathamatician
        ///  If you write out the lottery numbers into rows
        ///  and assign each row a colour. 
        ///  Then you can see which patterns occur the most.
        ///  
        /// 1-9   = R
        /// 10-19 = G
        /// 20-29 = B
        /// 30-32 = Y
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        private List<int> RenatoGianellaPattern(ThunderBallEntity src)
        {
            List<int> balls = Balls(src);

            var div = (int)Math.Ceiling((double)_rules.MaxMainBall / _rules.NoOfBallsToSelect);

            var rgby = new List<int>
            {
                BallRGColour(balls[0], div),
                BallRGColour(balls[1], div),
                BallRGColour(balls[2], div),
                BallRGColour(balls[3], div),
                BallRGColour(balls[4], div)
            };
            return rgby;
        }

        private int TBallRenatoGianellaPattern(ThunderBallEntity src)
        {
            var div = (int)Math.Ceiling((double)_rules.MaxBonusBall / _rules.NoOfBallsToSelect);
            var ball = int.Parse(src.Thunderball);

            return BallRGColour(ball, div);
        }

        private int BallRGColour(int ball, int div)
        {
            for (int i = 0; i <= _rules.NoOfBallsToSelect - 1; i++)
            {
                var gt = ball > (div * i);
                var lt = ball < (div * (i + 1)) + 1;

                if (ball > (div * i) && ball < (div * (i + 1)) + 1)
                    return i;
            }
            return -1;
        }
    }

}

