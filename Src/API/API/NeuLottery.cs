using AutoMapper;
using Domain.Helpers;
using Domain.Models;
using FluentValidation;
using Flurl.Util;
using Flurl.Http;
using Library.Azure.Odata;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;

namespace API
{
    public class NeuLottery
    {
        private readonly IDrawHistory _drawHistory;
        private readonly IHelper? _helper;
        private readonly IGuessHelper? _guesshelper;
        private readonly IValidator<NeuLotteryEntity> _validator;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly ITableStore? _repo;
        private readonly ITableStore? _guess; 
        private readonly ITableStore? _neurepo;

        public NeuLottery(IDrawHistory drawHistory, IHelper helper, IGuessHelper guesshelper, Dictionary<string, ITableStore> dict, IMapper mapper, IValidator<NeuLotteryEntity> validator, ILogger<DrawUpdate> logger)
        {
            _drawHistory = drawHistory;
            _helper = helper;
            _guesshelper = guesshelper;
            _validator = validator;
            _mapper = mapper;
            _logger = logger;

            if (!dict.TryGetValue(Environment.GetEnvironmentVariable("NeuLotteryContainer"), out _neurepo))
            {
                _logger.LogError($"No Table defined :-{Environment.GetEnvironmentVariable("NeuLotteryContainer")}");
            }

            if (!dict.TryGetValue(Environment.GetEnvironmentVariable("TableContainer"), out _repo))
            {
                _logger.LogError($"No Table defined :-{Environment.GetEnvironmentVariable("TableContainer")}");
            }

            if (!dict.TryGetValue(Environment.GetEnvironmentVariable("GuessContainer"), out _guess))
            {
                _logger.LogError($"No Table defined :-{Environment.GetEnvironmentVariable("GuessContainer")}");
            }
        }

        [Function("GetNeuLotteryDraws")]
        [OpenApiOperation(operationId: "GetNeuLotteryDraws", Description = "Get a list of all draws")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Configuration issue")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<Domain.Models.NeuLotteryDto>), Description = "The OK response")]
        public HttpResponseData GetNeuLotteryDraws([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "NeuLottery")] HttpRequestData req)
        {
            _logger.LogInformation("GetNeuLotteryDraws");
            var response = req.CreateResponse();

            if (_helper == null)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.WriteString("GetNeuLotteryDraws : _helper null");
                return response;
            }

            var resDraw = GetNeuLotteryDraws2(new NeuLotteryEntity() { PartitionKey = "Thunderball" });
            //var resGuess = GetGuesses(new NeuLotteryEntity());
            if (resDraw == null)
            {
                response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.WriteString("No Repo/Data");
            }
            else
            {
                response.StatusCode = HttpStatusCode.OK;
                response.Headers.Add("Content-Type", "application/json; charset=utf-8");

                //var ret =  _mapper.Map<Domain.Models.Guesses, Domain.Models.NeuLottery>(resGuess, resDraw);

                var jsonToReturn = JsonConvert.SerializeObject(resDraw);
                response.WriteString($"{jsonToReturn}");
            }

            return response;
        }

        [Function("NeuLotteryUpdate")]
        [OpenApiOperation(operationId: "NeuLotteryUpdate")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.OK, Description = "The OK response")]
        [OpenApiResponseWithoutBody(statusCode: HttpStatusCode.InternalServerError, Description = "Configuration issue")]
        public HttpResponseData NeuLotteryUpdate([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "NeuLottery/update")] HttpRequestData req)
        {
            _logger.LogInformation("NeuLotteryUpdate");
            var response = req.CreateResponse();

            if (_neurepo == null)
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
                    var entityDto = _mapper.Map<NeuLotteryDto>(item);
                    _logger.LogInformation(JsonConvert.SerializeObject(entityDto));

                    var entity = _mapper.Map<NeuLotteryEntity>(entityDto);
                    _validator.ValidateAndThrow(entity);

                    _logger.LogInformation(JsonConvert.SerializeObject(entity));

                    try
                    {
                        _neurepo.Insert(entity);
                    }
                    catch(Exception ex)
                    {
                        if (ex.InnerException != null )
                        {
                            if ((int)HttpStatusCode.Conflict == ((FlurlHttpException)ex.InnerException).StatusCode)
                            {
                                _logger.LogInformation($"Exists Already {((FlurlHttpException)ex.InnerException).StatusCode}");
                            }
                        }
                    }
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

        public List<Domain.Models.NeuLotteryDto>? GetNeuLotteryDraws2(NeuLotteryEntity entity)
        {
            _logger.LogInformation("GetDraws");

            if (_neurepo == null)
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
                var res = _neurepo.Select<Library.Azure.Odata.Models.OData<NeuLotteryEntity>>(sb);
                _logger.LogInformation($"GetDraws result count : {res.Value.Count}");

                try
                {
                    return _mapper.Map<List<NeuLotteryDto>>(res.Value);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return null;
                }
            }
        }

        public Domain.Models.Guesses? GetGuesses(NeuLotteryEntity entity)
        {
            _logger.LogInformation("GetGuesses");

            if (_repo == null)
            {
                _logger.LogWarning("GetGuesses - repo null");
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
                try
                {
                    var res = _guess.Select<Library.Azure.Odata.Models.OData<NeuLotteryEntity>>(sb);
                    _logger.LogInformation($"GetDraws result count : {res.Value.Count}");
                    return _mapper.Map<Domain.Models.Guesses>(res.Value);

                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"Probably doesnt exist yet. Error: {ex}");
                }

                return null;
            }
        }
    }
}
