using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using VacationRental.Application.Abstractions;
using VacationRental.Infrastructure;
using Xunit;

namespace VacationRental.Api.Tests
{
    [CollectionDefinition("Integration")]
    public sealed class IntegrationFixture : IDisposable, ICollectionFixture<IntegrationFixture>
    {
        private readonly TestServer _server;

        public HttpClient Client { get; }

        //public IUnitRepository UnitRepository { get; } = new UnitInMemoryRepository();

        public IntegrationFixture()
        {
            var builder = new WebHostBuilder().UseStartup<Startup>();
            //Inject services here
            
            _server = new TestServer(builder);

            Client = _server.CreateClient();
        }

        public void Dispose()
        {
            Client.Dispose();
            _server.Dispose();
        }
    }
}
