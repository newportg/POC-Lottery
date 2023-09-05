using AutoMapper;
using Domain.Models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Domain.Mapping
{
    public class ToHotBalls : Profile
    {
        public ToHotBalls()
        {
            CreateMap<HotBallsEntity, HotBalls>()
                .ForMember(dst => dst.DrawNumber, opt => opt.MapFrom(src => src.DrawNumber))
                .ForMember(dst => dst.Balls, opt => opt.MapFrom(src => Balls(src)));
        }

        private Dictionary<int, int> Balls(HotBallsEntity src)
        {
            return JsonConvert.DeserializeObject<Dictionary<int, int>>(src.Balls);
        }
    }
}
