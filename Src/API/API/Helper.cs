using AutoMapper;
using Domain.Models;
using Domain.Rules;
using Flurl.Util;
using Library.Azure.Odata;
using Microsoft.Extensions.Logging;

namespace API
{
    public interface IHelper
    {
        List<Lottery>? GetDraws(ThunderBallEntity entity);
        bool HasBeenDrawn( int[] ticket);
        bool HasConsecutiveNumbers(int[] numbers);
        Dictionary<int, int> HotBalls();
        Dictionary<int, int> DrawTotals();
        Dictionary<int, int> Deltas();
        bool IsTwoThirdsHot(Dictionary<int, int> hotballs, int[] ticket);
        string LatestDrawNumber();
        List<Ticket> CreateTickets();
        //void SaveTickets(List<Ticket> tickets);
    }

    public class Helper : IHelper
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly ITableStore? _repo;
        //private readonly ITableStore? _guessRepo;

        public Helper(Dictionary<string, ITableStore> dict, IMapper mapper, ILogger<DrawUpdate> logger)
        {
            _logger = logger;
            _mapper = mapper;

            if (!dict.TryGetValue(Environment.GetEnvironmentVariable("TableContainer"), out _repo))
            {
                _logger.LogError($"No Table defined :-{Environment.GetEnvironmentVariable("TableContainer")}");
            }

            //if (!dict.TryGetValue(Environment.GetEnvironmentVariable("GuessContainer"), out _guessRepo))
            //{
            //    _logger.LogError($"No Table defined :-{Environment.GetEnvironmentVariable("GuessContainer")}");
            //}
        }

        public List<Lottery>? GetDraws(ThunderBallEntity entity)
        {
            _logger.LogInformation("GetDraws");

            if (_repo == null)
            {
                _logger.LogWarning("GetDraws - repo null");
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
                var res = _repo.Select<Library.Azure.Odata.Models.OData<ThunderBallEntity>>(sb);
                _logger.LogInformation($"GetDraws result count : {res.Value.Count}");

                return _mapper.Map<List<Lottery>>(res.Value);
            }
        }

        public bool HasBeenDrawn(int[] ticket)
        {
            _logger.LogInformation("HasBeenDrawn");

            var previousdraws = GetDraws(new ThunderBallEntity());
            for (int i = 0; i < previousdraws.Count; i++)
            {
                return previousdraws[i].HasBeenDrawn(ticket);
            }
            return false;
        }

        public bool HasConsecutiveNumbers(int[] numbers)
        {
            _logger.LogInformation("HasConsecutiveNumbers");

            var two = CountConsecutiveSequences(numbers, 2);
            var three = CountConsecutiveSequences(numbers, 3);
            var four = CountConsecutiveSequences(numbers, 4);
            _logger.LogInformation($"CountConsecutiveSequences 2 = {two}");
            _logger.LogInformation($"CountConsecutiveSequences 3 = {three}");
            _logger.LogInformation($"CountConsecutiveSequences 4 = {four}");

            // Two consequitive numbers is fairly common so not including it.
            if( three != 0 || four != 0 ) 
            {
                return true;
            }

            return false;
        }

        private int CountConsecutiveSequences(int[] sequence, int sequenceLength)
        {
            _logger.LogInformation("CountConsecutiveSequences");

            int count = 0;

            for (int i = 0; i < sequence.Length - (sequenceLength - 1); i++)
            {
                bool isConsecutive = true;

                for (int j = 1; j < sequenceLength; j++)
                {
                    if (sequence[i] != sequence[i + j] - j)
                    {
                        isConsecutive = false;
                        break;
                    }
                }

                if (isConsecutive)
                {
                    count++;
                }
            }

            return count;
        }

        public Dictionary<int, int>? HotBalls()
        {
            _logger.LogInformation("HotBalls");

            var previousdraws = GetDraws(new ThunderBallEntity());
            if( previousdraws == null ) 
            {
                return null;
            }

            Dictionary<int, int> hot = new Dictionary<int, int>();

            foreach (var draw in previousdraws)
            {
                foreach (var ball in draw.Balls)
                {
                    if (hot.ContainsKey(ball))
                    {
                        hot[ball]++;
                    }
                    else
                    {
                        hot[ball] = 1;
                    }
                }
            }
            return hot;
        }

