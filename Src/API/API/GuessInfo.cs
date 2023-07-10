using AutoMapper;
using Azure;
using Domain.Models;
using FluentValidation;
using Flurl.Http;
using Library.Azure.Odata;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Net;


namespace API
{
    public class GuessInfo
    {
        private readonly ILogger _logger;
        private readonly IHelper? _helper;
        private readonly IGuessHelper? _guesshelper;

        public GuessInfo(ILogger<DrawUpdate> logger, IHelper helper, IGuessHelper guesshelper)
        {
            _logger = logger;
            _helper = helper;
            _guesshelper = guesshelper;
        }

        [Function("GetGuessByDrawNumber")]
        [OpenApiOperation(operationId: "GetGuessByDrawNumber", Description = "Get a Guess by id")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "DrawNumber")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Configuration issue")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(List<Ticket>), Description = "The OK response")]
        public HttpResponseData GetGuessByDrawNumber([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Guess/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation($"GetGuessByDrawNumber :{id}");
            var response = req.CreateResponse();

            // If there are already Guesses with drawnumber+1 get those
            var res = _guesshelper.GetGuesses(new ThunderBallEntity() { DrawNumber = id });
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


        [Function("GetTickets")]
        [OpenApiOperation(operationId: "GetTickets", Description = "Get a list tickets")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Configuration issue")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<Ticket>), Description = "The OK response")]
        public HttpResponseData GetTickets([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetTickets")] HttpRequestData req)
        {
            _logger.LogInformation("GetTickets :");
            var response = req.CreateResponse();

            try
            {
                // Get latest drawnumber +1 
                var drawnumber = int.Parse(_helper.LatestDrawNumber());
                drawnumber += 1;
                List<Ticket> tickets = new List<Ticket>();

                // If there are already Guesses with drawnumber+1 get those
                tickets = _guesshelper.GetGuesses(new ThunderBallEntity() { DrawNumber = drawnumber.ToString() });
                if (tickets == null || tickets.Count == 0)
                {
                    _logger.LogInformation($"Generate Guesses for {drawnumber}");

                    // else Create new ones
                    tickets = _helper.CreateTickets();
                }

                if (tickets == null)
                {
                    response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    response.WriteString("No Repo/Data");
                }
                else
                {
                    response.StatusCode = HttpStatusCode.OK;
                    response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                    var jsonToReturn = JsonConvert.SerializeObject(tickets);
                    response.WriteString($"{jsonToReturn}");
                }
            }
            catch (Exception ex)
            {
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.WriteString($"{ex.Message}");
            }

            return response;
        }

        [Function("SaveTickets")]
        [OpenApiOperation(operationId: "SaveTickets", Description = "Save a list tickets")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(List<Ticket>), Description = "Tickets", Example = typeof(List<Ticket>))]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Configuration issue")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Description = "The OK response")]
        public async Task<HttpResponseData> SaveTickets([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "SaveTickets")] HttpRequestData req)
        {
            _logger.LogInformation("SaveTickets");

            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var obj = JsonConvert.DeserializeObject<List<Ticket>>(body);
            var response = req.CreateResponse();

            try
            {
                // Dont save if there are already tickets with this draw number 
                _guesshelper.SaveTickets(obj);

                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                response.StatusCode = HttpStatusCode.OK;
                response.WriteString($"{body}");
            }
            catch (Exception ex)
            {
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.WriteString($"{ex.Message}");
            }

            return response;
        }

        [Function("GetLastGuesses")]
        [OpenApiOperation(operationId: "GetTickets", Description = "Get a list guesses")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Configuration issue")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<Ticket>), Description = "The OK response")]
        public HttpResponseData GetLastGuesses([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetLastGuesses")] HttpRequestData req)
        {
            _logger.LogInformation("GetLastGuesses :");
            var response = req.CreateResponse();

            try
            {
                List<Ticket> tickets = new List<Ticket>();

                // If there are already Guesses with drawnumber+1 get those
                tickets = _guesshelper.GetGuesses(new ThunderBallEntity() );
                if (tickets == null || tickets.Count == 0)
                {
                    _logger.LogInformation($"No Last Guesses ");
                    response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    response.WriteString("No Repo/Data");
                }
                else
                {
                    var maxdrawnumber = tickets.Max(x => x.DrawNumber);
                    List<Ticket>? ret = new List<Ticket>();
                    ret = tickets.Where(x => x.DrawNumber.Contains(maxdrawnumber)).ToList();

                    response.StatusCode = HttpStatusCode.OK;
                    response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                    var jsonToReturn = JsonConvert.SerializeObject(ret);
                    response.WriteString($"{jsonToReturn}");
                }
            }
            catch (Exception ex)
            {
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.WriteString($"{ex.Message}");
            }

            return response;
        }


    }
}
