// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Owin;

namespace Microsoft.AspNetCore.Diagnostics.HealthChecks
{
    public class HealthCheckMiddleware : OwinMiddleware
    {
        private readonly HealthCheckOptions _healthCheckOptions;
        private readonly HealthCheckService _healthCheckService;

        public HealthCheckMiddleware(
            OwinMiddleware next,
            HealthCheckOptions healthCheckOptions,
            HealthCheckService healthCheckService) : base(next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            _healthCheckOptions = healthCheckOptions ?? throw new ArgumentNullException(nameof(healthCheckOptions));
            _healthCheckService = healthCheckService ?? throw new ArgumentNullException(nameof(healthCheckService));
        }

        /// <summary>
        /// Processes a request.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public override async Task Invoke(IOwinContext owinContext)
        {
            if (owinContext == null)
            {
                throw new ArgumentNullException(nameof(owinContext));
            }

            // Get results
            var result = await _healthCheckService.CheckHealthAsync(_healthCheckOptions.Predicate, owinContext.Request.CallCancelled).ConfigureAwait(false);

            // Map status to response code - this is customizable via options. 
            if (!_healthCheckOptions.ResultStatusCodes.TryGetValue(result.Status, out var statusCode))
            {
                var message =
                    $"No status code mapping found for {nameof(HealthStatus)} value: {result.Status}." +
                    $"{nameof(HealthCheckOptions)}.{nameof(HealthCheckOptions.ResultStatusCodes)} must contain" +
                    $"an entry for {result.Status}.";

                throw new InvalidOperationException(message);
            }

            owinContext.Response.StatusCode = statusCode;

            if (!_healthCheckOptions.AllowCachingResponses)
            {
                // Similar to: https://github.com/aspnet/Security/blob/7b6c9cf0eeb149f2142dedd55a17430e7831ea99/src/Microsoft.AspNetCore.Authentication.Cookies/CookieAuthenticationHandler.cs#L377-L379
                var headers = owinContext.Response.Headers;
                headers.Set("Cache-Control", "no-store, no-cache");
                headers.Set("Pragma", "no-cache");
                headers.Set("Expires", "Thu, 01 Jan 1970 00:00:00 GMT");
            }

            if (_healthCheckOptions.ResponseWriter != null)
            {
                await _healthCheckOptions.ResponseWriter(owinContext, result).ConfigureAwait(false);
            }
        }

        private static IHealthCheck[] FilterHealthChecks(
            IReadOnlyDictionary<string, IHealthCheck> checks,
            ISet<string> names)
        {
            // If there are no filters then include all checks.
            if (names.Count == 0)
            {
                return checks.Values.ToArray();
            }

            // Keep track of what we don't find so we can report errors.
            var notFound = new HashSet<string>(names, StringComparer.OrdinalIgnoreCase);
            var matches = new List<IHealthCheck>();

            foreach (var kvp in checks)
            {
                if (!notFound.Remove(kvp.Key))
                {
                    // This check was excluded
                    continue;
                }

                matches.Add(kvp.Value);
            }

            if (notFound.Count > 0)
            {
                var message =
                    $"The following health checks were not found: '{string.Join(", ", notFound)}'. " +
                    $"Registered health checks: '{string.Join(", ", checks.Keys)}'.";
                throw new InvalidOperationException(message);
            }

            return matches.ToArray();
        }
    }
}