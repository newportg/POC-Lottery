using AutoMapper;
using FluentValidation;
using Library.Azure.Odata;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Domain.Models;
using System.Net;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Flurl.Http;
using AdaptiveCards.Templating;
using Azure;
using Domain.Helpers;

namespace API
{
    public class DrawUpdate
    {
        private readonly IDrawHistory _drawHistory;
        private readonly IHelper? _helper;
        private readonly IGuessHelper? _guesshelper;
        private readonly IValidator<ThunderBallEntity> _validator;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly ITableStore? _repo;

        public DrawUpdate(IDrawHistory drawHistory, IHelper helper, IGuessHelper guesshelper, Dictionary<string, ITableStore> dict, IMapper mapper, IValidator<ThunderBallEntity> validator, ILogger<DrawUpdate> logger)
        {
            _drawHistory = drawHistory;
            _helper = helper;
            _guesshelper = guesshelper;
            _validator = validator;
            _mapper = mapper;
            _logger = logger;

            if (!dict.TryGetValue(Environment.GetEnvironmentVariable("TableContainer"), out _repo))
            {
                _logger.LogError($"No Table defined :-{Environment.GetEnvironmentVariable("TableContainer")}");
            }
        }

        [Function("DrawUpdate")]
        [OpenApiOperation(operationId: "DrawUpdate")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Description = "The OK response")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Configuration issue")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Draw/Update")] HttpRequestData req)
        {
            _logger.LogInformation("DrawUpdate");
            var response = req.CreateResponse();

            if (_repo == null)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                response.WriteString("No Repo :(");
                return response;
            }

            var res = _drawHistory.ThunderBall();
            if (res != null)
            {
                foreach (var item in res)
                {
                    var entity = _mapper.Map<ThunderBallEntity>(item);
                    _validator.ValidateAndThrow(entity);

                    _logger.LogInformation(JsonConvert.SerializeObject(entity));
                    _repo.Upsert(entity);
                }

                response.StatusCode = HttpStatusCode.OK;
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                response.WriteString("Updated");
            }
            else
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                response.WriteString("Aaaaaaggghhhh!!!");

            }
            return response;
        }

        [Function("TimerUpdate")]
        [OpenApiOperation(operationId: "TimerUpdate")]
        public void TimerUpdate([TimerTrigger("0 0 9 * * *")] TimerInfo timer, FunctionContext functionContext)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            _logger.LogInformation($"Next timer schedule at: {timer.ScheduleStatus.Next}");

            if (_helper == null || _repo == null || _guesshelper == null)
            {
                return;
            }

            // Get max draw
            var maxDraw = _helper.LatestDrawNumber();
            _logger.LogInformation($"TimerUpdate Max Draw: {maxDraw}");

            // Update
            var res = _drawHistory.ThunderBall();
            if (res != null)
            {
                foreach (var item in res)
                {
                    var entity = _mapper.Map<ThunderBallEntity>(item);
                    _validator.ValidateAndThrow(entity);
                    _repo.Upsert(entity);
                }
            }

            // Get Max Draw - No Change exit
            if (maxDraw == _helper.LatestDrawNumber())
            {
                _logger.LogInformation($"TimerUpdate Max Draw : Nothing new {maxDraw}");
                return;
            }

            // Evaluate last draw
            var drawResult = _guesshelper.GetDrawResult(new ThunderBallEntity() { DrawNumber = maxDraw + 1 });
            // Publish the result
            _ = TeamsNotificationUpdate(drawResult, functionContext);

            // Create a New guess
            var tickets = _helper.CreateTickets(0);
            _guesshelper.SaveTickets(tickets);

        }

        [Function("SendTeams")]
        [OpenApiOperation(operationId: "SendTeams", Description = "send result to teams")]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(DrawResult), Description = "DrawResult", Example = typeof(DrawResult))]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Configuration issue")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(DrawResult), Description = "The OK response")]
        public async Task<HttpResponseData> SendTeams([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Guess/Teams")] HttpRequestData req, FunctionContext functionContext)
        {
            _logger.LogInformation("SendTeams :");

            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var drawResult = JsonConvert.DeserializeObject<DrawResult>(body);
            HttpResponseData response;

            try
            {
                var res = TeamsNotificationUpdate(drawResult, functionContext);
            }
            catch (Exception ex)
            {
                response = req.CreateResponse();
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.WriteString($"{ex.Message}");
                return response;
            }

            response = req.CreateResponse();
            response.StatusCode = HttpStatusCode.OK;
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            var jsonToReturn = JsonConvert.SerializeObject("Published");
            response.WriteString($"{jsonToReturn}");
            return response;
        }

        public async Task<bool> TeamsNotificationUpdate(DrawResult drawResult, FunctionContext functionContext)
        {
            _logger.LogInformation("TeamsNotificationUpdate ");

            try
            {
                _logger.LogInformation("{json}", $"{Directory.GetParent(functionContext.FunctionDefinition.PathToAssembly).FullName}\\TeamsNotificationCard.json");
                _logger.LogInformation("DrawResult : {drawResult}", JsonConvert.SerializeObject(drawResult));

                // Load Card Template
                string template = File.ReadAllText($"{Directory.GetParent(functionContext.FunctionDefinition.PathToAssembly).FullName}\\TeamsNotificationCard.json");

                var rows = await JsonRow(drawResult);
                template = template.Replace("${rows}", rows);

                // Create a Template instance from the template payload
                AdaptiveCardTemplate ACtemplate = new(template);

                // "Expand" the template - this generates the final Adaptive Card payload
                string cardJson = ACtemplate.Expand(drawResult);

                _logger.LogInformation("Teams Card :{cardJson}", cardJson);

                // Send Json to Webhook
                var webhook = Environment.GetEnvironmentVariable("TeamsWebhookNotification");
                if (!string.IsNullOrEmpty(webhook))
                {
                    await webhook.PostStringAsync(cardJson);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("{message}", ex.Message);
            }

            return true;
        }

        private async Task<string> JsonRow(DrawResult drawresult)
        {
            var rows = string.Empty;

            rows += ",{\"type\": \"TableRow\",\"cells\": [";
            rows += $"{{\"type\": \"TableCell\",\"style\": \"accent\",\"items\": [{{\"type\": \"TextBlock\",\"text\": \"{drawresult.DrawNumber}\",\"wrap\": true,\"weight\": \"Bolder\"}}]}},";
            rows += $"{{\"type\": \"TableCell\",\"style\": \"attention\",\"items\": [{{\"type\": \"TextBlock\",\"text\": \"{drawresult.DrawBalls[0]}\",\"wrap\": true,\"weight\": \"Bolder\"}}]}},";
            rows += $"{{\"type\": \"TableCell\",\"style\": \"attention\",\"items\": [{{\"type\": \"TextBlock\",\"text\": \"{drawresult.DrawBalls[1]}\",\"wrap\": true,\"weight\": \"Bolder\"}}]}},";
            rows += $"{{\"type\": \"TableCell\",\"style\": \"attention\",\"items\": [{{\"type\": \"TextBlock\",\"text\": \"{drawresult.DrawBalls[2]}\",\"wrap\": true,\"weight\": \"Bolder\"}}]}},";
            rows += $"{{\"type\": \"TableCell\",\"style\": \"attention\",\"items\": [{{\"type\": \"TextBlock\",\"text\": \"{drawresult.DrawBalls[3]}\",\"wrap\": true,\"weight\": \"Bolder\"}}]}},";
            rows += $"{{\"type\": \"TableCell\",\"style\": \"attention\",\"items\": [{{\"type\": \"TextBlock\",\"text\": \"{drawresult.DrawBalls[4]}\",\"wrap\": true,\"weight\": \"Bolder\"}}]}},";
            rows += $"{{\"type\": \"TableCell\",\"style\": \"attention\",\"items\": [{{\"type\": \"TextBlock\",\"text\": \"{drawresult.DrawThunderBall}\",\"wrap\": true,\"weight\": \"Bolder\"}}]}},";
            rows += $"{{\"type\": \"TableCell\",\"style\": \"attention\",\"items\": [{{\"type\": \"TextBlock\",\"text\": \"\",\"wrap\": true,\"weight\": \"Bolder\"}}]}}";
            rows += "]}";

            foreach (var item in drawresult.GuessResults)
            {
                rows += ",{\"type\": \"TableRow\",\"cells\": [";
                rows += $"{{\"type\": \"TableCell\",\"style\": \"accent\",\"items\": [{{\"type\": \"TextBlock\",\"text\": \"{drawresult.DrawNumber}\",\"wrap\": true,\"weight\": \"Bolder\"}}]}},";
                rows += $"{{\"type\": \"TableCell\",\"style\": {(item.GuessBall[0].Match ? "\"good\"" : "\"warning\"")},\"items\": [{{\"type\": \"TextBlock\",\"text\": \"{item.GuessBall[0].Ball}\",\"wrap\": true,\"weight\": \"Bolder\"}}]}},";
                rows += $"{{\"type\": \"TableCell\",\"style\": {(item.GuessBall[1].Match ? "\"good\"" : "\"warning\"")},\"items\": [{{\"type\": \"TextBlock\",\"text\": \"{item.GuessBall[1].Ball}\",\"wrap\": true,\"weight\": \"Bolder\"}}]}},";
                rows += $"{{\"type\": \"TableCell\",\"style\": {(item.GuessBall[2].Match ? "\"good\"" : "\"warning\"")},\"items\": [{{\"type\": \"TextBlock\",\"text\": \"{item.GuessBall[2].Ball}\",\"wrap\": true,\"weight\": \"Bolder\"}}]}},";
                rows += $"{{\"type\": \"TableCell\",\"style\": {(item.GuessBall[3].Match ? "\"good\"" : "\"warning\"")},\"items\": [{{\"type\": \"TextBlock\",\"text\": \"{item.GuessBall[3].Ball}\",\"wrap\": true,\"weight\": \"Bolder\"}}]}},";
                rows += $"{{\"type\": \"TableCell\",\"style\": {(item.GuessBall[4].Match ? "\"good\"" : "\"warning\"")},\"items\": [{{\"type\": \"TextBlock\",\"text\": \"{item.GuessBall[4].Ball}\",\"wrap\": true,\"weight\": \"Bolder\"}}]}},";
                rows += $"{{\"type\": \"TableCell\",\"style\": {(item.GuessBall[5].Match ? "\"good\"" : "\"warning\"")},\"items\": [{{\"type\": \"TextBlock\",\"text\": \"{item.GuessBall[5].Ball}\",\"wrap\": true,\"weight\": \"Bolder\"}}]}},";
                rows += $"{{\"type\": \"TableCell\",\"style\": {(item.Win > 0 ? "\"good\"" : "\"accent\"")},\"items\": [{{\"type\": \"TextBlock\",\"text\": \"{item.Win}\",\"wrap\": true,\"weight\": \"Bolder\"}}]}}";
                rows += "]}";
            }

            return rows;
        }
    }



    public class TimerInfo
    {
        public TimerStatus? ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class TimerStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
