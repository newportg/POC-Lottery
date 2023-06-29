using AutoMapper;
using Domain.Models;
using Domain.Rules;
using FluentValidation;
using Flurl.Util;
using Library.Azure.Odata;
using Library.Azure.Odata.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;

namespace API
{
    public class DrawInfo
    {
        private readonly IValidator<ThunderBallEntity> _validator;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly ITableStore? _repo;

        public DrawInfo(Dictionary<string, ITableStore> dict, IMapper mapper, IValidator<ThunderBallEntity> validator, ILogger<DrawUpdate> logger)
        {
            _validator = validator;
            _mapper = mapper;
            _logger = logger;

            if (!dict.TryGetValue(Environment.GetEnvironmentVariable("TableContainer"), out _repo))
            {
                _logger.LogError($"No Table defined :-{Environment.GetEnvironmentVariable("TableContainer")}");
            }
        }

        [Function("GetDrawByRowKey")]
        [OpenApiOperation(operationId: "GetDrawByRowKey", Description = "Get a draw by id")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "RowKey")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Configuration issue")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(List<Lottery>), Description = "The OK response")]
        public HttpResponseData GetDrawById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Draw/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation($"GetDrawByRowKey :{id}");
            var response = req.CreateResponse();

            var entity = new ThunderBallEntity();
            entity.RowKey = id;

            var res = GetDraws(new ThunderBallEntity() { RowKey=id});
            if (res == null)
            {
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.WriteString("No Repo/Data");
            }
            else
            {
                response.StatusCode = HttpStatusCode.OK;
                response.Headers.Add("Content-Type", "application/json; charset=utf-8");

                var jsonToReturn = JsonConvert.SerializeObject(res);
                response.WriteString($"{jsonToReturn}");
            }

            return response;
        }

