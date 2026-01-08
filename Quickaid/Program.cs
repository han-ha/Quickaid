using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Quickaid.Data;
using Quickaid.Services;
using Quickaid.Services.Interfaces;
using Quickaid.Mapping;
using Quickaid.Mapping.Interfaces;
using System.Text;

namespace Quickaid
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // baza danych
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure()
                ));

            // kontrolery
            builder.Services.AddControllers();
            builder.Services.AddRouting(options => options.LowercaseUrls = true);

            // Swagger (testy)
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // CORS (dla testów w Swaggerze)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // klucz JWT z konfiguracji TODO daæ lepszy default
            var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"] ?? "super_secret_dev_key");

            // konfiguracja uwierzytelniania JWT
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Quickaid API", Version = "v1" });

                var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter JWT Bearer token **_only_**",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme
                    }
                };

                c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        securityScheme,
                        Array.Empty<string>()
                    }
                });
            });


            // konfiguracja autoryzacji
            builder.Services.AddAuthorization();

            // rejestracja serwisów aplikacyjnych
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IArticleService, ArticleService>();
            builder.Services.AddScoped<IQuizService, QuizService>();
            builder.Services.AddScoped<IResultService, ResultService>();
            builder.Services.AddScoped<IAedService, AedService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IQuestionService, QuestionService>();
            builder.Services.AddScoped<IAnswerService, AnswerService>();
            builder.Services.AddScoped<IQuizSolverService, QuizSolverService>();

            // rejestracja mapperów
            builder.Services.AddScoped<IUserMapper, UserMapper>();
            builder.Services.AddScoped<IArticleMapper, ArticleMapper>();
            builder.Services.AddScoped<IQuizMapper, QuizMapper>();
            builder.Services.AddScoped<IResultMapper, ResultMapper>();
            builder.Services.AddScoped<IAedMapper, AedMapper>();
            builder.Services.AddScoped<IQuestionMapper, QuestionMapper>();
            builder.Services.AddScoped<IAnswerMapper, AnswerMapper>();
            builder.Services.AddScoped<IPasswordMapper, PasswordMapper>();

            var app = builder.Build();

            // db warmup
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.CanConnect();
            }

            // Swagger UI (testy)
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // middleware
            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
