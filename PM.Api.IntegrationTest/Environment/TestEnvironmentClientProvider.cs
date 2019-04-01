using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Net.Http;
using Microsoft.AspNetCore;
using PM.Api;

namespace PM.Api.IntegrationTest.Environment
{
    public class TestEnvironmentClientProvider: TestWebApplicationFactory<Startup>
    {
        public HttpClient Client { get; private set; }

        public TestEnvironmentClientProvider(TestWebApplicationFactory<Startup> factory)
        {
            Client = factory.CreateClient();
        }

        //protected override void Dispose(bool disposing)
        //{
        //    base.Dispose(disposing);
        //    Client.Dispose();
        //}
    }
}
