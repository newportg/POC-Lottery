using AutoMapper;
using ThunderBall.Models;
using Services;
using ThunderBall.Validation;
using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using System;
using System.Linq;
using Models;

namespace ThunderBall
{
    public class ThunderBallRepository : IThunderBallRepository
    {
        private readonly IValidator<ThunderBallEntity> _validator;
        private readonly IMapper _mapper;
        private readonly CloudTable _table;
        private readonly AzureTableHelper<ThunderBallEntity> _azTableHelper;

        public ThunderBallRepository(IDictionary<string, CloudTable> tables, IMapper mapper, IValidator<ThunderBallEntity> validator)
        {
            _validator = validator;
            _mapper = mapper;
            _table = tables["ThunderBall"];
            _azTableHelper = new AzureTableHelper<ThunderBallEntity>();

            _table.CreateIfNotExists();
        }

        public async Task<List<ThunderBallDto>> GetById(string drawNumber)
        {
            //var tableResult = await _azTableHelper.RetrieveAsync("ThunderBall", drawNumber, _table);
            var tableResult = await _azTableHelper.GetByRowKeyAsync(drawNumber, _table);
            return _mapper.Map<List<ThunderBallDto>>(tableResult);
        }

        public async Task<List<ThunderBallDto>> ListAll()
        {
            var tableResult = await _azTableHelper.ReadAllAsync(_table);
            var res = _mapper.Map<List<ThunderBallDto>>(tableResult);
            return res;
        }

        public async Task<List<ThunderBallDto>> ListUpTo(string drawNumber)
        {
            var tableResult = await _azTableHelper.GetListByKeyAsync("RowKey", drawNumber, _table, QueryComparisons.LessThanOrEqual);
            var res = _mapper.Map<List<ThunderBallDto>>(tableResult);

            List<ThunderBallDto> dtos = _mapper.Map<List<ThunderBallDto>>(tableResult)
                .OrderBy(x => x.DrawNumber)
                .ToList();

            return dtos;

        }

        //[Obsolete]
        //public async Task<bool> SaveAll(List<ThunderBallDto> tballs)
        //{
        //    bool rtn = true;

        //    foreach( var item in tballs)
        //    {
        //        var tr = await Save(item);
        //        if (tr.HttpStatusCode != 204)
        //        {
        //            rtn = false;
        //            break;
        //        }
        //    }

        //    return rtn;
        //}

        [Obsolete]
        public async Task<TableResult> Save(ThunderBallDto tball)
        {
            var entity = _mapper.Map<ThunderBallEntity>(tball);
            await _validator.ValidateAsync(entity, options => options.IncludeRuleSets("NewEntry"));

            var tableResult = await _azTableHelper.InsertOrMerge(_table, entity);
            return tableResult;
        }

        public async Task<TableResult> Delete(ThunderBallDto tball)
        {
            var entity = _mapper.Map<ThunderBallEntity>(tball);
            _validator.ValidateAndThrow(entity);

            var tableResult = await _azTableHelper.DeleteIfExists(_table, entity.PartitionKey, entity.RowKey);
            return tableResult;
        }

        public async Task<TableBatchResult[]> DeleteAll()
        {
            var entity = new ThunderBallEntity();

            var tableResult = await _azTableHelper.DeletePartitionFromTableAsync(entity.PartitionKey, _table);
            return tableResult;
        }
    }
}
