namespace MassTransit.Monitoring
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Transports;


    public class BusHealthCheck :
        IHealthCheck
    {
        readonly IBusInstance _busInstance;

        public BusHealthCheck(IBusInstance busInstance)
        {
            _busInstance = busInstance;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
        {
            var result = _busInstance.BusControl.CheckHealth();

            var data = new Dictionary<string, object>
            {
                ["Endpoints"] = new EndpointDictionary(result.Endpoints.ToDictionary(x => x.Key,
                    x => new Endpoint(Enum.GetName(typeof(BusHealthStatus), x.Value.Status), x.Value.Description)
                ))
            };

            return Task.FromResult(result.Status switch
            {
                BusHealthStatus.Healthy => HealthCheckResult.Healthy(result.Description, data),
                BusHealthStatus.Degraded => HealthCheckResult.Degraded(result.Description, result.Exception, data),
                _ => HealthCheckResult.Unhealthy(result.Description, result.Exception, data)
            });
        }


        class EndpointDictionary :
            Dictionary<string, Endpoint>
        {
            public EndpointDictionary(IDictionary<string, Endpoint> dictionary)
                : base(dictionary, StringComparer.OrdinalIgnoreCase)
            {
            }

            public override string ToString()
            {
                return string.Join(", ", this.Select(x => $"{x.Key}: {x.Value}"));
            }
        }


        class Endpoint
        {
            public Endpoint(string status, string description)
            {
                Status = status;
                Description = description;
            }

            string Status { get; }
            string Description { get; }

            public override string ToString()
            {
                return $"{Status} - {Description}";
            }
        }
    }
}