        [Function("GetDraws")]
        [OpenApiOperation(operationId: "GetDraws", Description = "Get a list of all draws")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Configuration issue")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<Lottery>), Description = "The OK response")]
        public HttpResponseData GetDraws([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Draw")] HttpRequestData req)
        {
            _logger.LogInformation("DrawInfo");
            var response = req.CreateResponse();

            var res = GetDraws(new ThunderBallEntity());
            if (res == null)
            {
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.WriteString("No Repo/Data");
            }
            else
            {
                response.StatusCode = HttpStatusCode.OK;
                response.Headers.Add("Content-Type", "application/json; charset=utf-8");

                var jsonToReturn = JsonConvert.SerializeObject(res);
                response.WriteString($"{jsonToReturn}");    
            }

            return response;
        }

        [Function("GetHotBalls")]
        [OpenApiOperation(operationId: "GetHotBalls", Description = "Get a list hot balls")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Configuration issue")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Dictionary<int, int>), Description = "The OK response")]
        public HttpResponseData GetHotBalls([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "HotBalls")] HttpRequestData req)
        {
            _logger.LogInformation("GetHotBalls :");
            var response = req.CreateResponse();

            var res = GetDraws(new ThunderBallEntity());
            if (res == null)
            {
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.WriteString("No Repo/Data");
            }
            else
            {
                response.StatusCode = HttpStatusCode.OK;
                response.Headers.Add("Content-Type", "application/json; charset=utf-8");

                Dictionary<int, int> hot = new Dictionary<int, int>();

                foreach (var draw in res)
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

                var jsonToReturn = JsonConvert.SerializeObject(hot);
                response.WriteString($"{jsonToReturn}");
            }

            return response;
        }

        [Function("GetDeltas")]
        [OpenApiOperation(operationId: "GetDeltas", Description = "Get a list deltas")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Configuration issue")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Dictionary<int, int>), Description = "The OK response")]
        public HttpResponseData GetDelta([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Deltas")] HttpRequestData req)
        {
            _logger.LogInformation("GetDeltas :");
            var response = req.CreateResponse();

            var res = GetDraws(new ThunderBallEntity());
            if (res == null)
            {
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.WriteString("No Repo/Data");
            }
            else
            {
                response.StatusCode = HttpStatusCode.OK;
                response.Headers.Add("Content-Type", "application/json; charset=utf-8");

                Dictionary<int, int> dic = new Dictionary<int, int>();

                foreach (var draw in res)
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

                var jsonToReturn = JsonConvert.SerializeObject(dic);
                response.WriteString($"{jsonToReturn}");
            }

            return response;
        }

        [Function("GetDrawTotals")]
        [OpenApiOperation(operationId: "GetDrawTotals", Description = "Get a list draw totals")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Configuration issue")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Dictionary<int, int>), Description = "The OK response")]
        public HttpResponseData GetDrawTotal([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "DrawTotals")] HttpRequestData req)
        {
            _logger.LogInformation("GetDrawTotals :");
            var response = req.CreateResponse();

            var res = GetDraws(new ThunderBallEntity());
            if (res == null)
            {
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.WriteString("No Repo/Data");
            }
            else
            {
                response.StatusCode = HttpStatusCode.OK;
                response.Headers.Add("Content-Type", "application/json; charset=utf-8");

                Dictionary<int, int> dic = new Dictionary<int, int>();
                foreach (var draw in res)
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

                var jsonToReturn = JsonConvert.SerializeObject(dic);
                response.WriteString($"{jsonToReturn}");
            }

            return response;
        }

        [Function("GetTickets")]
        [OpenApiOperation(operationId: "GetTickets", Description = "Get a list tickets")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Configuration issue")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<int[]>), Description = "The OK response")]
        public HttpResponseData GetTickets([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetTickets")] HttpRequestData req)
        {
            _logger.LogInformation("GetTickets :");
            var response = req.CreateResponse();
            var rules = new ThunderBallRules();

            List<int[]> tickets = new List<int[]>();

            var res = GetDraws(new ThunderBallEntity());
            if (res == null)
            {
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.WriteString("No Repo/Data");
            }
            else
            {
                response.StatusCode = HttpStatusCode.OK;
                response.Headers.Add("Content-Type", "application/json; charset=utf-8");

                // Generate all possible combinations of numbers
                IEnumerable<IEnumerable<int>> combinations = Enumerable.Range(1, rules.NoOfBalls)
                    .DifferentCombinations(rules.NoOfMainBalls);

                Random random = new Random();

                // Randomly select tickets from the combinations
                for (int i = 0; i < rules.NoOfGuesses(); i++)
                {
                    int[] ticket;
                    bool cons;
                    bool drawn;
                    do
                    {
                        ticket = combinations.ElementAt(random.Next(combinations.Count())).ToArray();

                        cons = HasConsecutiveNumbers(ticket);
                        drawn = HasBeenDrawn(res, ticket);

                        if (cons == true)
                            Console.WriteLine("Rejected - " + string.Join(", ", ticket));
                        if (drawn == true)
                            Console.WriteLine("Previously Drawn - " + string.Join(", ", ticket));

                    } while (cons && !drawn);

                    // Add ThunderBall
                    Random rnd = new Random();
                    int[] tic = new int[7];
                    for (int j = 0; j < ticket.Length; j++)
                    {
                        tic[j] = ticket[j];
                    }
                    tic[5] = rnd.Next(1, 14);
                    tic[6] = tic[0] + tic[1] + tic[2] + tic[3] + tic[4] + tic[5];

                    tickets.Add(tic);
                }

                var jsonToReturn = JsonConvert.SerializeObject(tickets);
                response.WriteString($"{jsonToReturn}");
            }

            return response;
        }


        private List<Lottery>? GetDraws(ThunderBallEntity entity)
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

        private bool HasBeenDrawn(List<Lottery> previousdraws,  int[] ticket)
        {
            for (int i = 0; i < previousdraws.Count; i++)
            {
                return previousdraws[i].HasBeenDrawn(ticket);
            }
            return false;
        }

        private bool HasConsecutiveNumbers(int[] numbers)
        {
            for (int i = 0; i < numbers.Length - 1; i++)
            {
                if (numbers[i] == numbers[i + 1] - 1)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
