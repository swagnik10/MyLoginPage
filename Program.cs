using MyApp.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection
builder.Services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();

var app = builder.Build();

// Swagger (Development only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// NHibernate configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
NHibernateHelper.Configure(connectionString);

app.Run();

