using Domain.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Net;

namespace API
{
    public class DeltaInfo
    {
        private readonly ILogger _logger;
        private readonly IHelper? _helper;

        public DeltaInfo(ILogger<DrawUpdate> logger, IHelper helper)
        {
            _logger = logger;
            _helper = helper;
        }

        [Function("GetDelta")]
        [OpenApiOperation(operationId: "GetDelta", Description = "Get list of draw deltas and ranges")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Configuration issue")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(Delta), Description = "The OK response")]
        public HttpResponseData GetDelta([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Delta")] HttpRequestData req, string id)
        {
            _logger.LogInformation($"GetDelta :{id}");
            var response = req.CreateResponse();

            // Get Previous Draws
            var previousdraws = _helper.GetDraws(new ThunderBallEntity());

            var delta = new Delta();
            delta.CreateRange(previousdraws);
            
            response.StatusCode = HttpStatusCode.OK;
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");

            var jsonToReturn = JsonConvert.SerializeObject(delta);
            response.WriteString($"{jsonToReturn}");

            return response;
        }
    }
}
