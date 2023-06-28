using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Common.Behaviours;
using Common.Mappings;
using Models;
using Rules;
using Validation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow;

namespace CoreTests
{
    [Binding]
    public class ThunderBallValidatorSteps
    {
        //private IMapper _mapper;
        private Models.ThunderBall _src;
        private ThunderBallValidator _validator;
        private ValidationResult _results;

        private ServiceProvider Bootstrapper()
        {
            var services = new ServiceCollection();
            services.AddMapProfiles("FMLottery");
            services.AddValidatorsFromAssembly(typeof(Domain.Bootstrapper).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

            return services.BuildServiceProvider();
        }

        [Given(@"I have created a Empty Thunderball object")]
        public void GivenIHaveCreatedAEmptyThunderballObject()
        {
            _ = Bootstrapper();
            //_mapper = services.GetRequiredService<IMapper>();

            _src = new Models.ThunderBall();
            _validator = new ThunderBallValidator();
        }

        [Given(@"I have created a Thunderball object")]
        public void GivenIHaveCreatedAThunderballObject()
        {
            _ = Bootstrapper();
            //_mapper = services.GetRequiredService<IMapper>();
            _validator = new ThunderBallValidator();

            _src = new Models.ThunderBall
            {
                Draws = new List<ThunderBallDto>
            {
                new ThunderBallDto
                {
                    DrawDate = DateTime.Today,
                    BallTotal = 15,
                    NumOddBalls = 3,
                    RenatoGianellaPattern = new int[4] { 5, 0, 0, 0 },
                    BallSet = "1",
                    Machine = "Test",
                    DrawNumber = "9999",
                    Balls = new List<int>() { 1, 2, 3, 4, 5 },
                    BonusBalls = new List<int> { 1 }
                }
            },
                BallTotalAvg = 15,
                NumBallDrawings = new List<Ball>
            {
                new Ball
                {
                    BallNumber = "1",
                    Count = 1,
                    Draws= new List<string>() { "9999" }
                },
                                new Ball
                {
                    BallNumber = "2",
                    Count = 1,
                    Draws= new List<string>() { "9999" }
                },
                new Ball
                {
                    BallNumber = "3",
                    Count = 1,
                    Draws= new List<string>() { "9999" }
                },
                new Ball
                {
                    BallNumber = "4",
                    Count = 1,
                    Draws= new List<string>() { "9999" }
                },
                new Ball
                {
                    BallNumber = "5",
                    Count = 1,
                    Draws= new List<string>() { "9999" }
                }
            },
                OddBalls = new int[6] { 0, 0, 0, 3, 0, 0 },
                RenatoGianellaOccurrance = new List<RenatoGianella>()
            {
                new RenatoGianella()
                {
                    Name = "5000",
                    Pattern = new int[4] { 5, 0, 0, 0 },
                    Count = 1,
                    Draws = new List<string>() { "9999" }
                }
            }
            };
        }

        [When(@"I validate it")]
        public void WhenIValidateIt()
        {
            _results = _validator.Validate(_src);
        }

        [Then(@"the result should be false")]
        public void ThenTheResultShouldBeFalse()
        {
            _results.IsValid.Should().BeFalse();
        }

        [Then(@"the result should be true")]
        public void ThenTheResultShouldBeTrue()
        {
            _results.IsValid.Should().BeTrue();
        }
    }
}
