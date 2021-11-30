// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Utilities;

using Microsoft.Extensions.Logging;

using System;
using System.Reflection;
using System.Xml;

namespace Developist.Extensions.Logging.Log4Net
{
    public class Log4NetLoggerAdapter : ILogger
    {
        private readonly log4net.ILog logger;
        private readonly Log4NetLoggerOptions options;

        public Log4NetLoggerAdapter(string categoryName, XmlElement configurationElement, Log4NetLoggerOptions options)
        {
            Ensure.Argument.NotNullOrEmpty(() => categoryName);
            Ensure.Argument.NotNull(() => configurationElement);

            var loggerRepository = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            logger = log4net.LogManager.GetLogger(loggerRepository.Name, categoryName);
            log4net.Config.XmlConfigurator.Configure(loggerRepository, configurationElement);

            this.options = Ensure.Argument.NotNull(() => options);
        }

        public IDisposable BeginScope<TState>(TState state) => new Log4NetLogScope(state, options);

        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    return logger.IsDebugEnabled;
                case LogLevel.Information:
                    return logger.IsInfoEnabled;
                case LogLevel.Warning:
                    return logger.IsWarnEnabled;
                case LogLevel.Error:
                    return logger.IsErrorEnabled;
                case LogLevel.Critical:
                    return logger.IsFatalEnabled;
                case LogLevel.None:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel), $"Not a valid {nameof(LogLevel)} value.");
            }
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            Ensure.Argument.NotNull(() => formatter);

            var message = formatter(state, exception);
            if (string.IsNullOrEmpty(message) && exception is null)
            {
                return;
            }

            switch (logLevel)
            {
                case LogLevel.Trace:
                case LogLevel.Debug:
                    logger.Debug(message, exception);
                    break;
                case LogLevel.Information:
                    logger.Info(message, exception);
                    break;
                case LogLevel.Warning:
                    logger.Warn(message, exception);
                    break;
                case LogLevel.Error:
                    logger.Error(message, exception);
                    break;
                case LogLevel.Critical:
                    logger.Fatal(message, exception);
                    break;
            }
        }
    }
}
