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
        private readonly Log4NetLoggerOptions options;

        public Log4NetLoggerProvider(Log4NetLoggerOptions options) => this.options = Ensure.Argument.NotNull(() => options);

        public ILogger CreateLogger(string categoryName)
            => loggers.GetOrAdd(categoryName, _ => new Log4NetLoggerAdapter(categoryName, ParseConfigurationFile(options), options));

        protected override void ReleaseManagedResources()
        {
            loggers.Clear();
            base.ReleaseManagedResources();
        }

        private static XmlElement ParseConfigurationFile(Log4NetLoggerOptions options)
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
