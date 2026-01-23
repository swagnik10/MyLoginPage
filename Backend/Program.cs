using MyApp.Infrastructure;
using MyApp.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

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

app.UseAuthorization();

//from postman - pass X-User-Id in header
app.Use(async (context, next) =>
{
    // DEV ONLY – REMOVE AFTER JWT
    if (context.Request.Headers.TryGetValue("X-User-Id", out var userId))
    {
        context.Items["UserId"] = int.Parse(userId!);
    }

    await next();
});

app.UseCors("DevCorsPolicy");

app.MapControllers();

// NHibernate configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
NHibernateHelper.Configure(connectionString);

app.Run();

