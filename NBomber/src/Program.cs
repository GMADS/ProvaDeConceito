using System;
using NBomber.CSharp;
using NBomber.Plugins.Http.CSharp;
using NBomber.Plugins.Network.Ping;

namespace NBomberTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var step = Step.Create("fetch_html_page", 
                                   clientFactory: HttpClientFactory.Create(), 
                                   execute: context =>
            {
                var request = Http.CreateRequest("GET", "https://nbomber.com")
                                  .WithHeader("Accept", "text/html");

                return Http.Send(request, context);
            });

            var scenario = ScenarioBuilder
                .CreateScenario("simple_http", step)
                .WithWarmUpDuration(TimeSpan.FromSeconds(5))
                .WithLoadSimulations(
                    Simulation.InjectPerSec(rate: 100, during: TimeSpan.FromSeconds(30))
                );

            // creates ping plugin that brings additional reporting data
            var pingPluginConfig = PingPluginConfig.CreateDefault(new[] {"nbomber.com"});
            var pingPlugin = new PingPlugin(pingPluginConfig);                

            NBomberRunner
                .RegisterScenarios(scenario)
                .WithWorkerPlugins(pingPlugin)
                .Run();

        }
    }
}
