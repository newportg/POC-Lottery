using AutoMapper;
using Models;
using Rules;
using ThunderBall.Models;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ThunderBall.Mapping
{
    public class ToThunderBallDtoMap : Profile
    {
        private static ThunderBallRules _rules;

        public ToThunderBallDtoMap()
        {
            _rules = new ThunderBallRules();

            CreateMap<ThunderBallEntity, ThunderBallDto>()
                .ForMember(dst => dst.Balls, opt => opt.MapFrom(src => Balls(src)))
                .ForMember(dst => dst.BonusBalls, opt => opt.MapFrom(src => BonusBall(src)))
                .ForMember(dst => dst.BallSet, opt => opt.MapFrom(src => src.BallSet))
                .ForMember(dst => dst.DrawDate, opt => opt.MapFrom(src => DrawDate(src)))
                .ForMember(dst => dst.DrawNumber, opt => opt.MapFrom(src => src.DrawNumber))
                .ForMember(dst => dst.Machine, opt => opt.MapFrom(src => src.Machine))
                .ForMember(dst => dst.BallTotal, opt => opt.MapFrom(src => BallTotal(src)))
                .ForMember(dst => dst.NumOddBalls, opt => opt.MapFrom(src => NumOddBalls(src)))
                .ForMember(dst => dst.RenatoGianellaPattern, opt => opt.MapFrom(src => RenatoGianellaPattern(src)))
                .ForMember(dst => dst.Delta, opt => opt.MapFrom(src => Delta(src)));
        }

        private DateTime DrawDate(ThunderBallEntity src)
        {
            if (string.IsNullOrEmpty(src.DrawDate))
                return DateTime.Now;
            return DateTime.Parse(src.DrawDate);
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

        private int BallTotal(ThunderBallEntity src)
        {
            return int.Parse(src.Ball1) + int.Parse(src.Ball2) + int.Parse(src.Ball3) + int.Parse(src.Ball4) + int.Parse(src.Ball5);
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
        private int[] RenatoGianellaPattern(ThunderBallEntity src)
        {
            var rgby = new int[_rules.NoOfMainBalls];
            rgby[BallRGColour(src.Ball1)] += 1;
            rgby[BallRGColour(src.Ball2)] += 1;
            rgby[BallRGColour(src.Ball3)] += 1;
            rgby[BallRGColour(src.Ball4)] += 1;
            rgby[BallRGColour(src.Ball5)] += 1;
            return rgby;
        }

        private static int BallRGColour(string ball)
        {
            var b = int.Parse(ball);
            var div = (int)Math.Ceiling((double)_rules.NoOfBalls / (_rules.NoOfMainBalls - 1));

            for (int i = 0; i < _rules.NoOfMainBalls - 1; i++)
            {
                if (b > div * i && b < div * (i + 1) + 1)
                    return i;
            }
            return -1;
        }
        private int[] Delta(ThunderBallEntity src)
        {
            var balls = new int[_rules.NoOfMainBalls];
            balls[0] = int.Parse(src.Ball1);
            balls[1] = int.Parse(src.Ball2) - int.Parse(src.Ball1);
            balls[2] = int.Parse(src.Ball3) - int.Parse(src.Ball2);
            balls[3] = int.Parse(src.Ball4) - int.Parse(src.Ball3);
            balls[4] = int.Parse(src.Ball5) - int.Parse(src.Ball4);

            return balls;
        }
    }
}
