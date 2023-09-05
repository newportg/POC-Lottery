using Domain.Helpers;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Net;

namespace API
{
    public class HotBalls
    {
        private readonly ILogger _logger;
        private readonly IHelper? _helper;
        private readonly IHotBallsHelper? _hotballshelper;

        public HotBalls(ILogger<DrawUpdate> logger, IHelper helper, IHotBallsHelper hotballshelper)
        {
            _logger = logger;
            _helper = helper;
            _hotballshelper = hotballshelper;
        }

        [Function("GetHotBalls")]
        [OpenApiOperation(operationId: "GetHotBalls", Description = "Get HotBalls")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Configuration issue/ Not Found")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(List<HotBalls>), Description = "The OK response")]
        public HttpResponseData GetHotBalls([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "HotBalls")] HttpRequestData req)
        {
            _logger.LogInformation($"GetHotBalls ");
            var response = req.CreateResponse();

            if (_hotballshelper == null)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.WriteString("GetHotBalls : _helper null");
                return response;
            }

            List<Domain.Models.HotBalls>? res = _hotballshelper.GetHotBalls();
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

        [Function("GetHotBallsById")]
        [OpenApiOperation(operationId: "GetHotBallsById", Description = "Get HotBalls")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = false, Type = typeof(int), Description = "DrawNumber")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Configuration issue")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(List<HotBalls>), Description = "The OK response")]
        public HttpResponseData GetHotBallsById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "HotBalls/{id:int?}")] HttpRequestData req, int id)
        {
            _logger.LogInformation($"GetHotBallsById :{id}");
            var response = req.CreateResponse();

            if (_hotballshelper == null)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.WriteString("GetHotBallsById : _helper null");
                return response;
            }

            var res = _hotballshelper.GetHotBalls(id);
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

        [Function("DeleteHotBalls")]
        [OpenApiOperation(operationId: "DeleteHotBalls", Description = "Delete HotBalls")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Configuration issue")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(bool), Description = "The OK response")]
        public HttpResponseData DeleteHotBalls([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "HotBalls")] HttpRequestData req)
        {
            _logger.LogInformation($"DeleteHotBalls :");
            var response = req.CreateResponse();

            if (_hotballshelper == null)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.WriteString("DeleteHotBalls : _helper null");
                return response;
            }

            var res = _hotballshelper.DeleteHotBalls();

            if (res == false)
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

        [Function("DeleteHotBallsById")]
        [OpenApiOperation(operationId: "DeleteHotBallsById", Description = "Delete HotBalls")]
        [OpenApiParameter(name: "id", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "DrawNumber")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Configuration issue")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(bool), Description = "The OK response")]
        public HttpResponseData DeleteHotBallsById([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "HotBalls/{id:int?}")] HttpRequestData req, int id = 0)
        {
            _logger.LogInformation($"DeleteHotBallsById :{id}");
            var response = req.CreateResponse();

            if (_hotballshelper == null)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.WriteString("DeleteHotBallsById : _helper null");
                return response;
            }

            var res = _hotballshelper.DeleteHotBalls(id);

            if (res == false)
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

        [Function("UpdateHotBalls")]
        [OpenApiOperation(operationId: "UpdateHotBalls", Description = "Update HotBalls")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Configuration issue")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(List<HotBalls>), Description = "The OK response")]
        public HttpResponseData UpdateHotBalls([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "HotBalls")] HttpRequestData req)
        {
            _logger.LogInformation($"UpdateHotBalls :");
            var response = req.CreateResponse();

            if (_hotballshelper == null)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.WriteString("UpdateHotBalls : _helper null");
                return response;
            }

            var res = _hotballshelper.UpdateHotBalls();
            if (res == false)
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
    }
}
