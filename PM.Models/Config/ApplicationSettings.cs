using System;
using System.Collections.Generic;
using System.Text;

namespace PM.Models.Config
{
    public class ApplicationSettings
    {
        public IReadOnlyCollection<ConnectionSettings> ConnectionStrings { get; set; }
        public IReadOnlyCollection<LoggingConfig> LogSettings { get; set; }
    }

    public class ConnectionSettings
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class LoggingConfig
    {
        public string LoggerName { get; set; }
        public string LogFile { get; set; }
        public string LogLayout { get; set; }
    }

}
