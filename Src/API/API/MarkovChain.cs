using AutoMapper;
using Domain.Helpers;
using Domain.Models;
using FluentValidation;
using Library.Azure.Odata;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;

namespace API
{
    public class MarkovChain
    {
        private readonly IValidator<ThunderBallEntity> _validator;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly ITableStore? _repo;
        private readonly IHelper? _helper;

        private Dictionary<int, Dictionary<int, double>> transitions;

        public MarkovChain(Dictionary<string, ITableStore> dict, IMapper mapper, IValidator<ThunderBallEntity> validator, ILogger<MarkovChain> logger, IHelper helper)
        {
            _validator = validator;
            _mapper = mapper;
            _logger = logger;
            _helper = helper;

            if (!dict.TryGetValue(Environment.GetEnvironmentVariable("TableContainer"), out _repo))
            {
                _logger.LogError($"No Table defined :-{Environment.GetEnvironmentVariable("TableContainer")}");
            }

            transitions = new Dictionary<int, Dictionary<int, double>>();
        }

        [Function("MarkovChain")]
        [OpenApiOperation(operationId: "MarkovChain", Description = "Get Markov Chain")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Configuration issue")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Description = "The OK response")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            _logger.LogInformation("MarkovChain");

            var response = req.CreateResponse();

            if (_helper == null)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.WriteString("MarkovChain : _helper null");
                return response;
            }

            var res = _helper.GetDraws(new ThunderBallEntity());

            AddTransition(1, 1, 0.9);
            AddTransition(1, 2, 0.075);
            AddTransition(1, 3, 0.025);

            AddTransition(2, 1, 0.15);
            AddTransition(2, 2, 0.8);
            AddTransition(2, 3, 0.05);

            AddTransition(3, 1, 0.25);
            AddTransition(3, 2, 0.25);
            AddTransition(3, 3, 0.5);

            int currentState = 3;
            Console.WriteLine(currentState);

            for (int i = 0; i < 10; i++)
            {
                int nextState = GetNextState(currentState);
                Console.WriteLine(nextState);
                currentState = nextState;
            }


            response.StatusCode = HttpStatusCode.OK;
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            response.WriteString("Welcome to Azure Functions!");

            return response;
        }

        public void AddTransition(int state, int nextState, double probability)
        {
            if (!transitions.ContainsKey(state))
            {
                transitions[state] = new Dictionary<int, double>();
            }

            transitions[state][nextState] = probability;
        }

        public int GetNextState(int state)
        {
            if (transitions.ContainsKey(state))
            {
                Dictionary<int, double> probabilities = transitions[state];
                double randomValue = new Random().NextDouble();
                double cumulativeProbability = 0;

                foreach (var kvp in probabilities)
                {
                    cumulativeProbability += kvp.Value;
                    if (randomValue < cumulativeProbability)
                    {
                        return kvp.Key;
                    }
                }
            }

            return -1; // Return a sentinel value or handle error case appropriately
        }
    }
}
