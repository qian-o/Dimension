using DimensionService.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Memory;
using System;
using System.IO;

namespace DimensionService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ClassHelper.UpdateHitokoto();
            Configuration.Default.MemoryAllocator = ArrayPoolMemoryAllocator.CreateWithAggressivePooling();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.ConfigureLogging((context, options) =>
                        {
                            options.AddLog4Net(Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "log4net.config"));
                        });
                        webBuilder.UseDefaultServiceProvider(options => options.ValidateScopes = false);
                        webBuilder.UseStartup<Startup>();
                    });
        }
    }
}
