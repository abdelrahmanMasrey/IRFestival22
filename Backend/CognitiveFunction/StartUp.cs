using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
[assembly: FunctionsStartup(typeof(CognitiveFunction.StartUp))]
namespace CognitiveFunction
{
    internal class StartUp : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            FunctionsHostBuilderContext context = builder.GetContext();
            builder.Services.AddSingleton(p=> new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(context.Configuration.GetValue<string>("ComputerVisionApiKey")))
                {
            //    EndPoint = context.Configuration.GetValue<string>("ComputerVisionEndPoint")
            });
        }
    }
}
