using AutoMapper;
using FluentValidation;
using Library.Azure.Odata;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Domain.Models;
using System.Net;
using Library.Azure.Odata.Models;
using Flurl.Util;
using Newtonsoft.Json;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using CsvHelper;

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

        [Function("DrawInfo1")]
        [OpenApiOperation(operationId: "DrawInfo", Description = "Get a list of all draws")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Configuration issue")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Draw/{id}")] HttpRequestData req, int id)
        {
            var response = req.CreateResponse();
            response.StatusCode = HttpStatusCode.InternalServerError;
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            response.WriteString("No Repo :(");
            return response;
        }

        [Function("DrawInfo")]
        [OpenApiOperation(operationId: "DrawInfo", Description = "Get a list of all draws")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Configuration issue")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<ThunderBallEntity>), Description = "The OK response")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Draw")] HttpRequestData req)
        {
            _logger.LogInformation("DrawInfo");
            var response = req.CreateResponse();


            if (_repo == null)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                response.WriteString("No Repo :(");
                return response;
            }

            var entity = new ThunderBallEntity();
            entity.RowKey = "3200";

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

            var res = _repo.Select<Library.Azure.Odata.Models.OData<ThunderBallEntity>>(sb);
            if (res == null)
            {
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.WriteString("No Data");
            }
            else
            {
                response.StatusCode = HttpStatusCode.OK;
                response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                //response.WriteString($"No. of Records : {res.Value.Count}");

                var jsonToReturn = JsonConvert.SerializeObject(res.Value);
                response.WriteString($"{jsonToReturn}");    
            }

            return response;
        }
    }
}
