using AutoMapper;
using Camalot.Models;
using CsvHelper;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Camalot
{
    public class DrawHistory : IDrawHistory
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public DrawHistory(IMapper mapper, ILogger<DrawHistory> logger)
        {
            _mapper = mapper;
            _logger = logger;

            _logger.LogInformation("CamalotThunderBallAsync");
        }

        public List<LotteryDto> ThunderBall()
        {
            try
            {
                _logger.LogInformation("Call Camalot");

                Url url = new("https://www.national-lottery.co.uk/results/thunderball/draw-history/csv");
                var data = url.GetStringAsync().Result;

                _logger.LogInformation("CsvReader");

                using TextReader reader = new StringReader(data);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                csv.Read();
                csv.ReadHeader();

                var csvRecords = csv.GetRecords<ThunderBallRaw>().ToList();
                return _mapper.Map<List<LotteryDto>>(csvRecords);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return null;
        }
        public List<LotteryDto> Lotto()
        {
            try
            {
                _logger.LogInformation("Call Camalot");

                Url url = new("https://www.national-lottery.co.uk/results/lotto/draw-history/csv");
                var data = url.GetStringAsync().Result;

                _logger.LogInformation("CsvReader");

                using TextReader reader = new StringReader(data);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                csv.Read();
                csv.ReadHeader();

                var csvRecords = csv.GetRecords<ThunderBallEntity>().ToList();
                return _mapper.Map<List<LotteryDto>>(csvRecords);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return null;
        }

    }
}
