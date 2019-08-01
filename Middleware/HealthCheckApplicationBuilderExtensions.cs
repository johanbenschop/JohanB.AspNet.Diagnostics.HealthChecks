// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.Owin;
using Owin;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// <see cref="IAppBuilder"/> extension methods for the <see cref="HealthCheckMiddleware"/>.
    /// </summary>
    public static class HealthCheckApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds a middleware that provides health check status.
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/>.</param>
        /// <param name="path">The path on which to provide health check status.</param>
        /// <returns>A reference to the <paramref name="app"/> after the operation has completed.</returns>
        /// <remarks>
        /// <para>
        /// If <paramref name="path"/> is set to <c>null</c> or the empty string then the health check middleware
        /// will ignore the URL path and process all requests. If <paramref name="path"/> is set to a non-empty
        /// value, the health check middleware will process requests with a URL that matches the provided value
        /// of <paramref name="path"/> case-insensitively, allowing for an extra trailing slash ('/') character.
        /// </para>
        /// <para>
        /// The health check middleware will use default settings from <see cref="IOptions{HealthCheckOptions}"/>.
        /// </para>
        /// </remarks>
        public static IAppBuilder UseHealthChecks(this IAppBuilder app, PathString path)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            UseHealthChecksCore(app, path, port: null, Array.Empty<object>());
            return app;
        }

        /// <summary>
        /// Adds a middleware that provides health check status.
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/>.</param>
        /// <param name="path">The path on which to provide health check status.</param>
        /// <param name="options">A <see cref="HealthCheckOptions"/> used to configure the middleware.</param>
        /// <returns>A reference to the <paramref name="app"/> after the operation has completed.</returns>
        /// <remarks>
        /// <para>
        /// If <paramref name="path"/> is set to <c>null</c> or the empty string then the health check middleware
        /// will ignore the URL path and process all requests. If <paramref name="path"/> is set to a non-empty
        /// value, the health check middleware will process requests with a URL that matches the provided value
        /// of <paramref name="path"/> case-insensitively, allowing for an extra trailing slash ('/') character.
        /// </para>
        /// </remarks>
        public static IAppBuilder UseHealthChecks(this IAppBuilder app, PathString path, HealthCheckOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            UseHealthChecksCore(app, path, port: null, new[] { Options.Create(options), });
            return app;
        }

        /// <summary>
        /// Adds a middleware that provides health check status.
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/>.</param>
        /// <param name="path">The path on which to provide health check status.</param>
        /// <param name="port">The port to listen on. Must be a local port on which the server is listening.</param>
        /// <returns>A reference to the <paramref name="app"/> after the operation has completed.</returns>
        /// <remarks>
        /// <para>
        /// If <paramref name="path"/> is set to <c>null</c> or the empty string then the health check middleware
        /// will ignore the URL path and process all requests on the specified port. If <paramref name="path"/> is
        /// set to a non-empty value, the health check middleware will process requests with a URL that matches the 
        /// provided value of <paramref name="path"/> case-insensitively, allowing for an extra trailing slash ('/') 
        /// character.
        /// </para>
        /// <para>
        /// The health check middleware will use default settings from <see cref="IOptions{HealthCheckOptions}"/>.
        /// </para>
        /// </remarks>
        public static IAppBuilder UseHealthChecks(this IAppBuilder app, PathString path, int port)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            UseHealthChecksCore(app, path, port, Array.Empty<object>());
            return app;
        }

        /// <summary>
        /// Adds a middleware that provides health check status.
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/>.</param>
        /// <param name="path">The path on which to provide health check status.</param>
        /// <param name="port">The port to listen on. Must be a local port on which the server is listening.</param>
        /// <returns>A reference to the <paramref name="app"/> after the operation has completed.</returns>
        /// <remarks>
        /// <para>
        /// If <paramref name="path"/> is set to <c>null</c> or the empty string then the health check middleware
        /// will ignore the URL path and process all requests on the specified port. If <paramref name="path"/> is
        /// set to a non-empty value, the health check middleware will process requests with a URL that matches the 
        /// provided value of <paramref name="path"/> case-insensitively, allowing for an extra trailing slash ('/') 
        /// character.
        /// </para>
        /// <para>
        /// The health check middleware will use default settings from <see cref="IOptions{HealthCheckOptions}"/>.
        /// </para>
        /// </remarks>
        public static IAppBuilder UseHealthChecks(this IAppBuilder app, PathString path, string port)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (port == null)
            {
                throw new ArgumentNullException(nameof(port));
            }

            if (!int.TryParse(port, out var portAsInt))
            {
                throw new ArgumentException("The port must be a valid integer.", nameof(port));
            }

            UseHealthChecksCore(app, path, portAsInt, Array.Empty<object>());
            return app;
        }

        /// <summary>
        /// Adds a middleware that provides health check status.
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/>.</param>
        /// <param name="path">The path on which to provide health check status.</param>
        /// <param name="port">The port to listen on. Must be a local port on which the server is listening.</param>
        /// <param name="options">A <see cref="HealthCheckOptions"/> used to configure the middleware.</param>
        /// <returns>A reference to the <paramref name="app"/> after the operation has completed.</returns>
        /// <remarks>
        /// <para>
        /// If <paramref name="path"/> is set to <c>null</c> or the empty string then the health check middleware
        /// will ignore the URL path and process all requests on the specified port. If <paramref name="path"/> is
        /// set to a non-empty value, the health check middleware will process requests with a URL that matches the 
        /// provided value of <paramref name="path"/> case-insensitively, allowing for an extra trailing slash ('/') 
        /// character.
        /// </para>
        /// </remarks>
        public static IAppBuilder UseHealthChecks(this IAppBuilder app, PathString path, int port, HealthCheckOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            UseHealthChecksCore(app, path, port, new[] { Options.Create(options), });
            return app;
        }

        /// <summary>
        /// Adds a middleware that provides health check status.
        /// </summary>
        /// <param name="app">The <see cref="IAppBuilder"/>.</param>
        /// <param name="path">The path on which to provide health check status.</param>
        /// <param name="port">The port to listen on. Must be a local port on which the server is listening.</param>
        /// <param name="options">A <see cref="HealthCheckOptions"/> used to configure the middleware.</param>
        /// <returns>A reference to the <paramref name="app"/> after the operation has completed.</returns>
        /// <remarks>
        /// <para>
        /// If <paramref name="path"/> is set to <c>null</c> or the empty string then the health check middleware
        /// will ignore the URL path and process all requests on the specified port. If <paramref name="path"/> is
        /// set to a non-empty value, the health check middleware will process requests with a URL that matches the 
        /// provided value of <paramref name="path"/> case-insensitively, allowing for an extra trailing slash ('/') 
        /// character.
        /// </para>
        /// </remarks>
        public static IAppBuilder UseHealthChecks(this IAppBuilder app, PathString path, string port, HealthCheckOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (!int.TryParse(port, out var portAsInt))
            {
                throw new ArgumentException("The port must be a valid integer.", nameof(port));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            UseHealthChecksCore(app, path, portAsInt, new[] { Options.Create(options), });
            return app;
        }

        private static void UseHealthChecksCore(IAppBuilder app, PathString path, int? port, object[] args)
        {
            // NOTE: we explicitly don't use Map here because it's really common for multiple health
            // check middleware to overlap in paths. Ex: `/health`, `/health/detailed` - this is order
            // sensititive with Map, and it's really surprising to people.
            //
            // See:
            // https://github.com/aspnet/Diagnostics/issues/511
            // https://github.com/aspnet/Diagnostics/issues/512
            // https://github.com/aspnet/Diagnostics/issues/514

            bool predicate(IOwinContext c)
            {
                return

                    // Process the port if we have one
                    (port == null || c.Request.LocalPort == port) &&

                    // We allow you to listen on all URLs by providing the empty PathString.
                    (!path.HasValue ||

                        // If you do provide a PathString, want to handle all of the special cases that 
                        // StartsWithSegments handles, but we also want it to have exact match semantics.
                        //
                        // Ex: /Foo/ == /Foo (true)
                        // Ex: /Foo/Bar == /Foo (false)
                        (c.Request.Path.StartsWithSegments(path, out var remaining) &&
                        string.IsNullOrEmpty(remaining.Value)));
            }

            app.MapWhen(predicate, b => b.Use<HealthCheckMiddleware>(args));
        }
    }
}