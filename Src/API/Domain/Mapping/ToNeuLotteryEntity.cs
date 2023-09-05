using AutoMapper;
using Domain.Models;
using Newtonsoft.Json;

namespace Domain.Mapping
{
    internal class ToNeuLotteryEntity : Profile
    {
        public ToNeuLotteryEntity()
        {
            CreateMap<NeuLotteryDto, NeuLotteryEntity>()
                .ForMember(dst => dst.PartitionKey, opt => opt.MapFrom(src => src.Name))
                .ForMember(dst => dst.RowKey, opt => opt.MapFrom(src => src.Draw.DrawNumber))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dst => dst.Draw, opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.Draw)));
        }
    }
}

