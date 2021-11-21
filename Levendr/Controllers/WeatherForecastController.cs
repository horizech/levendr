﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Levendr.Services;
using Levendr.Models;
using Levendr.Enums;

namespace Levendr.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            // PostgresqlDatabase
            //     .getInstance()
            //     .createDBIfNotExist("postgres");

            List<ColumnInfo> columns = new List<ColumnInfo>();
            columns.Add(new ColumnInfo() { Name = "name", Datatype = ColumnDataType.ShortText, IsRequired = true });
            columns.Add(new ColumnInfo() { Name = "title", Datatype = ColumnDataType.ShortText, IsRequired = true });

            ServiceManager
                .Instance
                .GetService<DatabaseService>()
                .GetDatabaseDriver()
                .CreateTable("public", "school", columns);

            ServiceManager
                .Instance
                .GetService<DatabaseService>()
                .GetDatabaseDriver()
                .GetTableColumns("public", "school");

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.UtcNow.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
