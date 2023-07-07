using AutoMapper;
using Domain.Models;
using Domain.Rules;
using FluentValidation;
using Flurl.Util;
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
    public class DrawInfo
    {
        private readonly ILogger _logger;
        private readonly IHelper? _helper;

        public DrawInfo(ILogger<DrawUpdate> logger, IHelper helper)
        {
            _logger = logger;
            _helper = helper;
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

            var res = _helper.GetDraws(new ThunderBallEntity() { RowKey=id});
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

            var res = _helper.GetDraws(new ThunderBallEntity());
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

            var dic = _helper.HotBalls();
            if (dic == null)
            {
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.WriteString("No Repo/Data");
            }
            else
            {
                response.StatusCode = HttpStatusCode.OK;
                response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var jsonToReturn = JsonConvert.SerializeObject(dic);
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

            var dic = _helper.Deltas();
            if (dic == null)
            {
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.WriteString("No Repo/Data");
            }
            else
            {
                response.StatusCode = HttpStatusCode.OK;
                response.Headers.Add("Content-Type", "application/json; charset=utf-8");
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

            var dic = _helper.DrawTotals();
            if (dic == null)
            {
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.WriteString("No Repo/Data");
            }
            else
            {
                response.StatusCode = HttpStatusCode.OK;
                response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                var jsonToReturn = JsonConvert.SerializeObject(dic);
                response.WriteString($"{jsonToReturn}");
            }

            return response;
        }

    }
}
