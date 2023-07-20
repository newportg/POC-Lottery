using AutoMapper;
using Domain.Models;
using Flurl.Util;
using Library.Azure.Odata;
using Microsoft.Extensions.Logging;

namespace API
{
    public interface IGuessHelper
    {
        bool SaveTickets(List<Ticket> tickets);
        List<Ticket>? GetGuesses(ThunderBallEntity entity);
        DrawResult GetDrawResult(ThunderBallEntity entity);
        void RegressionTest(Dictionary<string, ITableStore> dict);
    }

    public class GuessHelper : IGuessHelper
    {
        private readonly IHelper? _helper;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private ITableStore _repo;

        public GuessHelper(IHelper helper, Dictionary<string, ITableStore> dict, IMapper mapper, ILogger<GuessHelper> logger)
        {
            _logger = logger;
            _mapper = mapper;
            _helper = helper;

            if (!dict.TryGetValue(Environment.GetEnvironmentVariable("GuessContainer"), out _repo))
            {
                _logger.LogError($"No Table defined :-{Environment.GetEnvironmentVariable("GuessContainer")}");
            }
        }

        public List<Ticket>? GetGuesses(ThunderBallEntity entity)
        {
            _logger.LogInformation("GetGuesses");

            if (_repo == null)
            {
                _logger.LogWarning("GetGuesses - repo null");
                return null;
            }
            else
            {
                var kv = entity.ToKeyValuePairs();
                string sb = string.Empty;
                foreach (var item in kv)
                {
                    if (item.Value != null)
                    {
                        if (sb.Length > 0)
                        {
                            sb += " and ";
                        }

                        sb += " " + item.Key + " eq '" + item.Value + "'";
                    }
                }

                _logger.LogInformation($"GetDraws filter : {sb}");
                try
                {
                    var res = _repo.Select<Library.Azure.Odata.Models.OData<ThunderBallEntity>>(sb);
                    _logger.LogInformation($"GetDraws result count : {res.Value.Count}");
                    return _mapper.Map<List<Ticket>>(res.Value);

                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"Probably doesnt exist yet. Error: {ex}");
                }

                return null;
            }
        }
        public bool SaveTickets(List<Ticket> tickets)
        {
            _logger.LogInformation("SaveTickets");
            if (tickets == null)
            {
                return false;
            }
            if (_repo == null)
            {
                return false;
            }


            // Map Tickets to TableEntity
            var guesses = _mapper.Map<List<ThunderBallEntity>>(tickets);
            if( guesses.Count == 0 ) 
            {
                _logger.LogInformation("SaveTickets Guess Count 0!! ");
                return false;
            }

            // Does the Guess Table contain the ticket drawnumber
            var currentdraw = GetGuesses(new ThunderBallEntity() { DrawNumber = guesses[0].DrawNumber });
            if(currentdraw != null && currentdraw.Count > 0 ) 
            {
                // Already have guessed this draw.
                _logger.LogInformation($"SaveTickets - Previously Guessed {guesses[0].DrawNumber}");
                return false;
            }

            // how many rows already exist.
            // N.B - Check the library Upsert shouldnt need a row count, it should just append.!!!!
            int rowcount = 0;
            try {
                var res = _repo.Select<Library.Azure.Odata.Models.OData<ThunderBallEntity>>("PartitionKey eq 'Thunderball'");
                rowcount = res.Value.Count;
            }
            catch(Exception)
            {
               rowcount = 0;
            }

            // Save
            foreach (var guess in guesses)
            {
                _logger.LogInformation($"SaveTickets - rowCount {rowcount}");

                guess.RowKey = rowcount.ToString();
                _repo.Upsert(guess);
                rowcount++;
            }

            return true;
        }

        public DrawResult? GetDrawResult(ThunderBallEntity entity)
        {
            var guesses = GetGuesses(entity);

            if (_helper == null || guesses == null)
            {
                return null;
            }

            var draws = _helper.GetDraws(entity);
            var drawResult = new DrawResult();
            drawResult.DrawNumber = entity.DrawNumber;

            if (draws != null && draws.Count > 0)
            {
                drawResult.DrawBalls = draws[0].Balls;
                drawResult.DrawThunderBall = draws[0].BonusBalls[0];
            }

            foreach (var guess in guesses)
            {
                var gr = new GuessResult();
                var balllength = guess.Balls.Length;

                for (int i = 0; i < balllength; i++)
                {
                    gr.GuessBall[i] = funcBall(funcfunc(draws, guess.Balls[i]), guess.Balls[i]);
                }

                // ThunderBall
                gr.GuessBall[5] = funcBall(funcfunc(draws,guess.ThunderBall, true), guess.ThunderBall, true);
                gr.Win = WinBreakdown(gr.GuessBall);

                drawResult.GuessResults.Add(gr);
            }

            return drawResult;
        }

        public void RegressionTest(Dictionary<string, ITableStore> dict)
        {
            if (!dict.TryGetValue(Environment.GetEnvironmentVariable("RegTestContainer"), out _repo))
            {
                _logger.LogError($"No Table defined :-{Environment.GetEnvironmentVariable("RegTestContainer")}");
            }
        }

        private bool funcfunc(List<Lottery>draws, int guess, bool tball=false)
        {
            if( draws == null || draws.Count == 0)
            {
                return false;
            }

            if (tball)
            {
                return draws[0].BonusBalls[0] == guess;
            }
            return draws[0].Balls.Contains(guess);
        }
        private GuessBall funcBall( bool drawBall, int guessBall, bool thunderball = false)
        {
            if( drawBall )
            {
                return new GuessBall(guessBall, true, thunderball);
            }
            return new GuessBall(guessBall, false, thunderball);
        }
        private int WinBreakdown(GuessBall[] gb)
        {
            int match = 0;
            int tball = 0;
            foreach (var g in gb)
            {
                if (g.Match && !g.Thunderball) { match++; }
                if (g.Match && g.Thunderball) { tball++; }
            }

            if (match == 0 && tball == 1) { return 3; }
            else if (match == 1 && tball == 1) { return 5; }
            else if (match == 2 && tball == 1) { return 10; }
            else if (match == 3 && tball == 0) { return 10; }
            else if (match == 3 && tball == 1) { return 20; }
            else if (match == 4 && tball == 0) { return 100; }
            else if (match == 4 && tball == 1) { return 250; }
            else if (match == 5 && tball == 0) { return 5000; }
            else if (match == 5 && tball == 1) { return 50000; }
            return 0;
        }
    }
}
