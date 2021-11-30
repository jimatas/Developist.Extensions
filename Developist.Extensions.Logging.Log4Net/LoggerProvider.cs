// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Utilities;

using Microsoft.Extensions.Logging;

using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Developist.Extensions.Logging.Log4Net
{
    public class LoggerProvider : DisposableBase, ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, LoggerAdapter> loggers = new ConcurrentDictionary<string, LoggerAdapter>();
        private readonly LoggerOptions options;
        private log4net.Repository.ILoggerRepository loggerRepository;

        public LoggerProvider(LoggerOptions options)
        {
            this.options = Ensure.Argument.NotNull(() => options);
            InitializeLoggerRepository();
        }

        public ILogger CreateLogger(string categoryName)
        {
            Ensure.Argument.NotNullOrEmpty(() => categoryName);

            return loggers.GetOrAdd(categoryName, CreateLogger);

            LoggerAdapter CreateLogger(string _)
               => new LoggerAdapter(log4net.LogManager.GetLogger(loggerRepository.Name, categoryName), options);
        }

        protected override void ReleaseManagedResources()
        {
            loggers.Clear();
            base.ReleaseManagedResources();
        }

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
