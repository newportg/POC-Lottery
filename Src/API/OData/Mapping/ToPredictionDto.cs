using AutoMapper;
using Models;
using OData.Models;
using System;
using System.Collections.Generic;

namespace OData.Mapping
{
    public class ToPredictionDto : Profile
    {
        public ToPredictionDto()
        {
            CreateMap<PredictionEntity, PredictionDto>()
                .ForMember(dst => dst.Balls, opt => opt.MapFrom(src => Balls(src)))
                .ForMember(dst => dst.DrawDate, opt => opt.MapFrom(src => DrawDate(src)))
                .ForMember(dst => dst.DrawNumber, opt => opt.MapFrom(src => src.DrawNumber));
        }

        private DateTime DrawDate(PredictionEntity src)
        {
            if (string.IsNullOrEmpty(src.DrawDate))
                return DateTime.Now;
            return DateTime.Parse(src.DrawDate);
        }
        private List<int> Balls(PredictionEntity src)
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
    }
}