        public Dictionary<int, int>? DrawTotals()
        {
            _logger.LogInformation("DrawTotals");

            var previousdraws = GetDraws(new ThunderBallEntity());
            if (previousdraws == null)
            {
                return null;
            }

            Dictionary<int, int> dic = new Dictionary<int, int>();
            foreach (var draw in previousdraws)
            {
                if (dic.ContainsKey(draw.BallTotal))
                {
                    dic[draw.BallTotal]++;
                }
                else
                {
                    dic[draw.BallTotal] = 1;
                }
            }

            return dic;
        }

        public Dictionary<int, int>? Deltas()
        {
            _logger.LogInformation("Deltas");

            var previousdraws = GetDraws(new ThunderBallEntity());
            if (previousdraws == null)
            {
                return null;
            }

            Dictionary<int, int> dic = new Dictionary<int, int>();
            foreach (var draw in previousdraws)
            {
                foreach (var ball in draw.Delta)
                {
                    if (dic.ContainsKey(ball))
                    {
                        dic[ball]++;
                    }
                    else
                    {
                        dic[ball] = 1;
                    }
                }
            }
            return dic;
        }

        public bool IsTwoThirdsHot(Dictionary<int, int> hotballs, int[] ticket)
        {
            _logger.LogInformation("IsTwoThirdsHot");

            var hb = hotballs.OrderBy(x => x.Value).ToList();

            for (int i = 0; i < ticket.Length - 1; i++)
            {
                int j = hb.FindIndex(x => x.Key == ticket[i]);
                if (j <= 13)
                {
                    return false;
                }
            }

            return true;
        }

        public string? LatestDrawNumber()
        {
            _logger.LogInformation("LatestDrawNumber");

            var previousdraws = GetDraws(new ThunderBallEntity());
            if (previousdraws == null)
            {
                return null;
            }

            string drawnumber = string.Empty;
            drawnumber = previousdraws.Max(a => a.DrawNumber);
            _logger.LogInformation($"Previous Draw - Max Number 1 {drawnumber}");

            return drawnumber;
        }

        public List<Ticket> CreateTickets()
        {
            _logger.LogInformation("CreateTickets");

            var rules = new ThunderBallRules();
            List<Ticket> tickets = new List<Ticket>();

            // Generate all possible combinations of numbers
            IEnumerable<IEnumerable<int>> combinations = Enumerable.Range(1, rules.NoOfBalls)
                .DifferentCombinations(rules.NoOfMainBalls);

            var hotballs = HotBalls();
            var drawNumber = int.Parse(LatestDrawNumber());
            drawNumber += 1;

            Random random = new Random();
            for (int i = 0; i < rules.NoOfGuesses(); i++)
            {
                Ticket ticket = new Ticket();
                bool cons;
                bool drawn;
                bool hot;
                do
                {
                    ticket.Balls = combinations.ElementAt(random.Next(combinations.Count())).ToArray();

                    cons = HasConsecutiveNumbers(ticket.Balls);
                    drawn = HasBeenDrawn(ticket.Balls);
                    hot = IsTwoThirdsHot(hotballs, ticket.Balls);

                    if (cons == true)
                        Console.WriteLine("Rejected - " + string.Join(", ", ticket.Balls));
                    if (drawn == true)
                        Console.WriteLine("Previously Drawn - " + string.Join(", ", ticket.Balls));
                    if (hot == true)
                        Console.WriteLine("Hot - " + string.Join(", ", ticket.Balls));

                } while (cons || drawn || !hot);

                // Add ThunderBall
                var rnd = new Random();
                ticket.DrawNumber = drawNumber.ToString();
                ticket.ThunderBall = rnd.Next(1, 14);
                ticket.DrawTotal = ticket.Balls.Sum();

                tickets.Add(ticket);
            }
            return tickets;
        }


    }
}
