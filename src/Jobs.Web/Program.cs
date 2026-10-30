using Copasogo.Application;
using Copasogo.Application.Generic;
using Copasogo.Application.Generic.Services.Interfaces;
using Copasogo.Infra.Data;
using Copasogo.Infra.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ICurrentUserContext, NullCurrentUserContext>();

builder.Services
    .AddApplication()
    .AddInfraData(builder.Configuration)
    .AddInfraJobs();

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "Jobs host healthy" }));

app.Run();
