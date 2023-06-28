//using FluentAssertions;
//using Common.Mappings;
//using Models;
//using OData;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using TechTalk.SpecFlow;

//namespace ODataTests
//{
//    [Binding]
//    public class PredictionSteps
//    {
//        private IPredictionTable _repo;
//        List<PredictionDto> _res;
//        List<PredictionDto> _preds;
//        int _tableRes;

//        [Given(@"A connection to the Prediction store")]
//        public void GivenAConnectionToThePredictionStore()
//        {
//            var services = Bootstrapper();
//            _repo = services.GetRequiredService<IPredictionTable>();
//        }
       
//        [Given(@"I Create a list of Predictions with the DrawNumber of (.*)")]
//        public void GivenICreateAListOfPredictionsWithTheDrawNumberOf(int p0)
//        {
//            CreatePredictionDTO(p0);
//        }

//        [Given(@"Save the list of predictions")]
//        public void GivenSaveTheListOfPredictions()
//        {
//            WhenISaveThePredictions();
//        }


//        [When(@"I ask for a list by specifying the DrawNumber of (.*)")]
//        public void WhenIAskForAListBySpecifyingTheDrawNumberOf(int p0)
//        {
//            _res = _repo.Select("RowKey le '" + p0.ToString() + "'");
//        }
        
//        [When(@"I Save the Predictions")]
//        public void WhenISaveThePredictions()
//        {
//            foreach (var item in _preds)
//            {
//                _tableRes = _repo.Upsert(item);    // Upsert
//                ThenTheResultShouldBeHTTP_Status(204);
//            }
//        }
        
//        [When(@"I Delete the Predictions for DrawNumber (.*)")]
//        public void WhenIDeleteThePredictionsForDrawNumber(int p0)
//        {
//            var pred = new PredictionDto
//            {
//                //Game = "Thunderball",
//                DrawNumber = p0.ToString()
//            };

//            _repo.Delete(pred);  // Delete - needs the row key or a different delete
//        }
        
//        [Then(@"The result should be a list containing all of the predictions for DrawNumber (.*)")]
//        public void ThenTheResultShouldBeAListContainingAllOfThePredictionsForDrawNumber(int p0)
//        {
//            var res = _res;
//            res.Should().NotBeNull();
//            res.Count.Should().BeGreaterThan(0);
//            res[0].DrawNumber.Should().Be(p0.ToString());
//        }

//        [Then(@"The result should be HTTP_Status (.*)")]
//        public void ThenTheResultShouldBeHTTP_Status(int p0)
//        {
//            _tableRes.Should().Be(p0);  // 204 : Success : No Content 
//        }

//        //--- Helpers
//        private ServiceProvider Bootstrapper()
//        {
//            var services = new ServiceCollection();
//            AutoMapperProfile.AddMapProfiles(services, "FMLottery");

//            Environment.SetEnvironmentVariable("TableStorageUrl", "http://127.0.0.1:10002/devstoreaccount1");
//            Environment.SetEnvironmentVariable("ThunderBallSasKey", "eed0kJ5uDqBWPNkqW4g%2F8YaL1%2BiRuGuqF6GZM2QLa38%3D");
//            Environment.SetEnvironmentVariable("ThunderBallStorageTableName", "ThunderBall");
//            Environment.SetEnvironmentVariable("PredictionSasKey", "Hofq3tmS9lvqDh11VlCilq%2BhwktOC4VHGsC0T1OBg7Y%3D");
//            Environment.SetEnvironmentVariable("PredictionStorageTableName", "Predictions");

//            //http://127.0.0.1:10002/devstoreaccount1/Predictions
//            //?sv=2015-04-05
//            //&si=Application
//            //&tn=Predictions
//            //&sig=Hofq3tmS9lvqDh11VlCilq%2BhwktOC4VHGsC0T1OBg7Y%3D

//            services.AddODataServices(); 
//            services.AddLogging();
//            return services.BuildServiceProvider();
//        }

//        private void CreatePredictionDTO(int p0)
//        {
//            _preds = new List<PredictionDto>
//            {
//                new PredictionDto()
//                {
//                    //Game = "Thunderball",
//                    DrawNumber = p0.ToString(),
//                    DrawDate = DateTime.Today.AddYears(-2).AddDays(1),
//                    Balls = new List<int>() { 1, 2, 3, 4, 5 }
//                },
//                new PredictionDto()
//                {
//                    //Game = "Thunderball",
//                    DrawNumber = p0.ToString(),
//                    DrawDate = DateTime.Today.AddYears(-2).AddDays(1),
//                    Balls = new List<int>() { 5, 4, 3, 2, 1 }
//                }
//            };
//        }

//        //---------------------------------

//        //[AfterScenario]
//        //public void AfterScenario()
//        //{
//        //    _repo..DeleteAll();
//        //}
//    }
//}
