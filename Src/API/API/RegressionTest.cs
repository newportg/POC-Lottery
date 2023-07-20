using System.Net;
using Domain.Models;
using Library.Azure.Odata;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace API
{
    public class RegressionTest
    {
        private readonly ILogger _logger;
        private readonly ITableStore? _repo;
        private readonly IHelper? _helper;
        private readonly IGuessHelper? _guesshelper;
        private Stats _stats;

        public RegressionTest(Dictionary<string, ITableStore> dict, ILogger<RegressionTest> logger, IHelper helper, IGuessHelper guesshelper)
        {
            _logger = logger;
            _helper = helper;
            _guesshelper = guesshelper;

            // change the guess helpers repo.
            _guesshelper.RegressionTest(dict);

            if (!dict.TryGetValue(Environment.GetEnvironmentVariable("RegTestContainer"), out _repo))
            {
                _logger.LogError($"No Table defined :-{Environment.GetEnvironmentVariable("RegTestContainer")}");
            }

            _stats = new Stats();
        }

        [Function("RegressionTest")]
        [OpenApiOperation(operationId: "RegressionTest", Description = "Run through all lotteries and generate stats")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Configuration issue")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(Stats), Description = "The OK response")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation($"RegressionTest :");
            var response = req.CreateResponse();

            if (_helper == null)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.WriteString("GetGuessByDrawNumber : _helper null");
                return response;
            }

            // Clear database
            var count = 0;
            try
            {
                while (true)
                {
                    _repo.Delete(new Library.Azure.Odata.Models.TableEntity() { PartitionKey = "Thunderball", RowKey = count.ToString() });
                    count++;
                }
            }
            catch(Exception)
            {
                _logger.LogInformation($"Complete : {count}");
            }

            // Get draws
            List<Lottery> res = _helper.GetDraws(new ThunderBallEntity());
            List<Lottery> SortedList = res.OrderBy(o => int.Parse(o.DrawNumber)).ToList();

            foreach (var lottery in res)
            {
                _logger.LogInformation($"RegressionTest :{lottery.DrawNumber}");

                // loop through draws, lowest first
                //
                //  Get guesses
                var guesses = _guesshelper.GetGuesses(new ThunderBallEntity() { DrawNumber = lottery.DrawNumber });
                if (guesses != null)
                {
                    //  Compare with draw
                    var drawResult = _guesshelper.GetDrawResult(new ThunderBallEntity() { DrawNumber = lottery.DrawNumber });
                    //  Generate Stats
                    CalculateStats(drawResult);
                }
                // generate guesses (current draw + 1)
                var tickets = _helper.CreateTickets(int.Parse(lottery.DrawNumber));
                if ( tickets != null)
                    _guesshelper.SaveTickets(tickets);  
                // EOL
            }

            // Report
            if( _stats != null)
            {
                _logger.LogInformation($"Guesses :{_stats.Guesses} Won : £{_stats.Win}");
                _logger.LogInformation($"Wins Equal :{_stats.NoOfDrawsWhereReturnStake} Exceed :{_stats.NoOfDrawsWhereReturnMoreThanStake} LessThan : {_stats.NoOfDrawsWhereReturnLessThanStake} Win Less : {_stats.WinlessDraws}");
                _logger.LogInformation($"Matches 0: {_stats.matchesZero} 1: {_stats.matchesOne} 2: {_stats.matchesTwo} 3: {_stats.matchesTwo} 4: {_stats.matchesTwo} 5: {_stats.matchesTwo}");
                _logger.LogInformation($"Matches And Tball 1: {_stats.matchesOneAndTball} 3: {_stats.matchesThreeAndTball} 4: {_stats.matchesFourAndTball} 5: {_stats.matchesFiveAndTball}");
            }


            response.StatusCode = HttpStatusCode.OK;
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            var jsonToReturn = JsonConvert.SerializeObject(_stats);
            response.WriteString($"{jsonToReturn}");

            return response;
        }

        private void CalculateStats(DrawResult drawResult)
        {
            if(drawResult == null || _stats == null) return;

            _stats.NoOfDraws++;

            var thiswin = 0;
            var thisguesses = 0;

            foreach ( var item in drawResult.GuessResults )
            {
                _stats.Win += item.Win;
                _stats.Guesses++;

                thiswin += item.Win;
                thisguesses++;

                var bd = Breakdown(item.GuessBall);
                if( bd.Item1 == 0 && bd.Item2 == 0 ) _stats.matchesZero++;
                if (bd.Item1 == 1 && bd.Item2 == 0) _stats.matchesOne++;
                if (bd.Item1 == 1 && bd.Item2 == 0) _stats.matchesOneAndTball++;
                if (bd.Item1 == 2 && bd.Item2 == 0) _stats.matchesTwo++;
                if (bd.Item1 == 3 && bd.Item2 == 0) _stats.matchesThree++;
                if (bd.Item1 == 3 && bd.Item2 == 0) _stats.matchesThreeAndTball++;
                if (bd.Item1 == 4 && bd.Item2 == 0) _stats.matchesFour++;
                if (bd.Item1 == 4 && bd.Item2 == 0) _stats.matchesFourAndTball++;
                if (bd.Item1 == 5 && bd.Item2 == 0) _stats.matchesFive++;
                if (bd.Item1 == 5 && bd.Item2 == 1) _stats.matchesFiveAndTball++;
            }

            if (thiswin != 0 && thiswin == thisguesses)
                _stats.NoOfDrawsWhereReturnStake++;
            if (thiswin != 0 && thiswin > thisguesses)
                _stats.NoOfDrawsWhereReturnMoreThanStake++;
            else if (thiswin != 0 && thiswin < thisguesses)
                _stats.NoOfDrawsWhereReturnLessThanStake++;
            else if( thiswin == 0)
                _stats.WinlessDraws++;
        }

        private (int,int) Breakdown(GuessBall[] gb)
        {
            int match = 0;
            int tball = 0;
            foreach (var g in gb)
            {
                if (g.Match && !g.Thunderball) { match++; }
                if (g.Match && g.Thunderball) { tball++; }
            }

            return (match,tball);
        }
    }
}
