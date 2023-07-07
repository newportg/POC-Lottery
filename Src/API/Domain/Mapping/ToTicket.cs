using AutoMapper;
using Domain.Models;

namespace Domain.Mapping
{
    public class ToTicket : Profile
    {
        public ToTicket()
        {
            CreateMap<ThunderBallEntity, Ticket>()
                .ForMember(dst => dst.DrawNumber, opt => opt.MapFrom(src => src.DrawNumber))
                .ForMember(dst => dst.Balls, opt => opt.MapFrom(src => Balls(src)))
                .ForMember(dst => dst.ThunderBall, opt => opt.MapFrom(src => src.Thunderball))
                .ForMember(dst => dst.DrawTotal, opt => opt.MapFrom(src => DrawTotal(src)));
        }

        private int[] Balls(ThunderBallEntity src)
        {
            int[] rtn = new int[5];

            rtn[0] = int.Parse(src.Ball1);
            rtn[1] = int.Parse(src.Ball2);
            rtn[2] = int.Parse(src.Ball3);
            rtn[3] = int.Parse(src.Ball4);
            rtn[4] = int.Parse(src.Ball5);

            return rtn;
        }

        private int DrawTotal(ThunderBallEntity src)
        {
            int rtn = int.Parse(src.Ball1) + int.Parse(src.Ball2) + int.Parse(src.Ball3) + int.Parse(src.Ball4) + int.Parse(src.Ball5) + int.Parse(src.Thunderball);
            return rtn;
        }

    }
}
