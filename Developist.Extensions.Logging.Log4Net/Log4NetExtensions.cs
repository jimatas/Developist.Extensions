// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

using System;

namespace Developist.Extensions.Logging.Log4Net
{
    public static class Log4NetExtensions
    {
        public static ILoggingBuilder AddLog4Net(this ILoggingBuilder loggingBuilder) => loggingBuilder.AddLog4Net(_ => { });
        public static ILoggingBuilder AddLog4Net(this ILoggingBuilder loggingBuilder, Action<Log4NetLoggerOptions> configureOptions)
        {
            var options = new Log4NetLoggerOptions();
            configureOptions(options);

            loggingBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, Log4NetLoggerProvider>(_ => new Log4NetLoggerProvider(options)));
            return loggingBuilder;
        }

        public static ILoggerFactory AddLog4Net(this ILoggerFactory loggerFactory) => loggerFactory.AddLog4Net(_ => { });
        public static ILoggerFactory AddLog4Net(this ILoggerFactory loggerFactory, Action<Log4NetLoggerOptions> configureOptions)
        {
            var options = new Log4NetLoggerOptions();
            configureOptions(options);

            loggerFactory.AddProvider(new Log4NetLoggerProvider(options));
            return loggerFactory;
        }
    }
}
