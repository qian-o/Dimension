using DimensionService.Dao.CallRoom;
using DimensionService.Dao.ChatColumn;
using DimensionService.Dao.ChatLink;
using DimensionService.Dao.ChatMessages;
using DimensionService.Dao.FriendInfo;
using DimensionService.Dao.LoginInfo;
using DimensionService.Dao.UserInfo;
using DimensionService.Filter;
using DimensionService.Hubs;
using DimensionService.Middleware;
using DimensionService.Service.Attachment;
using DimensionService.Service.Call;
using DimensionService.Service.Chat;
using DimensionService.Service.Hitokoto;
using DimensionService.Service.UserManager;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace DimensionService
{
    public static class Startup
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = long.MaxValue;
            });

            services.AddControllers(options =>
            {
                options.Filters.Add<WebApiActionFilter>();
                options.Filters.Add<WebApiExceptionFilter>();
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            services.AddSwaggerGen(c =>
            {
                AssemblyName assembly = Assembly.GetExecutingAssembly().GetName();
                c.SwaggerDoc(assembly.Name, new OpenApiInfo
                {
                    Title = assembly.Name,
                    Version = assembly.Version.ToString()
                });
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{assembly.Name}.xml"));
            });

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", p =>
                {
                    p.SetIsOriginAllowed(_ => true);
                    p.AllowAnyHeader();
                    p.AllowAnyMethod();
                    p.AllowCredentials();
                });
            });

            services.AddSignalR();

            services.AddScoped<IUserManagerService, UserManagerService>();
            services.AddScoped<IAttachmentService, AttachmentService>();
            services.AddScoped<IHitokotoService, HitokotoService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddScoped<ICallService, CallService>();

            services.AddScoped<IUserInfoDAO, UserInfoDAO>();
            services.AddScoped<ILoginInfoDAO, LoginInfoDAO>();
            services.AddScoped<IFriendInfoDAO, FriendInfoDAO>();
            services.AddScoped<IChatLinkDAO, ChatLinkDAO>();
            services.AddScoped<IChatColumnDAO, ChatColumnDAO>();
            services.AddScoped<IChatMessagesDAO, ChatMessagesDAO>();
            services.AddScoped<ICallRoomDAO, CallRoomDAO>();
        }

        public static void ConfigureApp(this WebApplication app)
        {
#if DEBUG
            AssemblyName assembly = Assembly.GetExecutingAssembly().GetName();
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint($"/swagger/{assembly.Name}/swagger.json", assembly.Name));
#else
            app.UseHttpsRedirection();
#endif

            app.UseCors("CorsPolicy");

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseMiddleware<SignalRQueryStringAuthMiddleware>();

            app.MapControllers();
            app.MapHub<InformHub>("/InformHub");
        }
    }
}
