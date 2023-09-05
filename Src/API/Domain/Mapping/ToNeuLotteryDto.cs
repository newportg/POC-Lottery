using AutoMapper;
using Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Domain.Mapping
{
    internal class ToNeuLotteryDto : Profile
    {
        private Models.Rules _rules;

        public ToNeuLotteryDto()
        {
            _rules = Rules();

            CreateMap<LotteryDto, NeuLotteryDto>()
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Lottery))
                .ForMember(dst => dst.Draw, opt => opt.MapFrom(src => src));

            CreateMap<LotteryDto, DrawEntity>()
                .ForMember(dst => dst.DrawDate, opt => opt.MapFrom(src => DrawDate(src)))
                .ForMember(dst => dst.DrawNumber, opt => opt.MapFrom(src => src.DrawNumber))
                .ForMember(dst => dst.BallSet, opt => opt.MapFrom(src => src.BallSet))
                .ForMember(dst => dst.Machine, opt => opt.MapFrom(src => src.Machine))
                .ForMember(dst => dst.Balls, opt => opt.MapFrom(src => src));

            CreateMap<LotteryDto, BallEntity>()
                .ForMember(dst => dst.MainBalls, opt => opt.MapFrom(src => Balls(src.Balls)))
                .ForMember(dst => dst.BonusBalls, opt => opt.MapFrom(src => Balls(src.BonusBalls)));

            //-------------------------------------

            CreateMap<NeuLotteryEntity, NeuLotteryDto>()
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dst => dst.Draw, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<DrawEntity>(src.Draw)));

        }

        private string DrawDate(LotteryDto src)
        {
            if (src.DrawDate == DateTime.MinValue)
                return DateTime.Now.ToString("dd-MMM-yyy");
            return src.DrawDate.ToString("dd-MMM-yyy");
        }

        private List<Ball> Balls(List<int> src )
        {
            var lball = new List<Ball>();

            for( var i = 0; i < src.Count; i++ )
            {
                lball.Add( new Models.Ball() { Number = src[i] });
            }
            return lball;
        }

        private Models.Rules Rules()
        {
            Models.Rules rules = new Models.Rules();

            rules.MaxMainBall = 39;
            rules.MaxBonusBall = 14;
            rules.NoOfBallsToSelect = 5;
            rules.NoOfBonusBallsToSelect = 1;

            // Is the minimum no. of Main Balls without a Bonus ball - 1 Guess
            // MainBalls = 3 Bonus Balls = 0 Pays 10
            // (Payout - (CostPerGuess*1)) / CostPerGuess
            // (10 - 1) = (9 / 1) = 9 
            rules.NoOfGuesses = (10 - rules.CostPerGuess) / rules.CostPerGuess;
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

    }
}

