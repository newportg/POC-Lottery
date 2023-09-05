using AutoMapper;
using Domain.Models;
using Flurl.Util;
using Library.Azure.Odata;
using Library.Azure.Odata.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Domain.Helpers
{
    public interface IHotBallsHelper
    {
        List<Models.HotBalls>? GetHotBalls(int drawNumber = 0);
        bool UpdateHotBalls();
        bool DeleteHotBalls(int drawNumber = 0);

    }

    public class HotBallsHelper : IHotBallsHelper
    {
        private readonly IHelper? _helper;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private ITableStore _repo;

        public HotBallsHelper(IHelper helper, Dictionary<string, ITableStore> dict, IMapper mapper, ILogger<HotBallsHelper> logger)
        {
            _helper = helper;
            _logger = logger;
            _mapper = mapper;

            if (!dict.TryGetValue(Environment.GetEnvironmentVariable("HotBallsContainer"), out _repo))
            {
                _logger.LogError($"No Table defined :-{Environment.GetEnvironmentVariable("HotBallsContainer")}");
            }
        }

        public List<HotBalls> GetHotBalls(int drawNumber = 0)
        {
            _logger.LogInformation("GetHotBalls");

            if (_repo == null)
            {
                _logger.LogWarning("GetHotBalls - repo null");
                return null;
            }
            else
            {
                OData<HotBallsEntity> res = null;

                if (drawNumber == 0)
                {
                    res = Select(new HotBallsEntity());
                }
                else
                {
                    res = Select(new HotBallsEntity() { DrawNumber = drawNumber.ToString() });
                }

                if( res == null)
                {
                    _logger.LogInformation($"GetHotBalls Not Found : ");
                    return null;
                }

                _logger.LogInformation($"GetHotBalls result count : {res.Value.Count}");
                return _mapper.Map<List<HotBalls>>(res.Value);
            }
        }

        public bool DeleteHotBalls(int drawNumber = 0)
        {
            _logger.LogInformation("DeleteHotBalls");

            if (_repo == null)
            {
                _logger.LogWarning("DeleteHotBalls - repo null");
                return false;
            }
            else
            {
                try
                {
                    if (drawNumber == 0)
                    {
                        // delete the table
                        _repo.DeleteContainer();
                    }
                    else
                    {
                        var res = Select(new HotBallsEntity() { DrawNumber = drawNumber.ToString() });
                        if (res != null && res.Value.Count == 1)
                        {
                            _logger.LogInformation($"DeleteHotBalls :{res.Value[0].RowKey}");
                            _repo.Delete(res.Value[0]);
                        }

                    }
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"DeleteHotBalls :{ex.Message}");
                    return false;
                }
            }
        }

        public bool UpdateHotBalls()
        {
            _logger.LogInformation("UpdateHotBalls");

            if (_repo == null)
            {
                _logger.LogWarning("UpdateHotBalls - repo null");
                return false;
            }

            var previousdraws = _helper.GetDraws(new ThunderBallEntity());
            if (previousdraws == null)
            {
                return false;
            }

            try
            {
                Dictionary<int, int> hot = new Dictionary<int, int>();
                foreach (var draw in previousdraws)
                {
                    foreach (var ball in draw.Balls)
                    {
                        if (hot.ContainsKey(ball))
                        {
                            hot[ball]++;
                        }
                        else
                        {
                            hot[ball] = 1;
                        }
                    }
                    _logger.LogInformation($"UpdateHotBalls Count: {hot.Count}");

                    var json = JsonConvert.SerializeObject(hot);
                    var hb = new HotBalls() { DrawNumber = draw.DrawNumber, Balls = hot };
                    var entity = _mapper.Map<HotBallsEntity>(hb);

                    json = JsonConvert.SerializeObject(hb);
                    _logger.LogInformation($"UpdateHotBalls hot :{json}");

                    _repo.Upsert(entity);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return false;
        }

        private OData<HotBallsEntity> Select(HotBallsEntity entity)
        {
            if (_repo == null)
            {
                _logger.LogWarning("UpdateHotBalls Select - repo null");
                return null;
            }

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

            _logger.LogInformation($"HotBalls Select filter : {sb}");
            try
            {
                var res = _repo.Select<Library.Azure.Odata.Models.OData<HotBallsEntity>>(sb);
                return res;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Probably doesnt exist yet. Error: {ex}");
            }

            return null;
        }
    }
}
