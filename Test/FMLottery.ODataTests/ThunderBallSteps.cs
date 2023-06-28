using FluentAssertions;
using Common.Mappings;
using Models;
using OData;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;

namespace ODataTests
{
    [Binding]
    public class ThunderBallSteps
    {
        private IThunderBallTable _repo;
        List<LotteryDto> _res;
        LotteryDto _tball;
        int _tableRes;

        [Given(@"A connection to the ThunderBall store")]
        public void GivenAConnectionToTheThunderBallStore()
        {
            var services = Bootstrapper();
            _repo = services.GetRequiredService<IThunderBallTable>();
        }
        
        [Given(@"I Create a list of (.*) draws and Save it")]
        public void GivenICreateAListOfDrawsAndSaveIt(int p0)
        {
            for (var i = 0; i < p0; i++)
            {
                var _tball = CreateThunderBallDTO(1000 + i);
                _tableRes =  _repo.Upsert(_tball);
                //_tableRes.HttpStatusCode.Should().Be(204);
            }
        }

        [Given(@"I Create a LotteryDto object (.*) and Save it")]
        public void GivenICreateALotteryDtoObjectAndSaveIt(int p0)
        {
            var _tball = CreateThunderBallDTO(p0);
            _tableRes = _repo.Upsert(_tball);
            //_tableRes.Should().Be(201);
        }

        [Given(@"I Create a LotteryDto object (.*)")]
        public void GivenICreateALotteryDtoObject(int p0)
        {
            _tball = CreateThunderBallDTO(p0);
        }

        
        [When(@"I ask for a list without specifing a DrawNumber")]
        public void WhenIAskForAListWithoutSpecifingADrawNumber()
        {
            _res = _repo.Select();
        }
        
        [When(@"I ask for a list specifing a DrawNumber (.*)")]
        public void WhenIAskForAListSpecifingADrawNumber(int p0)
        {
            _res = _repo.Select("RowKey le '" + p0.ToString() + "'");
        }
        
        [When(@"I ask for a individual draw (.*)")]
        public void WhenIAskForAIndividualDraw(string p0)
        {
            // _res = _repo.Select("RowKey eq '" + p0 + "'");
            _res = _repo.GetById(p0);
        }
        
        [When(@"I Save the Draw")]
        public void WhenISaveTheDraw()
        {
            _tableRes = _repo.Upsert(_tball);    // Upsert
        }
        
        [When(@"I Delete the Draw (.*)")]
        public void WhenIDeleteTheDraw(int p0)
        {
            var tball = new LotteryDto
            {
                DrawNumber = p0.ToString()
            };

            _tableRes = _repo.Delete(tball);  // Delete
        }
        
        [Then(@"The result should be a list containing (.*) draws")]
        public void ThenTheResultShouldBeAListContainingDraws(int p0)
        {
            //var res = _tres.Result;
            _res.Should().NotBeNull();
            _res.Count.Should().BeGreaterOrEqualTo(p0);
        }
        
        [Then(@"The result should be a list containing all the draws upto the DrawNumber (.*)")]
        public void ThenTheResultShouldBeAListContainingAllTheDrawsUptoTheDrawNumber(int p0)
        {
            //var res = _tres.Result;
            _res.Should().NotBeNull();
            _res.Count.Should().BeGreaterThan(0);
            _res[^1].DrawNumber.Should().Be(p0.ToString());
        }
        
        [Then(@"The result should be a list containing that draw (.*)")]
        public void ThenTheResultShouldBeAListContainingThatDraw(int p0)
        {
            //var res = _tres.Result;
            _res.Should().NotBeNull();
            _res[0].DrawNumber.Should().Be(p0.ToString());
        }
        
        [Then(@"The result should be HTTPStatus (.*)")]
        public void ThenTheResultShouldBeHTTPStatus(int p0)
        {
            //_tableRes.Should().NotBeNull();
            _tableRes.Should().Be(p0);  // 204 : Success : No Content 
        }

        [Then(@"The result should be null")]
        public void ThenTheResultShouldBeNull()
        {
            _res.Should().BeNull();
        }


        //--- Helpers
        private ServiceProvider Bootstrapper()
        {
            var services = new ServiceCollection();
            AutoMapperProfile.AddMapProfiles(services, "OData");

            Environment.SetEnvironmentVariable("TableStorageUrl", "http://127.0.0.1:10002/devstoreaccount1");
            Environment.SetEnvironmentVariable("ThunderBallSasKey", "eed0kJ5uDqBWPNkqW4g%2F8YaL1%2BiRuGuqF6GZM2QLa38%3D");
            Environment.SetEnvironmentVariable("ThunderBallStorageTableName", "ThunderBall");
            Environment.SetEnvironmentVariable("PredictionSasKey",  "Hofq3tmS9lvqDh11VlCilq%2BhwktOC4VHGsC0T1OBg7Y%3D");
            Environment.SetEnvironmentVariable("PredictionStorageTableName", "Predictions");

            services.AddODataServices();
            services.AddLogging();
            return services.BuildServiceProvider();
        }

        private LotteryDto CreateThunderBallDTO(int p0)
        {
            _tball = new LotteryDto
            {
                Lottery = "Thunderball",
                DrawNumber = p0.ToString(),
                DrawDate = DateTime.Today.AddYears(-2).AddDays(1),
                Balls = new List<int>() { 1, 2, 3, 4, 5 },
                BonusBalls = new List<int>() { 1 },
                BallSet = "1",
                Machine = "Fred"
            };
            return _tball;
        }
    }
}
