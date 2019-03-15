using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace PM.Models.ViewModels
{
    public class AppLogs
    {
        public LogLevel LogType { get; set; }
        public string AppName { get; set; }
        public string Module { get; set; }
        public string Method { get; set; }
        public string Message { get; set; }
        public string ErrorDetails { get; set; }
    }
}
