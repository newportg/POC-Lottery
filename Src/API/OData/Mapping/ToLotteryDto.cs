using AutoMapper;
using Models;
using OData.Models;
using System;
using System.Collections.Generic;

namespace OData.Mapping
{
    public class ToLotteryDto : Profile
    {
        public ToLotteryDto()
        {
            CreateMap<ThunderBallEntity, LotteryDto>()
                .ForMember(dst => dst.Lottery, opt => opt.MapFrom(src => src.PartitionKey))
                .ForMember(dst => dst.Balls, opt => opt.MapFrom(src => Balls(src)))
                .ForMember(dst => dst.BonusBalls, opt => opt.MapFrom(src => BonusBall(src)))
                .ForMember(dst => dst.BallSet, opt => opt.MapFrom(src => src.BallSet))
                .ForMember(dst => dst.DrawDate, opt => opt.MapFrom(src => DrawDate(src)))
                .ForMember(dst => dst.DrawNumber, opt => opt.MapFrom(src => src.DrawNumber))
                .ForMember(dst => dst.Machine, opt => opt.MapFrom(src => src.Machine));
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
    }
}
