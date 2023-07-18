using AutoMapper;
using Domain.Models;
using Domain.Rules;
using System.Collections.Generic;

namespace Domain.Mapping
{
    public class ToLottery : Profile
    {
        private static ThunderBallRules _rules;

        public ToLottery()
        {
            _rules = new ThunderBallRules();

            CreateMap<ThunderBallEntity, Lottery>()
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.PartitionKey))
                .ForMember(dst => dst.DrawDate, opt => opt.MapFrom(src => src.DrawDate))
                .ForMember(dst => dst.BallSet, opt => opt.MapFrom(src => src.BallSet))
                .ForMember(dst => dst.Machine, opt => opt.MapFrom(src => src.Machine))
                .ForMember(dst => dst.DrawNumber, opt => opt.MapFrom(src => src.DrawNumber))
                .ForMember(dst => dst.Balls, opt => opt.MapFrom(src => Balls(src)))
                .ForMember(dst => dst.BonusBalls, opt => opt.MapFrom(src => BonusBall(src)))

                .ForMember(dst => dst.BallTotal, opt => opt.MapFrom(src => BallTotal(src)))
                .ForMember(dst => dst.NumOddBalls, opt => opt.MapFrom(src => NumOddBalls(src)))
                .ForMember(dst => dst.Delta, opt => opt.MapFrom(src => Delta(src)));
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
