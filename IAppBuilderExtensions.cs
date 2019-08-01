using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Owin;
using System.Collections.Generic;
using System.Linq;

namespace JohanB.AspNet.Diagnostics.HealthChecks
{
    public static class IAppBuilderExtensions
    {
        public static void UseHealthChecks(
            this IAppBuilder app,
            string url,
            HealthCheckOptions options,
            params IHealthCheck[] healthChecks)
        {
            // TODO will this work?
            var loggerFactory = new Microsoft.Extensions.Logging.LoggerFactory();
            var logger = loggerFactory.CreateLogger(nameof(HealthCheckMiddleware));

            app.Map(url, appBuilder =>
            {
                appBuilder.Use<HealthCheckMiddleware>(
                    logger,
                    options,
                    healthChecks);
            });
        }

        public static void UseHealthChecks(
            this IAppBuilder app,
            string url,
            params IHealthCheck[] healthChecks)
        {
            UseHealthChecks(app, url, new HealthCheckOptions(), healthChecks);
        }

        public static void UseHealthChecks(
            this IAppBuilder app,
            string url,
            HealthCheckOptions options,
            IEnumerable<IHealthCheck> healthChecks)
        {
            UseHealthChecks(app, url, options, healthChecks.ToArray());
        }

        public static void UseHealthChecks(
            this IAppBuilder app,
            string url,
            IEnumerable<IHealthCheck> healthChecks)
        {
            UseHealthChecks(app, url, new HealthCheckOptions(), healthChecks.ToArray());
        }
    }
}
