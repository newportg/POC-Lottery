﻿using AutoMapper;
using Models;
using ThunderBall.Models;
using System;

namespace ThunderBall
{
    public class ToThunderBallEntity : Profile
    {
        public ToThunderBallEntity()
        {
            CreateMap<ThunderBallDto, ThunderBallEntity>()
                .ForMember(dst => dst.RowKey, opt => opt.MapFrom(src => src.DrawNumber))
                .ForMember(dst => dst.Ball1, opt => opt.MapFrom(src => Balls(0, src)))
                .ForMember(dst => dst.Ball2, opt => opt.MapFrom(src => Balls(1, src)))
                .ForMember(dst => dst.Ball3, opt => opt.MapFrom(src => Balls(2, src)))
                .ForMember(dst => dst.Ball4, opt => opt.MapFrom(src => Balls(3, src)))
                .ForMember(dst => dst.Ball5, opt => opt.MapFrom(src => Balls(4, src)))
                .ForMember(dst => dst.Thunderball, opt => opt.MapFrom(src => BonusBall(0, src)))
                .ForMember(dst => dst.BallSet, opt => opt.MapFrom(src => src.BallSet))
                .ForMember(dst => dst.DrawDate, opt => opt.MapFrom(src => DrawDate(src)))
                .ForMember(dst => dst.DrawNumber, opt => opt.MapFrom(src => src.DrawNumber))
                .ForMember(dst => dst.Machine, opt => opt.MapFrom(src => src.Machine));
        }

        private string DrawDate(ThunderBallDto src)
        {
            if (src.DrawDate == DateTime.MinValue)
                return DateTime.Now.ToString("dd-MMM-yyy");
            return src.DrawDate.ToString("dd-MMM-yyy");
        }

        private string Balls(int idx, ThunderBallDto src)
        {
            if (src.Balls == null)
                return string.Empty;
            return src.Balls[idx].ToString();
        }

        private string BonusBall(int idx, ThunderBallDto src)
        {
            if (src.BonusBalls == null)
                return string.Empty;
            return src.BonusBalls[idx].ToString();
        }
    }
}
