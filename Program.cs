using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Reflection;
using WebApplication1.Application.Authorization;
using WebApplication1.Application.Interfaces;
using WebApplication1.Application.Services;
using WebApplication1.Infrastructure.Data;
using FluentValidation;
using FluentValidation.AspNetCore;
using Serilog;
using WebApplication1.Shared.Middlewares;
using WebApplication1.Shared.Filters;
using WebApplication1.Application.Validators;
using WebApplication1.Infrastructure.Services;
using WebApplication1.Domain.Entities;

namespace WebApplication1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString"));
            });


            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Adding Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // Adding Jwt Bearer
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:ValidAudience"],
                    ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
                };
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync("{\"message\": \"You are not authorized.\"}");
                    },
                    OnForbidden = async context =>
                    {
                        context.Response.StatusCode = 403;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync("{\"message\": \"You do not have permission to access this resource.\"}");
                    }
                };
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("IsGroupOwner", policy =>
                    policy.Requirements.Add(new IsGroupOwnerRequirement()));
                options.AddPolicy("IsInvitedUser", policy =>
                    policy.Requirements.Add(new IsInvitedUserRequirement()));
            });

            builder.Services.AddScoped<IAuthorizationHandler, IsGroupOwnerHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, IsInvitedUserHandler>();

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            builder.Services.AddScoped<IUserProfileService, UserProfileService>();
            builder.Services.AddScoped<IApplicationUserService, ApplicationUserService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<IGroupService, GroupService>();
            builder.Services.AddScoped<IGroupMemberService, GroupMemberService>();
            builder.Services.AddScoped<IReminderService, ReminderService>();
            builder.Services.AddScoped<IUserEventStatusService, UserEventStatusService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IExcelService, ExcelService>();
            builder.Services.AddScoped<IGroupInvitationService, GroupInvitationService>(); // Add this line
            builder.Services.AddMemoryCache();
            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // Manual FluentValidation registration
            builder.Services.AddScoped<UserExistsValidator>();
            builder.Services.AddScoped<CreateUserEventStatusDtoValidator>();
            builder.Services.AddScoped<UpdateUserEventStatusDtoValidator>();
            builder.Services.AddScoped<CreateReminderDtoValidator>();
            builder.Services.AddScoped<UpdateReminderDtoValidator>();
            builder.Services.AddScoped<CreateGroupMemberDtoValidator>();
            builder.Services.AddScoped<UpdateGroupMemberDtoValidator>();

            

            builder.Services.AddControllers(options => options.Filters.Add<ValidationFilter>());

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configure Serilog
            builder.Host.UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration)
                    .WriteTo.Console()
                    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day));

            var app = builder.Build();

            // Seed the database
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    await DbInitializer.Initialize(services);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            // Use Serilog for request logging
            app.UseSerilogRequestLogging();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseCors();

            app.UseWebSockets();

            

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        var buffer = new byte[1024 * 4];
                        var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), context.RequestAborted);

                        while (!receiveResult.CloseStatus.HasValue)
                        {
                            // Echo the message back
                            await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, receiveResult.Count), receiveResult.MessageType, receiveResult.EndOfMessage, context.RequestAborted);
                            receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), context.RequestAborted);
                        }

                        await webSocket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, context.RequestAborted);
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    }
                }
                else
                {
                    await next();
                }
            });

            app.UseGlobalExceptionHandling();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}