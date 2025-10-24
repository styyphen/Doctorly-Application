using Microsoft.EntityFrameworkCore;
using HealthApp.Infrastructure.Data;
using HealthApp.Infrastructure.Repositories;
using HealthApp.Domain.Interfaces;
using HealthApp.Application.Services;
using HealthApp.Infrastructure.Services;
using HealthApp.Infrastructure.Configuration;
using Doctorly.Api.GraphQL;
using HotChocolate.Data;
using HotChocolate.AspNetCore;
using ChilliCream.Nitro.App;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add Entity Framework
builder.Services.AddDbContext<HealthAppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IAttendeeRepository, AttendeeRepository>();

// Add Email Service
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.AddScoped<IEmailService, EmailService>();

// Add MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssemblies(typeof(HealthApp.Application.Commands.CreatePatientCommand).Assembly);
});

// Add GraphQL with HotChocolate
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddProjections()
    .AddFiltering()
    .AddSorting();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Doctorly API - Event Scheduling",
        Version = "v1",
        Description = "API for managing doctor appointments and events",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Doctorly Team",
            Email = "support@doctorly.com"
        }
    });

    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Seed database in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<HealthAppDbContext>();
    await DatabaseSeeder.SeedAsync(context);
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add GraphQL endpoint
app.UseRouting();

app.MapControllers();
app.MapGraphQL()
   .WithOptions(new GraphQLServerOptions
   {
       Tool = { Enable = app.Environment.IsDevelopment() }
   });

app.Run();

public partial class Program { }
