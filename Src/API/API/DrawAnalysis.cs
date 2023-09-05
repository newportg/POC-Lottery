using System.Net;
using Domain.Helpers;
using Domain.Models;
using Flurl.Util;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace API
{
    public class DrawAnalysis
    {
        private readonly ILogger _logger;
        private readonly IHelper? _helper;

        public DrawAnalysis(ILogger<DrawAnalysis> logger, IHelper helper)
        {
            _logger = logger;
            _helper = helper;
        }

        [Function("GetDrawAnalysis")]
        [OpenApiOperation(operationId: "GetDrawAnalysis", Description = "Get a list of all draws")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Configuration issue")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(Domain.Models.DrawAnalysis), Description = "The OK response")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var da = new Domain.Models.DrawAnalysis(_helper.GetDraws(new ThunderBallEntity()));

            var response = req.CreateResponse();
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            var jsonToReturn = JsonConvert.SerializeObject(da);
            response.WriteString($"{jsonToReturn}");
            return response;
        }
    }
}
