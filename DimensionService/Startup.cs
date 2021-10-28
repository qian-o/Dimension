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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace DimensionService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = long.MaxValue;
            });

            services.AddControllers(options =>
            {
                options.Filters.Add<WebApiActionFilter>();
                options.Filters.Add<WebApiExceptionFilter>();
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DimensionService", Version = "v1" });

                // Set the comments path for the Swagger JSON and UI.
                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
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

            services.AddQueuePolicy(options =>
            {
                options.MaxConcurrentRequests = 500;
                options.RequestQueueLimit = 15000;
            });

            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new HeaderApiVersionReader("Service-Version");
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
#if DEBUG
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DimensionService v1"));
#else
            app.UseHttpsRedirection();
#endif

            app.UseCors("CorsPolicy");

            app.UseMiddleware<SignalRQueryStringAuthMiddleware>();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<InformHub>("/InformHub");
            });
        }
    }
}
