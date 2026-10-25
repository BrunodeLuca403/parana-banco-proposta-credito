using Microsoft.EntityFrameworkCore;
using PropostaCredito.Application.Services.Implementations;
using PropostaCredito.Application.Services.Interface;
using PropostaCredito.Core.Events;
using PropostaCredito.Core.Repository;
using PropostaCredito.Core.Services;
using PropostaCredito.Infrastructure.Messaging;
using PropostaCredito.Infrastructure.Persistence;
using PropostaCredito.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PropostaDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPropostaRepository, PropostaRepository>();
builder.Services.AddScoped<IScoreCalculo, ScoreCalculo>();
builder.Services.AddScoped<IPropostaService, PropostaService>();

builder.Services.Configure<RabbitMqSettings>(
    builder.Configuration.GetSection(RabbitMqSettings.SectionName));

builder.Services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
builder.Services.AddScoped<IPropostaEventPublisher, PropostaEventPublisher>();

builder.Services.AddHostedService<ClienteCadastradoConsumer>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
