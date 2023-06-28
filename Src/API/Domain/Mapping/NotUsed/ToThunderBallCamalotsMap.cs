//using AutoMapper;
//using Common.Mappings;
//using Models;
//using Rules;
//using System.Collections.Generic;
//using System.Linq;

//namespace Mapping
//{
//    public class ToThunderBallCamalotsMap : Profile
//    {
//        public ToThunderBallCamalotsMap() //ThunderBallRules rules)
//        {
//            // Add as many of these lines as you need to map your objects
//            CreateMap<List<ThunderBallDto>, Models.ThunderBallCamalots>()
//                .ForMember(dst => dst.Draws, opt => opt.MapFrom(src => src));

//        }
//    }

//    public class ToThunderBallCamalotMap : Profile
//    {
//        public ToThunderBallCamalotMap() 
//        {
//            // Add as many of these lines as you need to map your objects
//            CreateMap<ThunderBallDto, Models.ThunderBallCamalot>()
//                .ForMember(dst => dst.Ball1, opt => opt.MapFrom(src => src.Balls[0]))
//                .ForMember(dst => dst.Ball2, opt => opt.MapFrom(src => src.Balls[1]))
//                .ForMember(dst => dst.Ball3, opt => opt.MapFrom(src => src.Balls[2]))
//                .ForMember(dst => dst.Ball4, opt => opt.MapFrom(src => src.Balls[3]))
//                .ForMember(dst => dst.Ball5, opt => opt.MapFrom(src => src.Balls[4]))
//                .ForMember(dst => dst.DrawDate, opt => opt.MapFrom(src => src.DrawDate))
//                .ForMember(dst => dst.DrawNumber, opt => opt.MapFrom(src => src.DrawNumber))
//                .ForMember(dst => dst.BallSet, opt => opt.MapFrom(src => src.BallSet))
//                .ForMember(dst => dst.Machine, opt => opt.MapFrom(src => src.Machine))
//                .ForMember(dst => dst.Thunderball, opt => opt.MapFrom(src => src.BonusBalls[0]));


//        }
//    }
//}
