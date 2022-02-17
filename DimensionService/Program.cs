using DimensionService.Common;

namespace DimensionService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ClassHelper.UpdateHitokoto();

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
