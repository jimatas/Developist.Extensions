// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Developist.Extensions.Logging.Log4Net
{
    public static class Log4NetExtensions
    {
        public static ILoggingBuilder AddLog4Net(this ILoggingBuilder loggingBuilder, string configurationFile = "log4net.config", string configurationSectionName = "log4net")
        {
            loggingBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, Log4NetLoggerProvider>(_ => new Log4NetLoggerProvider(configurationFile, configurationSectionName)));
            return loggingBuilder;
        }

        public static ILoggerFactory AddLog4Net(this ILoggerFactory loggerFactory, string configurationFile = "log4net.config", string configurationSectionName = "log4net")
        {
            loggerFactory.AddProvider(new Log4NetLoggerProvider(configurationFile, configurationSectionName));
            return loggerFactory;
        }
    }
}
