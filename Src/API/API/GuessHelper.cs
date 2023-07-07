using AutoMapper;
using Domain.Models;
using Flurl.Util;
using Library.Azure.Odata;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;

namespace API
{
    public interface IGuessHelper
    {
        bool SaveTickets(List<Ticket> tickets);
        List<Ticket>? GetGuesses(ThunderBallEntity entity);
    }

    public class GuessHelper : IGuessHelper
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly ITableStore? _repo;

        public GuessHelper(Dictionary<string, ITableStore> dict, IMapper mapper, ILogger<DrawUpdate> logger)
        {
            _logger = logger;
            _mapper = mapper;

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

            // Map Tickets to TableEntity
            var guesses = _mapper.Map<List<ThunderBallEntity>>(tickets);
            if( guesses.Count == 0 ) 
            {
                _logger.LogInformation("SaveTickets Guess Count 0!! ");
                return false;
            }

            // Does the Guess Table contain the ticket drawnumber
            tickets = GetGuesses(new ThunderBallEntity() { DrawNumber = guesses[0].DrawNumber });
            if( tickets != null || tickets.Count > 0 ) 
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
                rowcount++;
            }
            catch(Exception ex)
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
    }
}
