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

namespace API
{
    public class DrawUpdate
    {
        private readonly IDrawHistory _drawHistory;
        private readonly IValidator<ThunderBallEntity> _validator;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly ITableStore? _repo;

        public DrawUpdate(IDrawHistory drawHistory, Dictionary<string, ITableStore> dict, IMapper mapper, IValidator<ThunderBallEntity> validator, ILogger<DrawUpdate> logger)
        {
            _drawHistory = drawHistory;
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

            if ( _repo  == null ) {
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
    }
}
