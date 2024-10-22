using System.Reflection;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using Ticketing.Application.Service.Massage;
using Ticketing.Application.Service.Project;
using Ticketing.Application.Service.Stauts;
using Ticketing.Application.Service.Ticket;
using Ticketing.Application.Service.TicketFlow;
using Ticketing.Domain.Contracts;
using Ticketing.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddFastEndpoints();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
builder.Services
    .AddFastEndpoints()
    .SwaggerDocument(); //define a swagger document

builder.Services.AddControllers();
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TicketingDbContext>(options =>options.UseSqlServer(connectionString));
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IProjectRoleService, ProjectRoleService>();
builder.Services.AddScoped<IMassageService, MassageService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ITicketFlowService, TicketFlowService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddSeq(builder.Configuration.GetSection("Seq"));
});

var assembly = Assembly.GetExecutingAssembly();
builder.Services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(assembly));
var app = builder.Build();

app.UseCors("AllowAll");
// Configure the HTTP request pipeline.


app.MapControllers();
app.UseFastEndpoints();


if (app.Environment.IsDevelopment())
{
    app.UseSwaggerGen(); 
}

app.Run();

