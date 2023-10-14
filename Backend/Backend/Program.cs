
using Backend.Database;
using Backend.Models;
using Backend.Models.EmailSettings;
using Backend.Models.GoogleSettings;
using Backend.Models.Options;
using Backend.Services.Interfaces;
using Backend.Services;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Backend.Repositories.Interfaces;
using Backend.Repositories;

namespace Backend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddOptions<JwtTokensOptions>()
            .BindConfiguration(nameof(JwtTokensOptions))
            .ValidateDataAnnotations();

            //CREDENTIALS JSON config
            builder.Configuration.AddJsonFile("credentials.json", optional: false, reloadOnChange: true);
            builder.Services.Configure<EmailSettingsDto>(builder.Configuration.GetSection("EmailSettingsDto"));

            //GOOGLE CREDENTIALS
            builder.Services.Configure<GoogleCredentials>(builder.Configuration.GetSection("GoogleCredentials"));

            //DATABASE CONNECTION STRING
            string connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("No connectionString");
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            //ADD IDENTITY
            builder.Services.AddIdentityCore<User>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
           .AddRoles<IdentityRole>()
           .AddEntityFrameworkStores<ApplicationDbContext>()
           .AddDefaultTokenProviders(); // Ezzel regisztrálod a token szolgáltatókat


            builder.Services.AddMemoryCache();
            //PASSWORD HASH SETTING
            builder.Services.Configure<PasswordHasherOptions>(opt => opt.IterationCount = 210_000);


            //ADD SERVICES
            builder.Services.AddScoped<ITokenCreationService, JwtService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ISubscribtionService, SubscribtionService>();
            builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            builder.Services.AddScoped<IArticleService, ArticleService>();
            builder.Services.AddScoped<IArticleRepository, ArticleRepository>();        
            builder.Services.AddTransient<IEmailService, EmailService>();
            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<ICommentRepository, CommentRepository>();

            //JWT
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters()
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidAudience = builder.Configuration["JwtTokensOptions:AccessTokenOptions:Audience"],
                            ValidIssuer = builder.Configuration["JwtTokensOptions:AccessTokenOptions:Issuer"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtTokensOptions:AccessTokenOptions:Key"] ?? string.Empty))
                        };
                    });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            //SWAGGER
            builder.Services.AddSwaggerGen(options =>
            {
                    // https://dev.to/eduardstefanescu/aspnet-core-swagger-documentation-with-bearer-authentication-40l6
                    options.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer"
                    });
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme
                                    {
                                        Name = "Bearer",
                                        In = ParameterLocation.Header,
                                        Reference = new OpenApiReference
                                        {
                                            Id = "Bearer",
                                            Type = ReferenceType.SecurityScheme
                                        }
                                    },
                                new List<string>()
                            }
                    });
            });

            //CLOUDINARY
            builder.Services.AddSingleton(s =>
            new Cloudinary(new Account(
                builder.Configuration.GetValue<string>("CloudinaryConfig:cloud_name"),
                builder.Configuration.GetValue<string>("CloudinaryConfig:api_key"),
                builder.Configuration.GetValue<string>("CloudinaryConfig:api_secret"))));

            //CORS
            builder.Services.AddCors(options => options.AddDefaultPolicy(
                configurePolicy => configurePolicy
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                // or specify directly which origins are allowed
                //.WithOrigins("http://localhost:4200/")
                ));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseAuthorization();


            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            using (var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>())
            {
                await context.Database.MigrateAsync();
            }
            //ROLES
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var roleExist = await roleManager.RoleExistsAsync("Admin");
                if (!roleExist)
                {
                    _ = await roleManager.CreateAsync(new IdentityRole("Admin"));
                }
                roleExist = await roleManager.RoleExistsAsync("User");
                if (!roleExist)
                {
                    _ = await roleManager.CreateAsync(new IdentityRole("User"));
                }
                roleExist = await roleManager.RoleExistsAsync("Journalist");
                if (!roleExist)
                {
                    _ = await roleManager.CreateAsync(new IdentityRole("Journalist"));
                }
            }

            app.Run();
        }
    }
}