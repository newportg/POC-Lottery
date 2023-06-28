using AutoMapper;
using FluentValidation;
using Models;
using OData.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace OData
{
    public class ThunderBallTable : IThunderBallTable
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IValidator<ThunderBallEntity> _validator;
        private readonly TableStore _store;

        public ThunderBallTable(IMapper mapper, ILogger<ThunderBallTable> logger, IValidator<ThunderBallEntity> validator, TableStore tablestore)
        {
            _mapper = mapper;
            _logger = logger;
            _validator = validator;
            _store = tablestore;

            _logger.LogInformation("ThunderBallTable");
        }

        public List<LotteryDto> GetById(string drawNumber)
        {
            _logger.LogInformation("GetById");

            if (string.IsNullOrEmpty(drawNumber) || !int.TryParse(drawNumber, out _))
            {
                return null;
            }

            return Select("RowKey eq '" + drawNumber + "'");
        }

        public List<LotteryDto> Select(string filter = null)
        {
            _logger.LogInformation("Select");

            // validate the filter

            try
            {
                var res = _store.Select(filter);
                var configuration = new MapperConfiguration(cfg => { });
                var result = _mapper.Map<List<ThunderBallEntity>>(res.value);
                return _mapper.Map<List<LotteryDto>>(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public int Insert(LotteryDto dto)
        {
            _logger.LogInformation("Insert");

            var entity = _mapper.Map<ThunderBallEntity>(dto);
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

        public int Update(LotteryDto dto)
        {
            _logger.LogInformation("Update");

            var entity = _mapper.Map<ThunderBallEntity>(dto);
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

        public int Upsert(LotteryDto dto)
        {
            _logger.LogInformation("Upsert");

            var entity = _mapper.Map<ThunderBallEntity>(dto);
            _validator.ValidateAndThrow(entity);

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

        public int Delete(LotteryDto dto)
        {
            _logger.LogInformation("Delete");

            var entity = _mapper.Map<ThunderBallEntity>(dto);
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
