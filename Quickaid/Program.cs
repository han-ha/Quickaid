using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using Quickaid.Data;
using Quickaid.Services;
using Quickaid.Services.Interfaces;
using Quickaid.Mapping;
using Quickaid.Mapping.Interfaces;
using System.Text;
using Quickaid.Utils;
using Microsoft.OpenApi.Models;

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
            builder.Services.AddSwaggerGen(c =>
            {
                // podstawowe info o API
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Quickaid API",
                    Version = "v1"
                });

                // JWT w Swaggerze
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Wpisz: Bearer {token}",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        securityScheme,
                        Array.Empty<string>()
                    }
                });

                // XML comments (jeœli plik istnieje)
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                    c.IncludeXmlComments(xmlPath);
            });

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

            var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"] ?? "");

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

            // autoryzacja
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
            builder.Services.AddScoped<IInternalAedMapper, InternalAedMapper>();
            builder.Services.AddScoped<IExternalAedMapper, ExternalAedMapper>();
            builder.Services.AddScoped<IAedMergeMapper, AedMergeMapper>();
            builder.Services.AddScoped<IQuestionMapper, QuestionMapper>();
            builder.Services.AddScoped<IAnswerMapper, AnswerMapper>();
            builder.Services.AddScoped<IPasswordMapper, PasswordMapper>();

            builder.Services.AddScoped<AedGeoJsonUtils>();

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
