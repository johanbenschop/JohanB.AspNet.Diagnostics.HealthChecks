﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Owin;

namespace Microsoft.AspNetCore.Diagnostics.HealthChecks
{
    /// <summary>
    /// Contains options for the <see cref="HealthCheckMiddleware"/>.
    /// </summary>
    public class HealthCheckOptions
    {
        /// <summary>
        /// Gets or sets a predicate that is used to filter the set of health checks executed.
        /// </summary>
        /// <remarks>
        /// If <see cref="Predicate"/> is <c>null</c>, the <see cref="HealthCheckMiddleware"/> will run all
        /// registered health checks - this is the default behavior. To run a subset of health checks,
        /// provide a function that filters the set of checks.
        /// </remarks>
        public Func<HealthCheckRegistration, bool> Predicate { get; set; }

        /// <summary>
        /// Gets a dictionary mapping the <see cref="HealthStatus"/> to an HTTP status code applied to the response.
        /// This property can be used to configure the status codes returned for each status.
        /// </summary>
        public IDictionary<HealthStatus, int> ResultStatusCodes { get; } = new Dictionary<HealthStatus, int>()
        {
            { HealthStatus.Healthy, 200 },
            { HealthStatus.Degraded, 200 },
            { HealthStatus.Unhealthy, 503 },
        };

        /// <summary>
        /// Gets or sets a delegate used to write the response.
        /// </summary>
        /// <remarks>
        /// The default value is a delegate that will write a minimal <c>text/plain</c> response with the value
        /// of <see cref="HealthReport.Status"/> as a string.
        /// </remarks>
        public Func<IOwinContext, HealthReport, Task> ResponseWriter { get; set; } = HealthCheckResponseWriters.WriteMinimalPlaintext;

        /// <summary>
        /// Gets or sets a value that controls whether responses from the health check middleware can be cached.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The health check middleware does not perform caching of any kind. This setting configures whether
        /// the middleware will apply headers to the HTTP response that instruct clients to avoid caching.
        /// </para>
        /// <para>
        /// If the value is <c>false</c> the health check middleware will set or override the 
        /// <c>Cache-Control</c>, <c>Expires</c>, and <c>Pragma</c> headers to prevent response caching. If the value 
        /// is <c>true</c> the health check middleware will not modify the cache headers of the response.
        /// </para>
        /// </remarks>
        public bool AllowCachingResponses { get; set; }
    }
}