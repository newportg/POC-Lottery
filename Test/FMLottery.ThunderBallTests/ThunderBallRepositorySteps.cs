using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Common.Mappings;
using Models;
using Services;
using ThunderBall;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace ThunderBallTests
{
    [Binding]
    public class ThunderBallRepositorySteps
    {
        private IThunderBallRepository _repo;
        List<ThunderBallDto> _res;
        ThunderBallDto _tball;
        TableResult _tableRes;

        [Given(@"A connection to the ThunderBall Repository")]
        public void GivenAConnectionToTheThunderBallRepository()
        {
            var services = Bootstrapper();
            _repo = services.GetRequiredService<IThunderBallRepository>();
        }

        private ServiceProvider Bootstrapper()
        {
            var connectionString = "UseDevelopmentStorage=true"; //"DefaultEndpointsProtocol=https;AccountName=lottostrdevne;AccountKey=1hx9AYElawno9GfvKzIhJjS3TWpypoSmSXNmEJadUMqNjFKwxb4MFdp/djH1zlNOvmK6tNb203X9da5aKMa/vA==;EndpointSuffix=core.windows.net";
            var tables = "ThunderBall,Lotto";

            var services = new ServiceCollection();
            AutoMapperProfile.AddMapProfiles(services, "ThunderBall");
            services.AddPersistence(connectionString, tables);
            services.AddThunderBallServices();

            return services.BuildServiceProvider();
        }

        [Given(@"I Create a ThunderBallDTO object (.*)")]
        public void GivenICreateAThunderBallDTOObject(int p0)
        {
            _tball = CreateThunderBallDTO(p0);
        }

        [Given(@"I Create a ThunderBallDTO object (.*) and Save it")]
        public async Task GivenICreateAThunderBallDTOObjectAndSaveItAsync(int p0)
        {
            var _tball = CreateThunderBallDTO(p0);
            _tableRes = await _repo.Save(_tball);
            _tableRes.HttpStatusCode.Should().Be(204);
        }

        private ThunderBallDto CreateThunderBallDTO( int p0)
        {
            _tball = new ThunderBallDto
            {
                DrawNumber = p0.ToString(),
                DrawDate = DateTime.Today.AddYears(-2).AddDays(1),
                //DrawDate = DateTime.Parse("28-04-2020"),
                Balls = new List<int>() { 1, 2, 3, 4, 5 },
                BonusBalls = new List<int>() { 1 },
                BallSet = "1",
                Machine = "Fred"
            };
            return _tball;
        }

        [Given(@"I Create a list of (.*) draws and Save it")]
        public async void GivenICreateAListOfDrawsAndSaveIt(int p0)
        {
            for( var i=0; i<p0;i++)
            {
                var _tball = CreateThunderBallDTO( 1000 + i);
                _tableRes = await _repo.Save(_tball);
                _tableRes.HttpStatusCode.Should().Be(204);
            }
        }

        [When(@"I ask for a list without specifing a DrawNumber")]
        public void WhenIAskForAListWithoutSpecifingADrawNumberAsync()
        {
            _res = _repo.ListAll().Result;
        }


        [When(@"I ask for a list specifing a DrawNumber (.*)")]
        public void WhenIAskForAListSpecifingADrawNumber(int p0)
        {
            _res = _repo.ListUpTo(p0.ToString()).Result;
        }

        [When(@"I ask for a individual draw (.*)")]
        public void WhenIAskForAIndividualDrawAsync(int p0)
        {
            _res = _repo.GetById(p0.ToString()).Result;
        }

        [When(@"I Save the Draw")]
        public async Task WhenISaveTheDrawAsync()
        {
            _tableRes = await _repo.Save(_tball);    // Upsert
        }

        [When(@"I Delete the Draw (.*)")]
        public async Task WhenIDeleteTheDrawAsync(string p0)
        {
            var tball = new ThunderBallDto
            {
                DrawNumber = p0
            };

            _tableRes = await _repo.Delete(tball);  // Delete
        }

        [Then(@"The result should be a list containing (.*) draws")]
        public void ThenTheResultShouldBeAListContainingDraws(int p0)
        {
            //var res = _tres.Result;
            _res.Should().NotBeNull();
            _res.Count.Should().Be(p0);
        }

        [Then(@"The result should be a list containing that draw (.*)")]
        public void ThenTheResultShouldBeAListContainingThatDraw(int p0)
        {
            //var res = _tres.Result;
            _res.Should().NotBeNull();
            _res[0].DrawNumber.Should().Be(p0.ToString());
        }

        [Then(@"The result should be a list containing all the draws upto the DrawNumber (.*)")]
        public void ThenTheResultShouldBeAListContainingAllTheDrawsUptoTheDrawNumber(int p0)
        {
            //var res = _tres.Result;
            _res.Should().NotBeNull();
            _res.Count.Should().BeGreaterThan(0);
            _res[^1].DrawNumber.Should().Be(p0.ToString());
        }

        [Then(@"The result should be HTTPStatus (.*)")]
        public void ThenTheResultShouldBeHTTPStatus(int p0)
        {
            _tableRes.Should().NotBeNull();
            _tableRes.HttpStatusCode.Should().Be(p0);  // 204 : Success : No Content 
        }

        [AfterScenario]
        public void AfterScenario()
        {
            _repo.DeleteAll();
        }
    }
}
