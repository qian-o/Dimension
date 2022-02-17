using DimensionService;
using DimensionService.Common;

ClassHelper.UpdateHitokoto();
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureLogging((context, options) =>
{
    options.AddLog4Net(Path.Combine(AppContext.BaseDirectory, "log4net.config"));
});
builder.Services.ConfigureServices();

WebApplication app = builder.Build();
app.ConfigureApp();

app.Run();
