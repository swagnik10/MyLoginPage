using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MyApp.Infrastructure;
using MyApp.Infrastructure.Security;
using MyApp.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();
//jwt authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),

        ClockSkew = TimeSpan.Zero // important for dev clarity
    };
});


// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Dependency Injection
builder.Services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<NHibernate.ISession>(sp =>
{
    return NHibernateHelper.SessionFactory.OpenSession();
});
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCorsPolicy", policy =>
    {
        policy
            .WithOrigins("http://127.0.0.1:5801")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();

// Swagger (Development only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

//from postman - pass X-User-Id in header
/*app.Use(async (context, next) =>
{
    // DEV ONLY – REMOVE AFTER JWT
    if (context.Request.Headers.TryGetValue("X-User-Id", out var userId))
    {
        context.Items["UserId"] = int.Parse(userId!);
    }

    await next();
});*/

app.UseCors("DevCorsPolicy");

app.MapControllers();

// NHibernate configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
NHibernateHelper.Configure(connectionString);

app.Run();

