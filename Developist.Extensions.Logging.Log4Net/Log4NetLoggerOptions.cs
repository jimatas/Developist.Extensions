// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

namespace Developist.Extensions.Logging.Log4Net
{
    public class Log4NetLoggerOptions
    {
        public string DefaultScopeName = "scope";
        public string ConfigurationFilePath { get; set; } = "log4net.config";
        public string ConfigurationSectionName { get; set; } = "log4net";
    }
}
