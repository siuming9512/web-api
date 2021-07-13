using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitMq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebApiTest.Controllers
{
    [ApiController]
    [Route("test")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly RabbitMqConnection _rabbitMq;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, RabbitMqConnection rabbitMq)
        {
            _logger = logger;
            _rabbitMq = rabbitMq;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("publish/{id}")]
        public ActionResult Publish(int id)
        {
            using(var channel = _rabbitMq.GetConnection().CreateModel())
            {
                var body = new
                {
                    id
                };

                var bodyBytes = JsonSerializer.SerializeToUtf8Bytes(body);
                channel.QueueDeclare("publish-test", true, false, false, null);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;
                channel.BasicPublish("", "publish-test", false, properties, bodyBytes);
            }

            return NoContent();
        }

    }
}
