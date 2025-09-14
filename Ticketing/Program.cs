using System.Reflection;
using Carter;
using Microsoft.EntityFrameworkCore;
using Ticketing.Application.Service.Massage;
using Ticketing.Application.Service.Project;
using Ticketing.Application.Service.Stauts;
using Ticketing.Application.Service.Ticket;
using Ticketing.Application.Service.TicketFlow;
using Ticketing.Domain.Contracts;
using Ticketing.EndPoints.Reporting.Query.Export;
using Ticketing.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TicketingDbContext>(options =>options.UseSqlServer(connectionString));
builder.Services.AddScoped<IStatusService, StatusService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IProjectRoleService, ProjectRoleService>();
builder.Services.AddScoped<IMassageService, MassageService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<ITicketFlowService, TicketFlowService>();
builder.Services.AddSingleton<IExport, ExportService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddSeq(builder.Configuration.GetSection("Seq"));
});

var assembly = Assembly.GetExecutingAssembly();
builder.Services.AddMediatR(configuration => configuration.RegisterServicesFromAssembly(assembly));
builder.Services.AddCarter(new DependencyContextAssemblyCatalog(assembly));
var app = builder.Build();

app.UseCors("AllowAll");
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
   
//}
app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
app.MapControllers();
app.MapCarter();
//var scope = app.Services.CreateScope();
//var dbcontext = scope.ServiceProvider.GetRequiredService<TicketingDbContext>();
//dbcontext.Database.Migrate();
app.Run();
