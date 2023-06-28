using FluentAssertions;
using Camalot;
using Common.Mappings;
using Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using TechTalk.SpecFlow;

namespace CamalotTests
{
    [Binding]
    public class DrawHistorySteps
    {
        IDrawHistory _drawHistory;
        List<LotteryDto> _res;

        private ServiceProvider Bootstrapper()
        {
            var services = new ServiceCollection();
            AutoMapperProfile.AddMapProfiles(services, "Camalot");
            services.AddCamalotServices();
            services.AddLogging();

            return services.BuildServiceProvider();
        }

        [Given(@"I ama lottery application")]
        public void GivenIAmaLotteryApplication()
        {
            var services = Bootstrapper();
            _drawHistory = services.GetRequiredService<IDrawHistory>();
        }
        
        [When(@"I make a request for lottery draw history")]
        public void WhenIMakeARequestForLotteryDrawHistory()
        {
            _res = _drawHistory.ThunderBall();
        }
        
        [Then(@"Then I will have a List of lottery draws\.")]
        public void ThenThenIWillHaveAListOfLotteryDraws_()
        {
            _res.Should().HaveCountGreaterOrEqualTo(102);
        }
    }
}
