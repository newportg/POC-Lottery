using AutoMapper;
using FluentValidation;
using Models;
using OData.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace OData
{
    public class PredictionTable : IPredictionTable
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IValidator<PredictionEntity> _validator;
        private readonly TableStore _store;

        public PredictionTable(IMapper mapper, ILogger<PredictionTable> logger, IValidator<PredictionEntity> validator, TableStore tablestore)
        {
            _mapper = mapper;
            _logger = logger;
            _validator = validator;
            _store = tablestore;

            _logger.LogInformation("PredictionTable");
        }

        public List<PredictionDto> GetById(string drawNumber)
        {
            _logger.LogInformation("GetById");

            if (string.IsNullOrEmpty(drawNumber) || !int.TryParse(drawNumber, out _))
            {
                return null;
            }

            return Select("RowKey eq '" + drawNumber + "'");
        }

        public List<PredictionDto> Select(string filter = null)
        {
            _logger.LogInformation("Select");

            // validate the filter

            try
            {
                var res = _store.Select(filter);
                var configuration = new MapperConfiguration(cfg => { });
                var result = _mapper.Map<List<ThunderBallEntity>>(res.value);
                return _mapper.Map<List<PredictionDto>>(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public int Insert(PredictionDto dto)
        {
            _logger.LogInformation("Insert");

            var entity = _mapper.Map<PredictionEntity>(dto);
            _validator.ValidateAndThrow(entity);
            try
            {
                var res = _store.Insert(entity);
                return res.StatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 404;
            }
        }

        public int Update(PredictionDto dto)
        {
            _logger.LogInformation("Update");

            var entity = _mapper.Map<PredictionEntity>(dto);
            _validator.ValidateAndThrow(entity);
            try
            {
                var res = _store.Update(entity);
                return res.StatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 404;
            }
        }

        public int Upsert(PredictionDto dto)
        {
            _logger.LogInformation("Upsert");

            var entity = _mapper.Map<PredictionEntity>(dto);
            _validator.ValidateAndThrow(entity);

            _logger.LogInformation(JsonConvert.SerializeObject(entity));

            try
            {
                var res = _store.Upsert(entity);
                return res.StatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 404;
            }
        }

        public int Delete(PredictionDto dto)
        {
            _logger.LogInformation("Delete");

            var entity = _mapper.Map<PredictionEntity>(dto);
            _validator.ValidateAndThrow(entity);

            try
            {
                var res = _store.Delete(entity.RowKey, entity.PartitionKey);
                return res.StatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 404;
            }
        }
    }
}
