using AutoMapper;
using Domain.Models;
using Domain.Rules;
using Flurl.Util;
using Library.Azure.Odata;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Helpers
{
    public interface IGuessHelper
    {
        bool SaveTickets(List<Ticket> tickets);
        List<Ticket>? GetGuesses(ThunderBallEntity entity);
        DrawResult GetDrawResult(ThunderBallEntity entity);
        void RegressionTest(Dictionary<string, ITableStore> dict);
        List<Ticket> NewGuess(int _drawnumber = 0);
    }

    public class GuessHelper : IGuessHelper
    {
        private readonly IHelper? _helper;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private ITableStore _repo;
        private ThunderBallRules _rules;

        public GuessHelper(IHelper helper, Dictionary<string, ITableStore> dict, IMapper mapper, ILogger<GuessHelper> logger)
        {
            _logger = logger;
            _mapper = mapper;
            _helper = helper;
            _rules = new ThunderBallRules();

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

        public List<Ticket> NewGuess(int _drawnumber = 0)
        {
            // Create a ball selection pool
            //  Get the HotBalls 
            //  remove the bottom third
            //  top up the selection pool to bring it to Guesses*5 (45)
            //  order the balls into RG divisions

            // Get the RG Pattern for previous draws
            //  Get RGGuessCalc, which will return the no. and div for each column.

            var drawNumber = 0;

            if (_drawnumber == 0)
                drawNumber = int.Parse(_helper.LatestDrawNumber());
            else
                drawNumber = _drawnumber;

            drawNumber += 1;

            // Generate 9 rows of guesses
            var hotBalls = _helper.HotBalls();
            var hb = hotBalls.OrderBy(x => x.Value).ToList();
            var third = (int)Math.Ceiling((double)hb.Count / 3);

            for (int i = 0; i < third; i++)
            {
                hb.Remove(hb[0]);
            }

            // Top Up.
            var noOfBalls = _rules.NoOfGuesses() * _rules.NoOfMainBalls;
            var diff = noOfBalls - hb.Count;

            // Randomly select Balls 
            for (int i = 0; i < diff; i++)
            {
                Random random = new Random();
                int index = random.Next(hb.Count);
                hb.Add( hb[index]);
            }

            var div = (int)Math.Ceiling((double)_rules.NoOfBalls / _rules.NoOfMainBalls);
            var divlist = new List<List<int>>();
            var loMark = 0;
            var hiMark = div;

            for (int j = 0; j < _rules.NoOfMainBalls; j++)
            {
                List<int> divl = new List<int>();
                for (int i = 0; i < hb.Count; i++)
                {
                    if (hb[i].Key > loMark && hb[i].Key <= hiMark)
                        divl.Add(hb[i].Key);
                }
                divlist.Add(divl);
                loMark = hiMark;
                hiMark += div;
            }

            // Get the RG analysis
            var rg = new Domain.Models.DrawAnalysis(_helper.GetDraws(new ThunderBallEntity()));
            var divperguess = rg.RGguess;


            List<Ticket> tickets = new List<Ticket>();

            for (int j = 0; j < _rules.NoOfGuesses(); j++)
            {
                Ticket ticket = new Ticket();
                ticket.Balls = new int[_rules.NoOfMainBalls];

                for (int i = 0; i < _rules.NoOfMainBalls; i++)
                {
                    Random random = new Random();
                    int index = random.Next(divlist[i].Count);
                    ticket.Balls[i] = divlist[i][index];
                }

                if( IsUnique(ticket.Balls))
                {
                    Array.Sort(ticket.Balls);

                    // Add ThunderBall
                    var rnd = new Random();
                    ticket.DrawNumber = drawNumber.ToString();
                    ticket.ThunderBall = rnd.Next(1, 14);
                    ticket.DrawTotal = ticket.Balls.Sum();

                    tickets.Add(ticket);
                }
                else
                {
                    _logger.LogInformation("Repeat Loop Not unique");
                    // repeat the loop again
                    j--;
                }
            }

            return tickets;
        }

        public static bool IsUnique(int[] array)
        {
            HashSet<int> seenNumbers = new HashSet<int>();

            foreach (int num in array)
            {
                if (seenNumbers.Contains(num))
                {
                    return false; // Found a repeated value
                }

                seenNumbers.Add(num); // Add the number to the set
            }

            return true; // No repeated value found
        }
    }
}
