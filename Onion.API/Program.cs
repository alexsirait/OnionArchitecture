using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Onion.Application.Service;
using Onion.Application.Service.Contract;
using Onion.Domain.Exceptions;
using Onion.Domain.Repositories;
using Onion.Presistance;
using System.Net;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

//---alex Presistance Service
builder.Services.AddDbContext<IDataContext, DataContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("DataContext")));

//---alex Add application services to the container.
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    //---alex Auto generate database
    var serviceDb = app.Services.GetService<IServiceScopeFactory>()?.CreateScope();
    var db = serviceDb?.ServiceProvider.GetRequiredService<DataContext>();
    db?.Database.EnsureCreated();
}

//---alex Global Error handler
app.UseExceptionHandler(x => {
    x.Run(async c =>
    {
        var contextFeature = c.Features.Get<IExceptionHandlerFeature>();

        if (contextFeature == null) return;
        c.Response.StatusCode = contextFeature.Error switch
        {
            OperationCanceledException => (int)HttpStatusCode.ServiceUnavailable,
            NotFoundException => (int)HttpStatusCode.NotFound,
            _ => (int)HttpStatusCode.InternalServerError,
        };

        var errorResponse = new
        {
            statusCode = c.Response.StatusCode,
            message = contextFeature.Error.GetBaseException()?.Message,
        };

        await c.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    });
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
