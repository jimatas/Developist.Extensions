namespace Developist.Extensions.Logging.Log4Net
{
    public sealed class LoggerOptions
    {
        public string ConfigurationFilePath { get; set; } = "log4net.config";
        public string ConfigurationSectionName { get; set; } = "log4net";
        public string DefaultScopeName { get; set; } = "scope";
    }
}
