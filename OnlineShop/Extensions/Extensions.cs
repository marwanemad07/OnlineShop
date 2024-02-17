using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace OnlineShop.Extensions
{
    public static class Extensions
    {
        public static void AddDbContext(IServiceCollection services, ConfigurationManager cfg)
        {
            var connectionString = cfg.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
        }
        public static void AddIdentity(IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>(options => {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        }
        public static void ConfigureIdentityOptions(IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options => {
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
            });
        }

        public static void InjectServices(IServiceCollection services)
        {

        }
        public static void InjectReposotries(IServiceCollection services)
        {

        }

        public static void AddCorsPolicy(IServiceCollection services)
        {
            services.AddCors(options => {
                options.AddDefaultPolicy(options => {
                    options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
                options.AddPolicy("ApiPolicy", options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            });
        }

        public static void AddAuthentication(IServiceCollection services, ConfigurationManager cfg)
        {
            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options => {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidIssuer = cfg["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = cfg["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                                                        .GetBytes(cfg["JWT:Key"]))
                };
            });
        }

        public static void AddSwaggerGen(IServiceCollection services)
        {
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new() { Title = "3TierArchAPIs", Version = "v1" });

                // Configure JWT Bearer authentication
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                    Description = "JWT Authorization header using the Bearer scheme",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });

                // Add the security requirement for JWT Bearer token
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        Array.Empty<string>()
                    }
                });
            });
        }
    }
}
