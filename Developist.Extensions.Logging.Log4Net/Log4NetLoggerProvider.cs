// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Utilities;

using Microsoft.Extensions.Logging;

using System.Collections.Concurrent;
using System.IO;
using System.Xml;

namespace Developist.Extensions.Logging.Log4Net
{
    public class Log4NetLoggerProvider : DisposableBase, ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, Log4NetLoggerAdapter> loggers = new ConcurrentDictionary<string, Log4NetLoggerAdapter>();
        private readonly string configurationFile;
        private readonly string configurationSectionName;

        public Log4NetLoggerProvider(string configurationFile, string configurationSectionName = "log4net")
        {
            this.configurationFile = Ensure.Argument.NotNullOrEmpty(() => configurationFile);
            this.configurationSectionName = configurationSectionName;
        }

        public ILogger CreateLogger(string categoryName)
            => loggers.GetOrAdd(categoryName, _ => new Log4NetLoggerAdapter(categoryName, ParseConfigurationFile(configurationFile, configurationSectionName)));

        protected override void ReleaseManagedResources()
        {
            loggers.Clear();
            base.ReleaseManagedResources();
        }

        private static XmlElement ParseConfigurationFile(string configurationFile, string configurationSectionName)
        {
            var configurationDocument = new XmlDocument();
            using (var fileStream = File.OpenRead(configurationFile))
            {
                configurationDocument.Load(fileStream);
                fileStream.Close();
            }
            return (XmlElement)configurationDocument.GetElementsByTagName(configurationSectionName).Item(0);
        }
    }
}
