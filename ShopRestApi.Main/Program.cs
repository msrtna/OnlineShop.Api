
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using ShopRestApi.Api.Exceptions;
using ShopRestApi.Api.Filters;
using ShopRestApi.Api.Middleware;
using ShopRestApi.Application.Common.Settings;
using ShopRestApi.Application.Interfaces;
using ShopRestApi.Application.Mappings;
using ShopRestApi.Application.Repositories;
using ShopRestApi.Application.Validators.Products;
using ShopRestApi.Infrastructure.Identity;
using ShopRestApi.Infrastructure.Persistence;
using ShopRestApi.Infrastructure.Services;

namespace ShopRestApi.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
            });
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();
            builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
            builder.Services.AddValidatorsFromAssemblyContaining<CreateProductDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<UpdateProductDtoValidator>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(
                    "Bearer",
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                        Description = "Enter JWT Token"
                    });

                options.AddSecurityRequirement(
                    new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                    {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference =
                        new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type =
                                Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                },
                Array.Empty<string>()
            }
                    });
            });
            builder.Services.AddAutoMapper(typeof(ProductMappingProfile));

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services
                .AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
            var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
            builder.Services
                        .AddAuthentication(options =>
                        {
                            options.DefaultAuthenticateScheme =
                                JwtBearerDefaults.AuthenticationScheme;

                            options.DefaultChallengeScheme =
                                JwtBearerDefaults.AuthenticationScheme;
                        })
                        .AddJwtBearer(options =>
                        {
                            options.TokenValidationParameters =
                                new TokenValidationParameters
                                {
                                    ValidateIssuer = true,
                                    ValidateAudience = true,
                                    ValidateLifetime = true,
                                    ValidateIssuerSigningKey = true,

                                    ValidIssuer = jwtSettings!.Issuer,
                                    ValidAudience = jwtSettings.Audience,

                                    IssuerSigningKey =
                                        new SymmetricSecurityKey(
                                            Encoding.UTF8.GetBytes(
                                                jwtSettings.Key))
                                };
                        });

            builder.Host.UseSerilog((context, configuration) =>
            {
                configuration
                    .MinimumLevel.Information()
                    .WriteTo.Console()
                    .WriteTo.File(
                        path: "Logs/log-.txt",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7,
                        shared: true);
            });

            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var dbContext =
                    services.GetRequiredService<AppDbContext>();

                await dbContext.Database.MigrateAsync();

                var roleManager =
                    services.GetRequiredService<RoleManager<IdentityRole>>();

                await RoleSeeder.SeedAsync(roleManager);
            }
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.MapControllers();

            app.Run();
        }
    }
}
