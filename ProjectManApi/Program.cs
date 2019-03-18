using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using NLog.Extensions.Logging;

namespace PM.Api
{
    /// <summary>
    /// Main Program console for the Web Api
    /// </summary>
    public class Program
    {

        /// <summary>
        /// Main method for the .net core Api to start
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// Host builder method
        /// </summary>
        /// <param name="args">cmd args</param>
        /// <returns>IWebHostBuilder</returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
            ;
    }
}
