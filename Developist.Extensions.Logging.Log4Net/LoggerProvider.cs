using Microsoft.Extensions.Logging;

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Developist.Extensions.Logging.Log4Net
{
    public sealed class LoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, LoggerAdapter> loggers = new ConcurrentDictionary<string, LoggerAdapter>();
        private readonly LoggerOptions options;
        private log4net.Repository.ILoggerRepository? loggerRepository;

        public LoggerProvider(LoggerOptions options)
        {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            InitializeLoggerRepository();
        }

        public ILogger CreateLogger(string categoryName)
        {
            if (categoryName is null)
            {
                throw new ArgumentNullException(nameof(categoryName));
            }

            if (categoryName.Trim().Length == 0)
            {
                throw new ArgumentException(
                    message: "Value cannot be " + (categoryName.Length == 0 ? "empty." : "all white space."),
                    paramName: nameof(categoryName));
            }

            return loggers.GetOrAdd(categoryName, CreateLogger);

            LoggerAdapter CreateLogger(string _)
               => new LoggerAdapter(log4net.LogManager.GetLogger(loggerRepository!.Name, categoryName), options);
        }

        public void Dispose() => loggers.Clear();

        private void InitializeLoggerRepository()
        {
            loggerRepository = log4net.LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            log4net.Config.XmlConfigurator.Configure(loggerRepository, ParseConfigurationFile(options));
        }

        private static XmlElement ParseConfigurationFile(LoggerOptions options)
        {
            var configurationDocument = new XmlDocument();
            using (var fileStream = File.OpenRead(options.ConfigurationFilePath))
            {
                configurationDocument.Load(fileStream);
                fileStream.Close();
            }
            return (XmlElement)configurationDocument.GetElementsByTagName(options.ConfigurationSectionName).Item(0);
        }
    }
}
